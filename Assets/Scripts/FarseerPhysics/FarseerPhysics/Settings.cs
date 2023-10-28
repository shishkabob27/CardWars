using System;
using FarseerPhysics.Dynamics;
using UnityEngine;

namespace FarseerPhysics
{
	public static class Settings
	{
		public const float MaxFloat = float.MaxValue;

		public const float Epsilon = 1.1920929E-07f;

		public const float Pi = (float)Math.PI;

		public const int MaxSubSteps = 8;

		public const bool ConserveMemory = true;

		public const int MaxManifoldPoints = 2;

		public const float AABBExtension = 0.1f;

		public const float AABBMultiplier = 2f;

		public const float LinearSlop = 0.005f;

		public const float AngularSlop = 0.03490659f;

		public const float PolygonRadius = 0.01f;

		public const int MaxTOIContacts = 32;

		public const float VelocityThreshold = 1f;

		public const float MaxLinearCorrection = 0.2f;

		public const float MaxAngularCorrection = 0.13962635f;

		public const float Baumgarte = 0.2f;

		public const float TOIBaumgarte = 0.75f;

		public const float TimeToSleep = 0.5f;

		public const float LinearSleepTolerance = 0.01f;

		public const float AngularSleepTolerance = 0.03490659f;

		public const float MaxTranslation = 2f;

		public const float MaxTranslationSquared = 4f;

		public const float MaxRotation = (float)Math.PI / 2f;

		public const float MaxRotationSquared = 2.4674013f;

		public static int VelocityIterations = 8;

		public static int PositionIterations = 3;

		public static bool ContinuousPhysics = true;

		public static int TOIVelocityIterations = 8;

		public static int TOIPositionIterations = 20;

		public static bool EnableWarmstarting = true;

		public static bool AllowSleep = true;

		public static int MaxPolygonVertices = 16;

		public static bool UseFPECollisionCategories;

		public static Category DefaultFixtureCollisionCategories = Category.Cat1;

		public static Category DefaultFixtureCollidesWith = Category.All;

		public static Category DefaultFixtureIgnoreCCDWith;

		public static float MixFriction(float friction1, float friction2)
		{
			return Mathf.Sqrt(friction1 * friction2);
		}

		public static float MixRestitution(float restitution1, float restitution2)
		{
			return (!(restitution1 > restitution2)) ? restitution2 : restitution1;
		}
	}
}
