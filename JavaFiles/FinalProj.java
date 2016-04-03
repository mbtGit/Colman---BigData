package solution;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FileStatus;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.SequenceFile;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.input.SequenceFileInputFormat;
import org.apache.hadoop.mapreduce.lib.input.TextInputFormat;
import org.apache.hadoop.mapreduce.lib.output.SequenceFileOutputFormat;
import org.apache.hadoop.mapreduce.lib.partition.HashPartitioner;

public class FinalProj
{
	// main that handles all steps
	public static void main(String[] args) throws Exception 
	{
		// Initialize values
		String inputDir = args[0]; 
		String outputDir = args[1]; 
		String T2 = args[2];
		String KArg = args[3]; 
		if (T2.equals("null")) T2 = "0.3";
		if (KArg.equals("null")) KArg = "5";
		int K = Integer.parseInt(KArg);
		
		// send input to canopy
		CanopyRunner(inputDir, "stocksproj/canopydata", T2, KArg);
		
		// Data
		HashMap<ClusterCenter, ArrayList<Vector>> canopyClusters = new HashMap<ClusterCenter, ArrayList<Vector>>();
		
		ArrayList<Vector> allVectors = new ArrayList<Vector>();
		
		// Load results
		LoadResultsData(canopyClusters, allVectors);
		int nCanopyClusterCount = canopyClusters.size();
		
		// Get K centers
		List<ClusterCenter> kMeansCenters = new ArrayList<ClusterCenter>();
		List<ClusterCenter> CanopyCenters = new ArrayList<ClusterCenter>();
		String strKmeansP = GetKCenters(K, canopyClusters, allVectors, kMeansCenters, CanopyCenters);
	
		// K-means
		Path result = KMeansRunner(K, canopyClusters, kMeansCenters, CanopyCenters, strKmeansP);
		
		// Write output
		Utils.WriteOutput(outputDir,result);
	}
	
	
	public static void LoadResultsData(HashMap<ClusterCenter, ArrayList<Vector>> canopyClusters, ArrayList<Vector> allVectors) throws IOException
	{
		Configuration conf = new Configuration();
		FileSystem fs = FileSystem.get(conf);
		FileStatus[] resultFiles = fs.listStatus(new Path("stocksproj/canopydata/"));
		
		// load the canopy result files
		for (FileStatus file : resultFiles)
		{
			if (file.getPath().toString().indexOf("canopydata/canopy") != -1)
			{
				System.out.println("reading file: " + file.getPath().toString());
				
				// read the current canopy file
				SequenceFile.Reader reader = new SequenceFile.Reader(fs, file.getPath(), conf);

				// Going over the stocks in the canopy and preparing an ArrayList 
				ClusterCenter center = new ClusterCenter();
				Vector vector = new Vector();

				// loop the values in the file
				while (reader.next(center, vector))
				{
					// if the center is not in the list, add it
					if (!canopyClusters.containsKey(center))
					{
						canopyClusters.put(new ClusterCenter(center), new ArrayList<Vector>());
					}

					// add the vector to its center in the canopies and to the vectors for k-means
					canopyClusters.get(center).add(new Vector(vector));
					allVectors.add(new Vector(vector));
				}
				
				reader.close();
			}
		}
	}
	public static String GetKCenters(int K, HashMap<ClusterCenter, ArrayList<Vector>> canopyClusters, ArrayList<Vector> allVectors, List<ClusterCenter> kMeansCenters, List<ClusterCenter> CanopyCenters) throws IOException
	{
		int kForCanopys = 0;
		int nRelativeK;
		int nKToCut = Utils.GetAmountOfKToCut(K, canopyClusters, allVectors);

		// Run over all canopies clusters
		for (java.util.Map.Entry<ClusterCenter, ArrayList<Vector>> entry : canopyClusters.entrySet()) 
		{
			nRelativeK = (int)Math.round((entry.getValue().size()/(allVectors.size() + 0.0)) * K);
			nRelativeK -= nRelativeK > nKToCut ? nKToCut : 0;
			nRelativeK = nRelativeK == 0 ? 1 : nRelativeK;
			
			if (kForCanopys < K) 
			{
				// Run as RelativeK size and pick R-K random centers
				for (int nRelativeKIndex = 0; nRelativeKIndex < nRelativeK; nRelativeKIndex++) 
				{
					int n = Utils.GetRandomIndex(entry.getValue().size(), false);
					kMeansCenters.add(new ClusterCenter(entry.getValue().get(n)));
					CanopyCenters.add(new ClusterCenter(entry.getKey()));
				}				
				
				Utils.GetRandomIndex(null, true);
				kForCanopys += nRelativeK;
			}
		} 

		Configuration conf = new Configuration();
		Path kCenters = new Path("stocksproj/clustering/data/kcenters.txt");
		FileSystem fs = FileSystem.get(conf);
		
		// Create files
		if (fs.exists(kCenters)) fs.delete(kCenters, true);

		SequenceFile.Writer kCentersWriter = SequenceFile.createWriter(fs, conf, kCenters, ClusterCenter.class, ClusterCenter.class);

		// going over the centers
		for (int nIndex = 0; nIndex < kMeansCenters.size(); nIndex++)
		{
			kCentersWriter.append(CanopyCenters.get(nIndex), kMeansCenters.get(nIndex));
		}
		
		kCentersWriter.close();
		
		return (kCenters.toString());
	}
	
