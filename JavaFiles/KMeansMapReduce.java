package solution;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.SequenceFile;
import org.apache.hadoop.mapreduce.Mapper;
import org.apache.hadoop.mapreduce.Reducer;


// first iteration, k-random centers, in every follow-up iteration we have new calculated centers
public class KMeansMapReduce
{	
	public static enum Counter 
	{
		CONVERGED
	}
	
	public static class Map extends Mapper<ClusterCenter, Vector, ClusterCenter, Vector> 
	{
		HashMap<ClusterCenter, ArrayList<Vector>> canopy = new HashMap<ClusterCenter, ArrayList<Vector>>();

		@Override
		protected void setup(Context context) throws IOException, InterruptedException 
		{
			super.setup(context);
			Utils.currkMeansCenters.clear();
			Utils.currCanopyCenters.clear();
			
			ClusterCenter canopyCenter = new ClusterCenter();
			ClusterCenter kCenter = new ClusterCenter();
			
			Configuration conf = context.getConfiguration();
			Path centroids = new Path(conf.get("kcenters.path"));
			FileSystem fs = FileSystem.get(conf);
			SequenceFile.Reader reader = new SequenceFile.Reader(fs, centroids,	conf);
	
			while (reader.next(canopyCenter, kCenter))
			{
				Utils.currkMeansCenters.add(new ClusterCenter(kCenter));
				Utils.currCanopyCenters.add(new ClusterCenter(canopyCenter));
			}
			
			reader.close();
		}
	
		@Override
		protected void map(ClusterCenter key, Vector value, Context context) throws IOException, InterruptedException 
		{
			// variable declaration
			ClusterCenter closestKmeansCenter = null; // no center is close at start
			ClusterCenter relatedCanopy = null;
			double closestDistance = Double.MAX_VALUE; // the distance to the non-existing center is infinity
			
			// loop centers
			for (int nIndex = 0; nIndex < Utils.currkMeansCenters.size(); nIndex++)
			{
				ClusterCenter currCanopyKey = new ClusterCenter(Utils.currCanopyCenters.get(nIndex));
				ClusterCenter kcenter = new ClusterCenter(Utils.currkMeansCenters.get(nIndex));
				
				if (key.compareTo(currCanopyKey) == 0)
				{
					// get the distance between the vector and the current center
					double centerDistance = DistanceMeasurer.measureDistance(kcenter, value);
					
					// if there is no nearest (i.e. first loop)
					if (closestKmeansCenter == null) 
					{
						// replace the current center to be the nearest 
						closestKmeansCenter = kcenter;
						relatedCanopy = currCanopyKey;
						closestDistance = centerDistance;
					} 
					else 
					{
						// if the current center is closer then the current nearest center
						if (closestDistance > centerDistance) 
						{
							// replace the current center to be the nearest
							closestKmeansCenter = kcenter;
							relatedCanopy = currCanopyKey;
							closestDistance = centerDistance;
						}
					}
				}
			}
			
			context.write(closestKmeansCenter, value);
		}
	}
	
	
	public static class Reduce extends Reducer<ClusterCenter, Vector, ClusterCenter, Vector> 
	{
		HashMap<ClusterCenter, ArrayList<Vector>> currClusters = new HashMap<ClusterCenter, ArrayList<Vector>>();
		public HashMap<ClusterCenter,ClusterCenter> link;
		
		@Override
		protected void setup(Context context) throws IOException, InterruptedException 
		{
			super.setup(context);
			Utils.newCanopyCenters.clear();
			Utils.newkMeansCenters.clear();
			link = new HashMap<ClusterCenter,ClusterCenter>();
			
			ClusterCenter canopyCenter = new ClusterCenter();
			ClusterCenter kCenter = new ClusterCenter();
			
			Configuration conf = context.getConfiguration();
			Path centroids = new Path(conf.get("kcenters.path"));
			FileSystem fs = FileSystem.get(conf);
			SequenceFile.Reader reader = new SequenceFile.Reader(fs, centroids,	conf);
	
			while (reader.next(canopyCenter, kCenter))
			{
				link.put(new ClusterCenter(kCenter), new ClusterCenter(canopyCenter));
			}
			
			reader.close();
		}
		
