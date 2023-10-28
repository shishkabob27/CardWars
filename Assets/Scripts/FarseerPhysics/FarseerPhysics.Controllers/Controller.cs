using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers
{
	public abstract class Controller : FilterData
	{
		public bool Enabled;

		public World World;

		private ControllerType _type;
	}
}
