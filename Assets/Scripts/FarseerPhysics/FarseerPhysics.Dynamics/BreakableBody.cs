using System.Collections.Generic;
using UnityEngine;

namespace FarseerPhysics.Dynamics
{
	public class BreakableBody
	{
		public bool Broken;

		public Body MainBody;

		public List<Fixture> Parts;

		public float Strength;

		private float[] _angularVelocitiesCache;

		private bool _break;

		private Vector2[] _velocitiesCache;

		private World _world;
	}
}
