using System;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using UnityEngine;

public static class FarseerGizmos
{
	private static Vector3[] _circle16;

	private static Vector3[] Circle16
	{
		get
		{
			if (_circle16 == null)
			{
				_circle16 = new Vector3[16];
				for (int i = 0; i < _circle16.Length; i++)
				{
					float num = (float)i / (float)_circle16.Length;
					float f = num * (float)Math.PI * 2f;
					_circle16[i] = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
				}
			}
			return _circle16;
		}
	}

	public static bool GizmosActive
	{
		get
		{
			return false;
		}
	}

	public static void DrawBody(Body body, Color color)
	{
		if (body != null && body.FixtureList != null)
		{
			List<Fixture> fixtureList = body.FixtureList;
			int count = fixtureList.Count;
			Transform2D transform = default(Transform2D);
			body.GetTransform(out transform);
			for (int i = 0; i < count; i++)
			{
				DrawShape(fixtureList[i], transform, color);
			}
		}
	}

	public static void DrawJoint(FarseerPhysics.Dynamics.Joints.Joint2D joint)
	{
		if (joint.Enabled)
		{
			Body bodyA = joint.BodyA;
			Body bodyB = joint.BodyB;
			Transform2D transform;
			bodyA.GetTransform(out transform);
			Vector2 vector = Vector2.zero;
			if (!joint.IsFixedType())
			{
				Transform2D transform2;
				bodyB.GetTransform(out transform2);
				vector = transform2.p;
			}
			Vector2 worldAnchorB = joint.WorldAnchorB;
			Vector2 p = transform.p;
			Vector2 worldAnchorA = joint.WorldAnchorA;
			Color color = new Color32(128, 205, 205, byte.MaxValue);
			switch (joint.JointType)
			{
			case JointType.Distance:
				DrawSegment(worldAnchorA, worldAnchorB, color);
				break;
			case JointType.Pulley:
			{
				PulleyJoint pulleyJoint = (PulleyJoint)joint;
				Vector2 groundAnchorA = pulleyJoint.GroundAnchorA;
				Vector2 groundAnchorB = pulleyJoint.GroundAnchorB;
				DrawSegment(groundAnchorA, worldAnchorA, color);
				DrawSegment(groundAnchorB, worldAnchorB, color);
				DrawSegment(groundAnchorA, groundAnchorB, color);
				break;
			}
			case JointType.FixedMouse:
				DrawPoint(worldAnchorA, 0.5f, new Color32(0, byte.MaxValue, 0, byte.MaxValue));
				DrawSegment(worldAnchorA, worldAnchorB, new Color32(205, 205, 205, byte.MaxValue));
				break;
			case JointType.Revolute:
				DrawSegment(worldAnchorB, worldAnchorA, color);
				DrawSolidCircle(worldAnchorB, 0.1f, Vector2.zero, Color.red);
				DrawSolidCircle(worldAnchorA, 0.1f, Vector2.zero, Color.blue);
				break;
			case JointType.FixedAngle:
				break;
			case JointType.FixedRevolute:
				DrawSegment(p, worldAnchorA, color);
				DrawSolidCircle(worldAnchorA, 0.1f, Vector2.zero, Color.magenta);
				break;
			case JointType.FixedLine:
				DrawSegment(p, worldAnchorA, color);
				DrawSegment(worldAnchorA, worldAnchorB, color);
				break;
			case JointType.FixedDistance:
				DrawSegment(p, worldAnchorA, color);
				DrawSegment(worldAnchorA, worldAnchorB, color);
				break;
			case JointType.FixedPrismatic:
				DrawSegment(p, worldAnchorA, color);
				DrawSegment(worldAnchorA, worldAnchorB, color);
				break;
			case JointType.Gear:
				DrawSegment(p, vector, color);
				break;
			default:
				DrawSegment(p, worldAnchorA, color);
				DrawSegment(worldAnchorA, worldAnchorB, color);
				DrawSegment(vector, worldAnchorB, color);
				break;
			}
		}
	}

