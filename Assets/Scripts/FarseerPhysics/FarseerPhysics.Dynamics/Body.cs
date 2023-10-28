using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.FakeXna;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics.Contacts;
using UnityEngine;

namespace FarseerPhysics.Dynamics
{
	public class Body : IDisposable
	{
		private static int _bodyIdCounter;

		internal float AngularVelocityInternal;

		public int BodyId;

		public ControllerFilter ControllerFilter;

		internal BodyFlags Flags;

		internal Vector2 Force;

		internal float InvI;

		internal float InvMass;

		internal Vector2 LinearVelocityInternal;

		public PhysicsLogicFilter PhysicsLogicFilter;

		internal float SleepTime;

		internal Sweep Sweep;

		internal float Torque;

		internal World World;

		internal Transform2D Xf;

		private float _angularDamping;

		private BodyType _bodyType;

		private float _inertia;

		private float _linearDamping;

		private float _mass;

		public int IslandIndex;

		[CompilerGenerated]
		private List<Fixture> _003CFixtureList_003Ek__BackingField;

		public BodyType BodyType
		{
			get
			{
				return _bodyType;
			}
		}

		public List<Fixture> FixtureList
		{
			[CompilerGenerated]
			get
			{
				return _003CFixtureList_003Ek__BackingField;
			}
		}

		public ContactEdge ContactList { get; internal set; }

		public bool IsDisposed { get; set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				World.RemoveBody(this);
				IsDisposed = true;
				GC.SuppressFinalize(this);
			}
		}

		public void DestroyFixture(Fixture fixture)
		{
			ContactEdge contactEdge = ContactList;
			while (contactEdge != null)
			{
				Contact contact = contactEdge.Contact;
				contactEdge = contactEdge.Next;
				Fixture fixtureA = contact.FixtureA;
				Fixture fixtureB = contact.FixtureB;
				if (fixture == fixtureA || fixture == fixtureB)
				{
					World.ContactManager.Destroy(contact);
				}
			}
			if ((Flags & BodyFlags.Enabled) == BodyFlags.Enabled)
			{
				IBroadPhase broadPhase = World.ContactManager.BroadPhase;
				fixture.DestroyProxies(broadPhase);
			}
			FixtureList.Remove(fixture);
			fixture.Destroy();
			fixture.Body = null;
			ResetMassData();
		}

		public void GetTransform(out Transform2D transform)
		{
			transform = Xf;
		}

		public void ResetMassData()
		{
			_mass = 0f;
			InvMass = 0f;
			_inertia = 0f;
			InvI = 0f;
			Sweep.LocalCenter = Vector2.zero;
			if (BodyType == BodyType.Kinematic)
			{
				Sweep.C0 = Xf.p;
				Sweep.C = Xf.p;
				Sweep.A0 = Sweep.A;
				return;
			}
			Vector2 zero = Vector2.zero;
			foreach (Fixture fixture in FixtureList)
			{
				if (fixture.Shape._density != 0f)
				{
					MassData massData = fixture.Shape.MassData;
					_mass += massData.Mass;
					zero += massData.Mass * massData.Centroid;
					_inertia += massData.Inertia;
				}
			}
			if (BodyType == BodyType.Static)
			{
				Sweep.C0 = (Sweep.C = Xf.p);
				return;
			}
			if (_mass > 0f)
			{
				InvMass = 1f / _mass;
				zero *= InvMass;
			}
			else
			{
				_mass = 1f;
				InvMass = 1f;
			}
			if (_inertia > 0f && (Flags & BodyFlags.FixedRotation) == 0)
			{
				_inertia -= _mass * VectorHelper.Dot(zero, zero);
				InvI = 1f / _inertia;
			}
			else
			{
				_inertia = 0f;
				InvI = 0f;
			}
			Vector2 c = Sweep.C;
			Sweep.LocalCenter = zero;
			Sweep.C0 = (Sweep.C = MathUtils.Mul(ref Xf, ref Sweep.LocalCenter));
			Vector2 vector = Sweep.C - c;
			LinearVelocityInternal += new Vector2((0f - AngularVelocityInternal) * vector.y, AngularVelocityInternal * vector.x);
		}

		public Vector2 GetWorldPoint(ref Vector2 localPoint)
		{
			return MathUtils.Mul(ref Xf, ref localPoint);
		}

		public Vector2 GetWorldPoint(Vector2 localPoint)
		{
			return GetWorldPoint(ref localPoint);
		}
	}
}
