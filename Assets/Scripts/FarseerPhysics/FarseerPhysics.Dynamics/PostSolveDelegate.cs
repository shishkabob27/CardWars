using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics
{
	public delegate void PostSolveDelegate(Contact contact, ContactVelocityConstraint impulse);
}
