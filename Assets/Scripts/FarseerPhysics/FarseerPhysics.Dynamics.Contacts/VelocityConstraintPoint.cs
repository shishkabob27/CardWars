using UnityEngine;

namespace FarseerPhysics.Dynamics.Contacts
{
	public sealed class VelocityConstraintPoint
	{
		public Vector2 rA;

		public Vector2 rB;

		public float normalImpulse;

		public float tangentImpulse;

		public float normalMass;

		public float tangentMass;

		public float velocityBias;
	}
}
