package solution;

public class DistanceMeasurer {

	public static final double measureDistance(ClusterCenter center, Vector v) {
		double sum = 0;
		int length = v.getVector().length;
		for (int i = 0; i < length; i++) {
			sum += Math.abs(center.getCenter().getVector()[i]
					- v.getVector()[i]);
		}

		return sum / length;
	}
	
	// returns the distance between a center and a vector
	// the distance is the sum of the differences of every element of the center's vector and the paramter's vector values
	/*public static final double measureDistance(ClusterCenter center, Vector v) {
		
		double min = 0;
		double max = 0;
		
		min = Math.min(center.getCenter().getVector()[0], v.getVector()[0]);
		max = Math.max(center.getCenter().getVector()[0], v.getVector()[0]);
		
		for (int i = 1; i < center.getCenter().getVector().length; i++) {
			if (center.getCenter().getVector()[i] < min) {
				min = center.getCenter().getVector()[i];
			}
			if (center.getCenter().getVector()[i] > max) {
				max = center.getCenter().getVector()[i];
			}
			if (v.getVector()[i] < min) {
				min = v.getVector()[i];
			}
			if (v.getVector()[i] > max) {
				max = v.getVector()[i];
			}
		}

		double sum = 0;
		
		double currCenterVal;
		double currVecVal;
		int length = Math.min(center.getCenter().getVector().length, v.getVector().length);
		for (int i = 0; i < length; i++) {
			currCenterVal = ((center.getCenter().getVector()[i] - min)  * (1-0)) /  (max - min);
			currVecVal = ((v.getVector()[i] - min)  * (1-0)) /  (max - min);
			sum += Math.abs(currCenterVal - currVecVal);
		}
		return sum;
	}*/
}
