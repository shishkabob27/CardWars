using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;

namespace FarseerPhysics.Dynamics
{
	public class Island
	{
		private const float LinTolSqr = 0.0001f;

		private const float AngTolSqr = 0.0012184699f;

		public Body[] Bodies;

		public int BodyCount;

		public int ContactCount;

		public int JointCount;

		public Velocity[] _velocities;

		public Position[] _positions;

		public int BodyCapacity;

		public int ContactCapacity;

		private ContactManager _contactManager;

		private ContactSolver _contactSolver;

		private Contact[] _contacts;

		public int JointCapacity;

		private Joint2D[] _joints;

		public float JointUpdateTime;

		private float _tmpTime;
	}
}
