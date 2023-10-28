using System;

namespace FarseerPhysics.Dynamics
{
	[Flags]
	public enum WorldFlags
	{
		NewFixture = 1,
		ClearForces = 4,
		SubStepping = 0x10
	}
}
