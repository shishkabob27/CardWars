using System.Collections.Generic;

public class PointCloudGesture : DiscreteGesture
{
	public List<PointCloudRegognizer.Point> RawPoints = new List<PointCloudRegognizer.Point>(64);

	public List<PointCloudRegognizer.Point> NormalizedPoints = new List<PointCloudRegognizer.Point>(64);

	public PointCloudGestureTemplate RecognizedTemplate;

	public float MatchDistance;

	public float MatchScore;
}
