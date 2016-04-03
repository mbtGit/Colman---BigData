package solution;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Hashtable;

import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.SequenceFile;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;
import org.apache.hadoop.mapreduce.Reducer;
import org.apache.hadoop.mapreduce.Reducer.Context;

public class CanopyMapReduce 
{
	public static ArrayList<ClusterCenter> kmeansCenters = new ArrayList<ClusterCenter>();
	
	public static class Map extends Mapper<LongWritable, Text, ClusterCenter, Vector> 
	{
		public static ArrayList<ClusterCenter> canopyCenters = new ArrayList<ClusterCenter>();
		public static int nCanopyMapperCounter = 0;
		
		public void map(LongWritable key, Text value, Context context) throws IOException, InterruptedException 
		{	
			String textVal = value.toString();
			Vector stockVector = Utils.inputToVector(textVal);
			
			// Canopy T2 radios
			double T2 = Double.parseDouble(context.getConfiguration().get("T2"));
			boolean isClusterFound = false;
			
			// Run over all already clusters 
			for (ClusterCenter cCurrCenter : canopyCenters) 
			{
				// if distance smaller then t2, add vector to center
				if (DistanceMeasurer.measureDistance(cCurrCenter, stockVector) < T2)	
				{
					context.write(cCurrCenter, stockVector);
					isClusterFound = true;
					break;
				}
			}

			// If no center is close enough to be cluster with the current map iteration
			if (!isClusterFound) 
			{
				ClusterCenter newCenter = new ClusterCenter(stockVector, Map.nCanopyMapperCounter);
				canopyCenters.add(newCenter);
				context.write(newCenter, stockVector);
			}
			
			Map.nCanopyMapperCounter++;
		}
	}


	
	
	public static class Reduce extends Reducer<ClusterCenter, Vector, Text, Text> 
	{
		public static Hashtable<ClusterCenter, ArrayList<Vector>> reducedCenters = new Hashtable<ClusterCenter, ArrayList<Vector>>();
		
		public void reduce(ClusterCenter key, Iterable<Vector> values, Context context) throws IOException, InterruptedException 
		{
			
			double T2 = Double.parseDouble(context.getConfiguration().get("T2"));
			boolean isClusterFound = false;

			// Run over all already reduced centers list 
			for (java.util.Map.Entry<ClusterCenter, ArrayList<Vector>> currCenterEntry : reducedCenters.entrySet()) 
			{
				ClusterCenter cCurrCenter = currCenterEntry.getKey();

				// if distance smaller then t2, add all centers vectors to current center
				if (DistanceMeasurer.measureDistance(cCurrCenter, key.getCenter()) < T2)	
				{						
					for (Vector v : values)
					{
						currCenterEntry.getValue().add(new Vector(v));
					}

					isClusterFound = true;
					break;
				}
			}

			// If no center is close enough to be cluster with the current map iteration
			if (!isClusterFound) 
			{
				ClusterCenter currCenter = new ClusterCenter(key);
				reducedCenters.put(currCenter, new ArrayList<Vector>());
				for (Vector v : values)
				{
					reducedCenters.get(currCenter).add(new Vector(v));
				}
			}
		}
		
		@Override
		protected void cleanup(Context context) throws IOException,	InterruptedException 
		{			
			// Run over all already reduced centers list 
			for (java.util.Map.Entry<ClusterCenter, ArrayList<Vector>> currCenterEntry : reducedCenters.entrySet()) 
			{
				ClusterCenter cCurrCenter = currCenterEntry.getKey();

				// Reduce centers by "merging the centers vectors" to same file according the center id
				WriteVectorsToCenterFile(context , String.format("stocksproj/canopydata/canopy%d", cCurrCenter.getId()), cCurrCenter, currCenterEntry.getValue());
			}
		}
		
		public void WriteVectorsToCenterFile(Context context, String strFullPath, ClusterCenter cCenterFile, Iterable<Vector> values) throws IOException
		{
			FileSystem fs = FileSystem.get(context.getConfiguration());
			SequenceFile.Writer writer = SequenceFile.createWriter(fs, context.getConfiguration(), new Path(strFullPath), ClusterCenter.class, Vector.class);

			// Run over all vectors
			for (Vector vCurrVector : values) 
			{
				writer.append(cCenterFile, vCurrVector);
			}
			
			writer.close();
			fs.close();
		}
	}
}