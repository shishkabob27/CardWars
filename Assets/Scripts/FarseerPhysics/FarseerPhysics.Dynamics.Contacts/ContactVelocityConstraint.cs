using FarseerPhysics.Common;
using UnityEngine;

namespace FarseerPhysics.Dynamics.Contacts
{
	public sealed class ContactVelocityConstraint
	{
		public VelocityConstraintPoint[] points;

		public Vector2 normal;

		public Mat22 normalMass;

		public Mat22 K;

		public int indexA;

		public int indexB;

		public float invMassA;

		public float invMassB;

		public float invIA;

		public float invIB;

		public float friction;

		public float restitution;

		public float tangentSpeed;

		public int pointCount;

		public int contactIndex;
	}
}
