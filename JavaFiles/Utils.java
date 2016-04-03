package solution;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.List;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FSDataOutputStream;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.SequenceFile;

public class Utils 
{
	public static Hashtable<Integer,Integer> randomIndexMap;
	public static List<ClusterCenter> currkMeansCenters;
	public static List<ClusterCenter> currCanopyCenters;
	public static List<ClusterCenter> newkMeansCenters;
	public static List<ClusterCenter> newCanopyCenters;
	
	
	static
	{
		Utils.randomIndexMap = new Hashtable<Integer, Integer>();
		currkMeansCenters = new ArrayList<ClusterCenter>();
		currCanopyCenters = new ArrayList<ClusterCenter>();
		newkMeansCenters = new ArrayList<ClusterCenter>();
		newCanopyCenters = new ArrayList<ClusterCenter>();
	}
	
	public static int GetAmountOfKToCut(int K, HashMap<ClusterCenter, ArrayList<Vector>> canopyClusters, ArrayList<Vector> vectorsForKMeans)
	{
		int nTemp = 0;
		int nSumOfK = 0;
		int nSelfKAdded = 0;
		
		// Find how much interventions we will need to make for the linearic distribution 
		for (java.util.Map.Entry<ClusterCenter, ArrayList<Vector>> entry : canopyClusters.entrySet()) 
		{
			nTemp = (int)Math.round((entry.getValue().size()/(vectorsForKMeans.size() + 0.0)) * K);
			nSumOfK += nTemp;
			nSelfKAdded += nTemp < 1 ? 1 : 0;
		}
		
		return (nSelfKAdded + K - nSumOfK);
	}
	public static int GetRandomIndex(Integer nVecSize, boolean bInitializedRandom)
	{
		int nNumToReturn = -1;
		
		if (bInitializedRandom) {randomIndexMap.clear();}
		
		if (nVecSize != null)
		{
			nNumToReturn = (int)Math.floor(Math.random() * nVecSize);
			while (randomIndexMap.containsKey(nNumToReturn)) { nNumToReturn = (int)Math.floor(Math.random() * nVecSize); }
			randomIndexMap.put(nNumToReturn, nNumToReturn);
		}
		
		return (nNumToReturn);
	}
	public static void WriteOutput(String outputFile, Path ResultPath) throws IOException, InterruptedException, ClassNotFoundException 
	{
		HashMap<ClusterCenter, ArrayList<Vector>> clusters = new HashMap<ClusterCenter, ArrayList<Vector>>();
		Path outputPath = new Path(outputFile);
		Configuration conf = new Configuration();
		FileSystem fs = FileSystem.get(conf);
		StringBuilder output = new StringBuilder();
		
		System.out.println("Build output file");
		
		// delete existing file
		if (fs.exists(outputPath)) fs.delete(outputPath, true);

		// the kmeans results
		SequenceFile.Reader reader = new SequenceFile.Reader(fs, ResultPath, conf);
		ClusterCenter center = new ClusterCenter();
		Vector vector = new Vector();
		
		while (reader.next(center, vector))
		{
			if (!clusters.containsKey(center))
			{
				clusters.put(new ClusterCenter(center),	new ArrayList<Vector>());
			}
			// add the vector to his center
			clusters.get(center).add(new Vector(vector));
		}
		reader.close();


		// for each center, add all its vectors to the final output
		for (ClusterCenter key : clusters.keySet())
		{
			for (Vector vec : clusters.get(key))
			{
				output.append(String.format("%s,", vec.getName()));
			}
			
			output.deleteCharAt(output.length() - 1);
			output.append("\n");
		}
		
		System.out.println(output);
		
		FSDataOutputStream out = fs.create(outputPath);
		out.writeUTF(output.toString());
		out.close();
	}
	// create Vector instance from file string
	public static Vector inputToVector(String input)	
	{

        String stockName = input.substring(0, input.indexOf(' '));
        String strMenaiot = input.substring(input.indexOf(' ') + 1);

		String[] arrStrMenaiot = strMenaiot.split(",");
		ArrayList<Double> stockValues = new ArrayList<Double>();

        for(String s: arrStrMenaiot) {
        	stockValues.add(new Double(Double.parseDouble(s)));
	    }
        
		// create a new vector and put the data in it
		Vector stockVector = new Vector();
		stockVector.setVector(toDoubleArray(stockValues));
		stockVector.setName(stockName);

		return stockVector;
	}
	
	// convert an array list to double list
	public static double[] toDoubleArray(ArrayList<Double> input)	
	{
		double[] doubleList = new double[input.size()];
		int i = 0;

		for (Double d : input)	{
			doubleList[i] = d;
			i++;
		}

		return doubleList;
	}
}
