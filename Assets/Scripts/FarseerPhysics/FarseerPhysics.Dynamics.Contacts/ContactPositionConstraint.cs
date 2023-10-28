using FarseerPhysics.Collision;
using UnityEngine;

namespace FarseerPhysics.Dynamics.Contacts
{
	public sealed class ContactPositionConstraint
	{
		public Vector2[] localPoints;

		public Vector2 localNormal;

		public Vector2 localPoint;

		public int indexA;

		public int indexB;

		public float invMassA;

		public float invMassB;

		public Vector2 localCenterA;

		public Vector2 localCenterB;

		public float invIA;

		public float invIB;

		public ManifoldType type;

		public float radiusA;

		public float radiusB;

		public int pointCount;
	}
}
