using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics
{
	public delegate bool OnCollisionEventHandler(Fixture fixtureA, Fixture fixtureB, Contact contact);
}
