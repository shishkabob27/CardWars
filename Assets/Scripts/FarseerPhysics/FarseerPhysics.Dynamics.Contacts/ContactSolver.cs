namespace FarseerPhysics.Dynamics.Contacts
{
	public class ContactSolver
	{
		public TimeStep _step;

		public Position[] _positions;

		public Velocity[] _velocities;

		public ContactPositionConstraint[] _positionConstraints;

		public ContactVelocityConstraint[] _velocityConstraints;

		public Contact[] _contacts;

		public int _count;
	}
}
