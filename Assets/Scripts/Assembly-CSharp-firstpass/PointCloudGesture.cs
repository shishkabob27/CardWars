using System.Collections.Generic;

public class PointCloudGesture : DiscreteGesture
{
	public List<PointCloudRegognizer.Point> RawPoints;
	public List<PointCloudRegognizer.Point> NormalizedPoints;
	public PointCloudGestureTemplate RecognizedTemplate;
	public float MatchDistance;
	public float MatchScore;
}
