namespace FarseerPhysics.Dynamics.Joints
{
	public sealed class JointEdge
	{
		public Joint2D Joint2D;

		public JointEdge Next;

		public Body Other;

		public JointEdge Prev;
	}
}
