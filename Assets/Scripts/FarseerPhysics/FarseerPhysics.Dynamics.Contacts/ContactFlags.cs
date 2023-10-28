using System;

namespace FarseerPhysics.Dynamics.Contacts
{
	[Flags]
	public enum ContactFlags
	{
		None = 0,
		Island = 1,
		Touching = 2,
		Enabled = 4,
		Filter = 8,
		BulletHit = 0x10,
		TOI = 0x20
	}
}
