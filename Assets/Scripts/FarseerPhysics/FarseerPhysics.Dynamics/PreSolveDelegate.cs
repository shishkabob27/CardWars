using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics
{
	public delegate void PreSolveDelegate(Contact contact, ref Manifold oldManifold);
}
