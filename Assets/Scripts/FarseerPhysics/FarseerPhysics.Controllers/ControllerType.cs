using System;

namespace FarseerPhysics.Controllers
{
	[Flags]
	public enum ControllerType
	{
		GravityController = 1,
		VelocityLimitController = 2,
		AbstractForceController = 4,
		BuoyancyController = 8
	}
}
