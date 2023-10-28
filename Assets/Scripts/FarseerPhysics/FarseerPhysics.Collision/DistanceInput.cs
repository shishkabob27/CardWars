using FarseerPhysics.Common;

namespace FarseerPhysics.Collision
{
	public class DistanceInput
	{
		public DistanceProxy ProxyA = new DistanceProxy();

		public DistanceProxy ProxyB = new DistanceProxy();

		public Transform2D TransformA;

		public Transform2D TransformB;

		public bool UseRadii;
	}
}
