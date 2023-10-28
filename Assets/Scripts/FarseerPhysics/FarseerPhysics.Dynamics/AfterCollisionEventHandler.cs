using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics
{
	public delegate void AfterCollisionEventHandler(Fixture fixtureA, Fixture fixtureB, Contact contact);
}
