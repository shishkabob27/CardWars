using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FarseerPhysics.Dynamics.Joints
{
	public abstract class Joint2D
	{
		public float Breakpoint;

		internal JointEdge EdgeA;

		internal JointEdge EdgeB;

		public bool Enabled;

		internal bool IslandFlag;

		protected int m_index;

		private Action<Joint2D, float> Broke;

		[CompilerGenerated]
		private JointType _003CJointType_003Ek__BackingField;

		[CompilerGenerated]
		private Body _003CBodyA_003Ek__BackingField;

		[CompilerGenerated]
		private Body _003CBodyB_003Ek__BackingField;

		public JointType JointType
		{
			[CompilerGenerated]
			get
			{
				return _003CJointType_003Ek__BackingField;
			}
		}

		public Body BodyA
		{
			[CompilerGenerated]
			get
			{
				return _003CBodyA_003Ek__BackingField;
			}
		}

		public Body BodyB
		{
			[CompilerGenerated]
			get
			{
				return _003CBodyB_003Ek__BackingField;
			}
		}

		public abstract Vector2 WorldAnchorA { get; }

		public abstract Vector2 WorldAnchorB { get; }

		public bool IsFixedType()
		{
			return JointType == JointType.FixedRevolute || JointType == JointType.FixedDistance || JointType == JointType.FixedPrismatic || JointType == JointType.FixedLine || JointType == JointType.FixedMouse || JointType == JointType.FixedAngle || JointType == JointType.FixedFriction;
		}
	}
}
