using FarseerPhysics.Common;

namespace FarseerPhysics.Collision.Shapes
{
	public class PolygonShape : Shape
	{
		public Vertices Normals;

		public Vertices Vertices;

        public PolygonShape(float density) : base(density)
        {
        }
    }
}
