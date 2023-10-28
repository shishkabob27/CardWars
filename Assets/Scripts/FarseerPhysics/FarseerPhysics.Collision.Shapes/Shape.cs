namespace FarseerPhysics.Collision.Shapes
{
	public abstract class Shape
	{
		private static int _shapeIdCounter;

		public MassData MassData;

		public int ShapeId;

		internal float _density;

		internal float _radius;

		public ShapeType ShapeType { get; internal set; }

		public float Radius
		{
			get
			{
				return _radius;
			}
		}

		protected Shape(float density)
		{
			_density = density;
			ShapeType = ShapeType.Unknown;
			ShapeId = _shapeIdCounter++;
		}
	}
}