	// Runners
	public static void CanopyRunner(String inputDir, String outputDir, String T2, String K) throws Exception
	{
		System.out.println("Canopy Map/Reduce.");

		Configuration conf = new Configuration();
		conf.setBoolean("fs.hdfs.impl.disable.cache", true);
		FileSystem fs = FileSystem.get(conf);
		Path in = new Path(inputDir);
		Path out = new Path(outputDir);
		conf.set("T2", T2);
		conf.set("K", K);

		// delete the existing output
		if (fs.exists(out)) fs.delete(out, true);

		Job job = new Job(conf);
		job.setJobName("StocksProj-Canopy");
		job.setJarByClass(CanopyMapReduce.class);

		// output key and value classes
		job.setOutputKeyClass(ClusterCenter.class);
		job.setOutputValueClass(Vector.class);

		// set map, reduce and partition classes
		job.setMapperClass(CanopyMapReduce.Map.class);
		job.setReducerClass(CanopyMapReduce.Reduce.class);
		job.setPartitionerClass(HashPartitioner.class);

		// set input and output classes
		job.setInputFormatClass(TextInputFormat.class);
		job.setOutputFormatClass(SequenceFileOutputFormat.class);

		// set input and output paths
		FileInputFormat.setInputPaths(job, in);
		SequenceFileOutputFormat.setOutputPath(job, out);

		// submit the job and wait for it to complete 
		job.waitForCompletion(true);
	}
	public static Path KMeansRunner(int K, HashMap<ClusterCenter, ArrayList<Vector>> canopyClusters, List<ClusterCenter> kMeansCenters, List<ClusterCenter> CanopyCenters, String strKmeansP) throws IOException, InterruptedException, ClassNotFoundException 
	{
		int nIteration = 0;
		
		System.out.println("KMeans Clustering");
		Path in = new Path("stocksproj/canopydata");
		Path out = new Path("stocksproj/kmeansout");
		Path kCenters = new Path(strKmeansP);
		Configuration conf = new Configuration();
		conf.setBoolean("fs.hdfs.impl.disable.cache", true);
		conf.set("num.iteration", nIteration + "");
		conf.set("K", K + "");
		conf.set("kcenters.path", kCenters.toString());
		Job job = new Job(conf);
		job.setJobName("KMeans Clustering");
		job.setMapperClass(KMeansMapReduce.Map.class);
		job.setReducerClass(KMeansMapReduce.Reduce.class);
		job.setJarByClass(KMeansMapReduce.class);
		job.setInputFormatClass(SequenceFileInputFormat.class);
		job.setOutputFormatClass(SequenceFileOutputFormat.class);
		job.setOutputKeyClass(ClusterCenter.class);
		job.setOutputValueClass(Vector.class);
		SequenceFileInputFormat.addInputPath(job, in);
		SequenceFileOutputFormat.setOutputPath(job, out);
		FileSystem fs = FileSystem.get(conf);
		if (fs.exists(out)) fs.delete(out, true);

		job.waitForCompletion(true);
		long lNumOfFinishedCenters = job.getCounters().findCounter(KMeansMapReduce.Counter.CONVERGED).getValue();
		
		while (lNumOfFinishedCenters < K) 
		{
			nIteration++;
			
			conf = new Configuration();
			conf.setBoolean("fs.hdfs.impl.disable.cache", true);
			conf.set("kcenters.path", kCenters.toString());
			conf.set("num.iteration", nIteration + "");
			job = new Job(conf);
			job.setJobName("KMeans Clustering " + nIteration);
			job.setMapperClass(KMeansMapReduce.Map.class);
			job.setReducerClass(KMeansMapReduce.Reduce.class);
			job.setJarByClass(KMeansMapReduce.class);
			job.setInputFormatClass(SequenceFileInputFormat.class);
			job.setOutputFormatClass(SequenceFileOutputFormat.class);
			job.setOutputKeyClass(ClusterCenter.class);
			job.setOutputValueClass(Vector.class);
			SequenceFileInputFormat.addInputPath(job, in);
			SequenceFileOutputFormat.setOutputPath(job, out);
			if (fs.exists(out))
				fs.delete(out, true);

			job.waitForCompletion(true);
			lNumOfFinishedCenters = job.getCounters().findCounter(KMeansMapReduce.Counter.CONVERGED).getValue();
		}

		return (new Path("stocksproj/kmeansout/part-r-00000"));
	}
}