package solution;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;
import java.util.Arrays;

import org.apache.hadoop.io.WritableComparable;

public class Vector implements WritableComparable<Vector> {

	private double[] vector;
	private String name = "";
	
	// default constructor
	public Vector() {
		super();
	}

	// copy constructor
	public Vector(Vector v) {
		super();
		int l = v.vector.length;
		this.vector = new double[l];
		System.arraycopy(v.vector, 0, this.vector, 0, l);
		this.name = v.getName();
	}
	
	// constructor that defines the size of the double array
	public Vector(int size)	{
		super();
		this.vector = new double[size];
		this.name = "BLANK";
	}

	// constructor based on x,y values, the size of the array is 2
	public Vector(double x, double y) {
		super();
		this.vector = new double[] { x, y };
	}

	// write the Vector into the output
	@Override
	public void write(DataOutput out) throws IOException {
		out.writeInt(vector.length);
		out.writeUTF(name);
		for (int i = 0; i < vector.length; i++)
			out.writeDouble(vector[i]);
	}

	// changes the Vector according to the data in the input
	@Override
	public void readFields(DataInput in) throws IOException {
		int size = in.readInt();
		name = in.readUTF();
		vector = new double[size];
		for (int i = 0; i < size; i++)
			vector[i] = in.readDouble();
	}

	// compares this vector to v, filed by filed
	@Override
	public int compareTo(Vector v) {

		boolean equals = true;
		for (int i = 0; i < vector.length; i++) {
			if (vector[i] != v.vector[i]) {
				equals = false;
				break;
			}
		}
		if (equals)
			return 0;
		else
			return 1;
	}

	// getters
	public double[] getVector() {
		return vector;
	}
	
	public String getName()	{
		return name;
	}
	
	
	// setters
	public void setName(String value) {
		this.name = value;
	}
	
	public void setVector(double[] vector) {
		this.vector = vector;
	}

	// toString
	@Override
	public String toString() {
		return "Vector " + name + "  [vector=" + Arrays.toString(vector) + "]";
	}

}
