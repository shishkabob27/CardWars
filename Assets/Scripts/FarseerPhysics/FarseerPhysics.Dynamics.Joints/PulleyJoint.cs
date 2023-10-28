using UnityEngine;

namespace FarseerPhysics.Dynamics.Joints
{
	public class PulleyJoint : Joint2D
	{
		public Vector2 GroundAnchorA;

		public Vector2 GroundAnchorB;

		public Vector2 LocalAnchorA;

		public Vector2 LocalAnchorB;

		private float _impulse;

		private float _limitImpulse1;

		private float _limitImpulse2;

		private float m_constant;

		private int m_indexA;

		private int m_indexB;

		private Vector2 m_uA;

		private Vector2 m_uB;

		private Vector2 m_rA;

		private Vector2 m_rB;

		private Vector2 m_localCenterA;

		private Vector2 m_localCenterB;

		private float m_invMassA;

		private float m_invMassB;

		private float m_invIA;

		private float m_invIB;

		private float m_mass;

		public override Vector2 WorldAnchorA
		{
			get
			{
				return base.BodyA.GetWorldPoint(LocalAnchorA);
			}
		}

		public override Vector2 WorldAnchorB
		{
			get
			{
				return base.BodyB.GetWorldPoint(LocalAnchorB);
			}
		}
	}
}
