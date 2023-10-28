using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NgInterpolate
{
	public enum EaseType
	{
		Linear,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc
	}

	public delegate Vector3 ToVector3<T>(T v);

	public delegate float Function(float a, float b, float c, float d);

	private static Vector3 Identity(Vector3 v)
	{
		return v;
	}

	private static Vector3 TransformDotPosition(Transform t)
	{
		return t.position;
	}

	private static IEnumerable<float> NewTimer(float duration)
	{
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			yield return elapsedTime;
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= duration)
			{
				yield return elapsedTime;
			}
		}
	}

	private static IEnumerable<float> NewCounter(int start, int end, int step)
	{
		for (int i = start; i <= end; i += step)
		{
			yield return i;
		}
	}

	public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float duration)
	{
		IEnumerable<float> driver = NewTimer(duration);
		return NewEase(ease, start, end, duration, driver);
	}

	public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, int slices)
	{
		IEnumerable<float> driver = NewCounter(0, slices + 1, 1);
		return NewEase(ease, start, end, slices + 1, driver);
	}

	private static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver)
	{
		Vector3 distance = end - start;
		foreach (float item in driver)
		{
			float i = item;
			yield return Ease(ease, start, distance, i, total);
		}
	}

	private static Vector3 Ease(Function ease, Vector3 start, Vector3 distance, float elapsedTime, float duration)
	{
		start.x = ease(start.x, distance.x, elapsedTime, duration);
		start.y = ease(start.y, distance.y, elapsedTime, duration);
		start.z = ease(start.z, distance.z, elapsedTime, duration);
		return start;
	}

	public static Function Ease(EaseType type)
	{
		Function result = null;
		switch (type)
		{
		case EaseType.Linear:
			result = Linear;
			break;
		case EaseType.EaseInQuad:
			result = EaseInQuad;
			break;
		case EaseType.EaseOutQuad:
			result = EaseOutQuad;
			break;
		case EaseType.EaseInOutQuad:
			result = EaseInOutQuad;
			break;
		case EaseType.EaseInCubic:
			result = EaseInCubic;
			break;
		case EaseType.EaseOutCubic:
			result = EaseOutCubic;
			break;
		case EaseType.EaseInOutCubic:
			result = EaseInOutCubic;
			break;
		case EaseType.EaseInQuart:
			result = EaseInQuart;
			break;
		case EaseType.EaseOutQuart:
			result = EaseOutQuart;
			break;
		case EaseType.EaseInOutQuart:
			result = EaseInOutQuart;
			break;
		case EaseType.EaseInQuint:
			result = EaseInQuint;
			break;
		case EaseType.EaseOutQuint:
			result = EaseOutQuint;
			break;
		case EaseType.EaseInOutQuint:
			result = EaseInOutQuint;
			break;
		case EaseType.EaseInSine:
			result = EaseInSine;
			break;
		case EaseType.EaseOutSine:
			result = EaseOutSine;
			break;
		case EaseType.EaseInOutSine:
			result = EaseInOutSine;
			break;
		case EaseType.EaseInExpo:
			result = EaseInExpo;
			break;
		case EaseType.EaseOutExpo:
			result = EaseOutExpo;
			break;
		case EaseType.EaseInOutExpo:
			result = EaseInOutExpo;
			break;
		case EaseType.EaseInCirc:
			result = EaseInCirc;
			break;
		case EaseType.EaseOutCirc:
			result = EaseOutCirc;
			break;
		case EaseType.EaseInOutCirc:
			result = EaseInOutCirc;
			break;
		}
		return result;
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, float duration)
	{
		IEnumerable<float> steps = NewTimer(duration);
		return NewBezier<Transform>(ease, nodes, TransformDotPosition, duration, steps);
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, int slices)
	{
		IEnumerable<float> steps = NewCounter(0, slices + 1, 1);
		return NewBezier<Transform>(ease, nodes, TransformDotPosition, slices + 1, steps);
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, float duration)
	{
		IEnumerable<float> steps = NewTimer(duration);
		return NewBezier<Vector3>(ease, points, Identity, duration, steps);
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, int slices)
	{
		IEnumerable<float> steps = NewCounter(0, slices + 1, 1);
		return NewBezier<Vector3>(ease, points, Identity, slices + 1, steps);
	}

	private static IEnumerable<Vector3> NewBezier<T>(Function ease, IList nodes, ToVector3<T> toVector3, float maxStep, IEnumerable<float> steps)
	{
		if (nodes.Count < 2)
		{
			yield break;
		}
		Vector3[] points = new Vector3[nodes.Count];
		foreach (float step2 in steps)
		{
			float step = step2;
			for (int i = 0; i < nodes.Count; i++)
			{
				points[i] = toVector3((T)nodes[i]);
			}
			yield return Bezier(ease, points, step, maxStep);
		}
	}

	private static Vector3 Bezier(Function ease, Vector3[] points, float elapsedTime, float duration)
	{
		for (int num = points.Length - 1; num > 0; num--)
		{
			for (int i = 0; i < num; i++)
			{
				points[i].x = ease(points[i].x, points[i + 1].x - points[i].x, elapsedTime, duration);
				points[i].y = ease(points[i].y, points[i + 1].y - points[i].y, elapsedTime, duration);
				points[i].z = ease(points[i].z, points[i + 1].z - points[i].z, elapsedTime, duration);
			}
		}
		return points[0];
	}

	public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop)
	{
		return NewCatmullRom<Transform>(nodes, TransformDotPosition, slices, loop);
	}

	public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop)
	{
		return NewCatmullRom<Vector3>(points, Identity, slices, loop);
	}

	private static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, int slices, bool loop)
	{
		if (nodes.Count < 2)
		{
			yield break;
		}
		yield return toVector3((T)nodes[0]);
		int last = nodes.Count - 1;
		for (int current = 0; loop || current < last; current++)
		{
			if (loop && current > last)
			{
				current = 0;
			}
			int previous = ((current != 0) ? (current - 1) : ((!loop) ? current : last));
			int start = current;
			int end = ((current != last) ? (current + 1) : ((!loop) ? current : 0));
			int next = ((end != last) ? (end + 1) : ((!loop) ? end : 0));
			int stepCount = slices + 1;
			for (int step = 1; step <= stepCount; step++)
			{
				yield return CatmullRom(toVector3((T)nodes[previous]), toVector3((T)nodes[start]), toVector3((T)nodes[end]), toVector3((T)nodes[next]), step, stepCount);
			}
		}
	}

	private static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime, float duration)
	{
		float num = elapsedTime / duration;
		float num2 = num * num;
		float num3 = num2 * num;
		return previous * (-0.5f * num3 + num2 - 0.5f * num) + start * (1.5f * num3 + -2.5f * num2 + 1f) + end * (-1.5f * num3 + 2f * num2 + 0.5f * num) + next * (0.5f * num3 - 0.5f * num2);
	}

	private static float Linear(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * (elapsedTime / duration) + start;
	}

	private static float EaseInQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		return (0f - distance) * elapsedTime * (elapsedTime - 2f) + start;
	}

	private static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 1f;
		return (0f - distance) / 2f * (elapsedTime * (elapsedTime - 2f) - 1f) + start;
	}

	private static float EaseInCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (elapsedTime * elapsedTime * elapsedTime + 2f) + start;
	}

	private static float EaseInQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return (0f - distance) * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1f) + start;
	}

	private static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return (0f - distance) / 2f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2f) + start;
	}

	private static float EaseInQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2f) + start;
	}

	private static float EaseInSine(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return (0f - distance) * Mathf.Cos(elapsedTime / duration * ((float)Math.PI / 2f)) + distance + start;
	}

	private static float EaseOutSine(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * Mathf.Sin(elapsedTime / duration * ((float)Math.PI / 2f)) + start;
	}

	private static float EaseInOutSine(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return (0f - distance) / 2f * (Mathf.Cos((float)Math.PI * elapsedTime / duration) - 1f) + start;
	}

	private static float EaseInExpo(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * Mathf.Pow(2f, 10f * (elapsedTime / duration - 1f)) + start;
	}

	private static float EaseOutExpo(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * (0f - Mathf.Pow(2f, -10f * elapsedTime / duration) + 1f) + start;
	}

	private static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * Mathf.Pow(2f, 10f * (elapsedTime - 1f)) + start;
		}
		elapsedTime -= 1f;
		return distance / 2f * (0f - Mathf.Pow(2f, -10f * elapsedTime) + 2f) + start;
	}

	private static float EaseInCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		return (0f - distance) * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
	}

	private static float EaseOutCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * Mathf.Sqrt(1f - elapsedTime * elapsedTime) + start;
	}

	private static float EaseInOutCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((!(elapsedTime > duration)) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return (0f - distance) / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) + 1f) + start;
	}
}