	public static void DrawShape(Fixture fixture, Transform2D xf, Color color)
	{
		switch (fixture.ShapeType)
		{
		case ShapeType.Circle:
		{
			CircleShape circleShape = (CircleShape)fixture.Shape;
			Vector2 center = MathUtils.Mul(ref xf, circleShape.Position);
			float radius = circleShape.Radius;
			Vector2 axis = MathUtils.Mul(xf.q, new Vector2(1f, 0f));
			DrawSolidCircle(center, radius, axis, color);
			break;
		}
		case ShapeType.Polygon:
		{
			PolygonShape polygonShape = (PolygonShape)fixture.Shape;
			int count2 = polygonShape.Vertices.Count;
			if (count2 <= Settings.MaxPolygonVertices)
			{
				Vector2[] array = new Vector2[Settings.MaxPolygonVertices];
				for (int j = 0; j < count2; j++)
				{
					array[j] = MathUtils.Mul(ref xf, polygonShape.Vertices[j]);
				}
				DrawSolidPolygon(array, count2, color);
			}
			break;
		}
		case ShapeType.Edge:
		{
			EdgeShape edgeShape = (EdgeShape)fixture.Shape;
			Vector2 start = MathUtils.Mul(ref xf, edgeShape.Vertex1);
			Vector2 end = MathUtils.Mul(ref xf, edgeShape.Vertex2);
			DrawSegment(start, end, color);
			break;
		}
		case ShapeType.Chain:
		{
			ChainShape chainShape = (ChainShape)fixture.Shape;
			int count = chainShape.Vertices.Count;
			Vector2 vector = MathUtils.Mul(ref xf, chainShape.Vertices[count - 1]);
			DrawCircle(vector, 0.05f, color);
			for (int i = 0; i < count; i++)
			{
				Vector2 vector2 = MathUtils.Mul(ref xf, chainShape.Vertices[i]);
				DrawSegment(vector, vector2, color);
				vector = vector2;
			}
			break;
		}
		}
	}

	private static void DrawPolygon(Vector2[] vertices, int count, float red, float green, float blue)
	{
		DrawPolygon(vertices, count, new Color(red, green, blue));
	}

	public static void DrawPolygon(Vector2[] vertices, int count, Color color)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		for (int i = 1; i < count; i++)
		{
			Gizmos.DrawLine(vertices[i - 1], vertices[i]);
		}
		Gizmos.DrawLine(vertices[count - 1], vertices[0]);
		Gizmos.color = color2;
	}

	private static void DrawSolidPolygon(Vector2[] vertices, int count, float red, float green, float blue)
	{
		DrawSolidPolygon(vertices, count, new Color(red, green, blue), true);
	}

	private static void DrawSolidPolygon(Vector2[] vertices, int count, Color color)
	{
		DrawSolidPolygon(vertices, count, color, true);
	}

	public static void DrawSolidPolygon(Vector2[] vertices, int count, Color color, bool outline)
	{
		DrawPolygon(vertices, count, color.r, color.g, color.b);
	}

	private static void DrawCircle(Vector2 center, float radius, float red, float green, float blue)
	{
		DrawCircle(center, radius, new Color(red, green, blue));
	}

	public static void DrawCircle(Vector2 center, float radius, Color color)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Vector3[] array = new Vector3[16];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = radius * Circle16[i] + (Vector3)center;
		}
		for (int j = 1; j < array.Length; j++)
		{
			Gizmos.DrawLine(array[j], array[j - 1]);
		}
		Gizmos.DrawLine(array[0], array[array.Length - 1]);
		Gizmos.color = color2;
	}

	private static void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float green, float blue)
	{
		DrawSolidCircle(center, radius, axis, new Color(red, green, blue));
	}

	public static void DrawTransform(ref Transform2D transform)
	{
		Vector2 p = transform.p;
		Vector2 end = p + 0.4f * transform.q.GetXAxis();
		DrawSegment(p, end, Color.red);
		end = p + 0.4f * transform.q.GetYAxis();
		DrawSegment(p, end, Color.green);
	}

	public static void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
	{
		DrawCircle(center, radius, color);
		DrawSegment(center, center + axis * radius, color);
	}

	public static void DrawAABB(ref AABB aabb, Color color)
	{
		DrawPolygon(new Vector2[4]
		{
			new Vector2(aabb.LowerBound.x, aabb.LowerBound.y),
			new Vector2(aabb.UpperBound.x, aabb.LowerBound.y),
			new Vector2(aabb.UpperBound.x, aabb.UpperBound.y),
			new Vector2(aabb.LowerBound.x, aabb.UpperBound.y)
		}, 4, color);
	}

	private static void DrawSegment(Vector2 start, Vector2 end, float red, float green, float blue)
	{
		DrawSegment(start, end, new Color(red, green, blue));
	}

	public static void DrawSegment(Vector2 start, Vector2 end, Color color)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(start, end);
		Gizmos.color = color2;
	}

	public static void DrawPoint(Vector2 p, float size, Color color)
	{
		Vector2[] array = new Vector2[4];
		float num = size / 2f;
		array[0] = p + new Vector2(0f - num, 0f - num);
		array[1] = p + new Vector2(num, 0f - num);
		array[2] = p + new Vector2(num, num);
		array[3] = p + new Vector2(0f - num, num);
		DrawSolidPolygon(array, 4, color, true);
	}
}
