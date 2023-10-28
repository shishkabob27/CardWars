using System.Runtime.CompilerServices;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;

namespace FarseerPhysics.Dynamics.Contacts
{
	public class Contact
	{
		private enum ContactType
		{
			NotSupported,
			Polygon,
			PolygonAndCircle,
			Circle,
			EdgeAndPolygon,
			EdgeAndCircle,
			ChainAndPolygon,
			ChainAndCircle
		}

		private static EdgeShape _edge = new EdgeShape();

		private static ContactType[,] _registers = new ContactType[4, 4]
		{
			{
				ContactType.Circle,
				ContactType.EdgeAndCircle,
				ContactType.PolygonAndCircle,
				ContactType.ChainAndCircle
			},
			{
				ContactType.EdgeAndCircle,
				ContactType.NotSupported,
				ContactType.EdgeAndPolygon,
				ContactType.NotSupported
			},
			{
				ContactType.PolygonAndCircle,
				ContactType.EdgeAndPolygon,
				ContactType.Polygon,
				ContactType.ChainAndPolygon
			},
			{
				ContactType.ChainAndCircle,
				ContactType.NotSupported,
				ContactType.ChainAndPolygon,
				ContactType.NotSupported
			}
		};

		public Fixture FixtureA;

		public Fixture FixtureB;

		internal ContactFlags Flags;

		public Manifold Manifold;

		internal ContactEdge NodeA;

		internal ContactEdge NodeB;

		public float TOI;

		internal int TOICount;

		private ContactType _type;

		[CompilerGenerated]
		private float _003CFriction_003Ek__BackingField;

		[CompilerGenerated]
		private float _003CRestitution_003Ek__BackingField;

		[CompilerGenerated]
		private float _003CTangentSpeed_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CChildIndexA_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CChildIndexB_003Ek__BackingField;

		public float Friction
		{
			[CompilerGenerated]
			set
			{
				_003CFriction_003Ek__BackingField = value;
			}
		}

		public float Restitution
		{
			[CompilerGenerated]
			set
			{
				_003CRestitution_003Ek__BackingField = value;
			}
		}

		public float TangentSpeed
		{
			[CompilerGenerated]
			set
			{
				_003CTangentSpeed_003Ek__BackingField = value;
			}
		}

		internal int ChildIndexA
		{
			[CompilerGenerated]
			set
			{
				_003CChildIndexA_003Ek__BackingField = value;
			}
		}

		internal int ChildIndexB
		{
			[CompilerGenerated]
			set
			{
				_003CChildIndexB_003Ek__BackingField = value;
			}
		}

		public bool IsTouching()
		{
			return (Flags & ContactFlags.Touching) == ContactFlags.Touching;
		}

		private void Reset(Fixture fA, int indexA, Fixture fB, int indexB)
		{
			Flags = ContactFlags.Enabled;
			FixtureA = fA;
			FixtureB = fB;
			ChildIndexA = indexA;
			ChildIndexB = indexB;
			Manifold.PointCount = 0;
			NodeA.Contact = null;
			NodeA.Prev = null;
			NodeA.Next = null;
			NodeA.Other = null;
			NodeB.Contact = null;
			NodeB.Prev = null;
			NodeB.Next = null;
			NodeB.Other = null;
			TOICount = 0;
			if (FixtureA != null && FixtureB != null)
			{
				Friction = Settings.MixFriction(FixtureA.Friction, FixtureB.Friction);
				Restitution = Settings.MixRestitution(FixtureA.Restitution, FixtureB.Restitution);
			}
			TangentSpeed = 0f;
		}

		internal void Destroy()
		{
			FixtureA.Body.World.ContactManager.RemoveActiveContact(this);
			FixtureA.Body.World.ContactPool.Enqueue(this);
			Reset(null, 0, null, 0);
		}
	}
}
