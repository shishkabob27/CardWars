using System.Collections.Generic;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics
{
	public class ContactManager
	{
		public BeginContactDelegate BeginContact;

		public IBroadPhase BroadPhase;

		public CollisionFilterDelegate ContactFilter;

		public List<Contact> ContactList;

		public HashSet<Contact> ActiveContacts;

		private List<Contact> ActiveList;

		public EndContactDelegate EndContact;

		public BroadphaseDelegate OnBroadphaseCollision;

		public PostSolveDelegate PostSolve;

		public PreSolveDelegate PreSolve;

		internal void Destroy(Contact contact)
		{
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			Body body = fixtureA.Body;
			Body body2 = fixtureB.Body;
			if (EndContact != null && contact.IsTouching())
			{
				EndContact(contact);
			}
			ContactList.Remove(contact);
			if (contact.NodeA.Prev != null)
			{
				contact.NodeA.Prev.Next = contact.NodeA.Next;
			}
			if (contact.NodeA.Next != null)
			{
				contact.NodeA.Next.Prev = contact.NodeA.Prev;
			}
			if (contact.NodeA == body.ContactList)
			{
				body.ContactList = contact.NodeA.Next;
			}
			if (contact.NodeB.Prev != null)
			{
				contact.NodeB.Prev.Next = contact.NodeB.Next;
			}
			if (contact.NodeB.Next != null)
			{
				contact.NodeB.Next.Prev = contact.NodeB.Prev;
			}
			if (contact.NodeB == body2.ContactList)
			{
				body2.ContactList = contact.NodeB.Next;
			}
			if (ActiveContacts.Contains(contact))
			{
				ActiveContacts.Remove(contact);
			}
			contact.Destroy();
		}

		internal void RemoveActiveContact(Contact contact)
		{
			if (ActiveContacts.Contains(contact))
			{
				ActiveContacts.Remove(contact);
			}
		}
	}
}
