using System;
using System.Collections.Generic;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;

namespace FarseerPhysics.Dynamics
{
	public class Fixture : IDisposable
	{
		private static int _fixtureIdCounter;

		public AfterCollisionEventHandler AfterCollision;

		public BeforeCollisionEventHandler BeforeCollision;

		public OnCollisionEventHandler OnCollision;

		public OnSeparationEventHandler OnSeparation;

		public FixtureProxy[] Proxies;

		public int ProxyCount;

		internal Category _collidesWith;

		internal Category _collisionCategories;

		internal short _collisionGroup;

		internal Dictionary<int, bool> _collisionIgnores;

		public Category IgnoreCCDWith;

		private float _friction;

		private float _restitution;

		private bool _isSensor;

		public ShapeType ShapeType
		{
			get
			{
				return Shape.ShapeType;
			}
		}

		public Shape Shape { get; internal set; }

		public Body Body { get; internal set; }

		public float Friction
		{
			get
			{
				return _friction;
			}
		}

		public float Restitution
		{
			get
			{
				return _restitution;
			}
		}

		public bool IsDisposed { get; set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Body.DestroyFixture(this);
				IsDisposed = true;
				GC.SuppressFinalize(this);
			}
		}

		internal void Destroy()
		{
			Proxies = null;
			Shape = null;
			BeforeCollision = null;
			OnCollision = null;
			OnSeparation = null;
			AfterCollision = null;
			if (Body.World.FixtureRemoved != null)
			{
				Body.World.FixtureRemoved(this);
			}
			Body.World.FixtureAdded = null;
			Body.World.FixtureRemoved = null;
			OnSeparation = null;
			OnCollision = null;
		}

		internal void DestroyProxies(IBroadPhase broadPhase)
		{
			for (int i = 0; i < ProxyCount; i++)
			{
				broadPhase.RemoveProxy(Proxies[i].ProxyId);
				Proxies[i].ProxyId = -1;
			}
			ProxyCount = 0;
		}
	}
}
