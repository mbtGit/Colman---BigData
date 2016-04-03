package solution;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;

import org.apache.hadoop.io.WritableComparable;

public class ClusterCenter implements WritableComparable<ClusterCenter> {

	private Vector center;
	private int id;
	private boolean covered;
	
	// default constructor
	public ClusterCenter() {
		super();
		this.center = null;
		this.id = -1;
		this.covered = false;
	}

	// copy constructor
	public ClusterCenter(ClusterCenter center) {
		super();
		this.center = new Vector(center.center);
		this.id = center.id;
		this.covered = center.covered;
	}

	// turn Vector to a center constructor
	public ClusterCenter(Vector center, int nClusterId) {
		super();
		this.center = center;
		this.id = nClusterId;
		this.covered = false;
	}
	// turn Vector to a center constructor
	public ClusterCenter(Vector center) {
		super();
		this.center = center;
		this.id = -1;
		this.covered = false;
	}
	
	// return if this and the parameter center are converged
	public boolean converged(ClusterCenter c) {
		return compareTo(c) == 0 ? true : false;
	}

	// write the Center into the output
	@Override
	public void write(DataOutput out) throws IOException {
		out.writeInt(this.id);
		out.writeBoolean(this.covered);
		center.write(out);
	}
	
	// changes the center according to the data in the input
	@Override
	public void readFields(DataInput in) throws IOException {
		this.id = in.readInt();
		this.covered = in.readBoolean();
		this.center = new Vector();
		center.readFields(in);
		
	}

	// compare the two centers' vectors
	@Override
	public int compareTo(ClusterCenter o) {
		int res = center.compareTo(o.getCenter());
		return res;
	}

	// returns if this and the parameter centers' vectors are equal
	@Override
	public boolean equals(Object o)	{
		return (this.compareTo((ClusterCenter)o) == 0);
	}

	// getter
	public Vector getCenter() {
		return center;
	}
	
	public int getId() {
		return id;
	}
	
	public boolean getCovered() {
		return covered;
	}
	
	public void setCovered(boolean b) {
		covered = b;
	}
	
	// toString
	@Override
	public String toString() {
		return "ClusterCenter " + center.getName() + " [center=" + center + "]";
	}

	// return a hash code based on the centers' string
	@Override
	public int hashCode(){
		int res = this.toString().hashCode();
		return res;
	}

}
