using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using UnityEngine;

namespace FarseerPhysics.Dynamics
{
	public class World
	{
		public BodyDelegate BodyAdded;

		public BodyDelegate BodyRemoved;

		internal Queue<Contact> ContactPool;

		public FixtureDelegate FixtureAdded;

		public FixtureDelegate FixtureRemoved;

		internal WorldFlags Flags;

		public JointDelegate JointAdded;

		public JointDelegate JointRemoved;

		public ControllerDelegate ControllerAdded;

		public ControllerDelegate ControllerRemoved;

		private float _invDt0;

		public Island Island;

		private Body[] _stack;

		private bool _stepComplete;

		private HashSet<Body> _bodyAddList;

		private HashSet<Body> _bodyRemoveList;

		private HashSet<Joints.Joint2D> _jointAddList;

		private HashSet<Joints.Joint2D> _jointRemoveList;

		private TOIInput _input;

		public bool Enabled;

		public Vector2 Gravity;

		private List<Body> AwakeBodyList;

		private HashSet<Body> IslandSet;

		private HashSet<Body> TOISet;

		[CompilerGenerated]
		private ContactManager _003CContactManager_003Ek__BackingField;

		[CompilerGenerated]
		private HashSet<Body> _003CAwakeBodySet_003Ek__BackingField;

		public ContactManager ContactManager
		{
			[CompilerGenerated]
			get
			{
				return _003CContactManager_003Ek__BackingField;
			}
		}

		public HashSet<Body> AwakeBodySet
		{
			[CompilerGenerated]
			get
			{
				return _003CAwakeBodySet_003Ek__BackingField;
			}
		}

		public void RemoveBody(Body body)
		{
			if (!_bodyRemoveList.Contains(body))
			{
				_bodyRemoveList.Add(body);
			}
			if (AwakeBodySet.Contains(body))
			{
				AwakeBodySet.Remove(body);
			}
		}
	}
}