		@Override
		protected void reduce(ClusterCenter key, Iterable<Vector> values, Context context) throws IOException, InterruptedException 
		{
			ClusterCenter copyCenter = new ClusterCenter(key);
			
			// if we didnt get this center already
			if (!currClusters.containsKey(key))
			{
				// add to centers list
				currClusters.put(copyCenter, new ArrayList<Vector>());
				System.out.print(copyCenter.getCenter().getName() + " ----> ");
			}
	
			for (Vector vec : values)
			{
				// add all this reduce center vectors to current center list
				currClusters.get(copyCenter).add(new Vector(vec));		
				System.out.print(vec.getName() + ", ");
			}
			
			System.out.println();
		}
	
		@Override
		protected void cleanup(Context context) throws IOException,	InterruptedException 
		{
			super.cleanup(context);
			List<ClusterCenter> newkMeansCenters = new ArrayList<ClusterCenter>();
			List<ClusterCenter> newCanopyCenters = new ArrayList<ClusterCenter>();
			
			// moving on all centers
			for (ClusterCenter key : currClusters.keySet()) 
			{			
				ClusterCenter relatedCanopyCenter = this.link.get(key);		
		
				// calculate new center according to vectors
				Vector newCenter = new Vector(); 
				List<Vector> vectorList = new ArrayList<Vector>();
				int vectorSize = key.getCenter().getVector().length;
				newCenter.setVector(new double[vectorSize]);
	
				// sort all vectors in the list so we wont set different double results.
				Vector temp = new Vector();
				ArrayList<Vector> sorted = currClusters.get(key);
				for (int i = 0; i < sorted.size(); i++)
				{
					for (int j = 0; j < sorted.size(); j++) {
						if (sorted.get(i).getName().compareTo(sorted.get(j).getName()) > 0) {
							temp = sorted.get(j);
							sorted.set(j, sorted.get(i));
							sorted.set(i, temp);
						}
					}
				}
				
				// loop all the vectors in the current cluster
				for (Vector value : currClusters.get(key)) 
				{
					// add vector to the new center
					vectorList.add(new Vector(value));
	
					// sum all vectors to the new vector, so we could calculate the average lager
					for (int i = 0; i < value.getVector().length; i++) {
						newCenter.getVector()[i] += value.getVector()[i];
					}
				}
	
				// dividing all doubles in vector for average.
				for (int i = 0; i < newCenter.getVector().length; i++) {
					newCenter.getVector()[i] = newCenter.getVector()[i]	/ vectorList.size();
				}
	
				// create new center for next iterate
				ClusterCenter newKmeansCenterCopy = new ClusterCenter(newCenter);
				
				// send current key vectors with their new center.
				newkMeansCenters.add(newKmeansCenterCopy);
				newCanopyCenters.add(relatedCanopyCenter);
	
				// if the new center is equal to the old one, then we can stop
				relatedCanopyCenter.setCovered(newKmeansCenterCopy.converged(key)); 
				
				// send current key vectors with their new center.
				for (Vector vector : vectorList) 
				{
					context.write(newKmeansCenterCopy, vector);
				}
			}
	
			// write new centers to file 
			Configuration conf = context.getConfiguration();
			Path outPath = new Path(conf.get("kcenters.path"));
			FileSystem fs = FileSystem.get(conf);
			fs.delete(outPath, true);
	
			final SequenceFile.Writer out = SequenceFile.createWriter(fs, context.getConfiguration(), outPath, ClusterCenter.class, ClusterCenter.class);
			context.getCounter(Counter.CONVERGED).setValue(0);
			
			// going over the centers
			for (int nIndex = 0; nIndex < newkMeansCenters.size(); nIndex++)
			{
				out.append(newCanopyCenters.get(nIndex), newkMeansCenters.get(nIndex));
				if (newCanopyCenters.get(nIndex).getCovered()) 
					context.getCounter(Counter.CONVERGED).increment(1);
			}

			out.close();
		}
	}
}