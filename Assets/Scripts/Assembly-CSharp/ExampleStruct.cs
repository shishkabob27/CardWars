using System;
using UnityEngine;

[Serializable]
public struct ExampleStruct
{
	public int valueInt;

	[Range(0f, 1f)]
	public float valueFloat;

	public string valueString;

	public GameObject valueGameObject;

	public Vector3 valueVectr;

	public Quaternion valueQuaternion;

	public Rect valueRect;

	public AnimationCurve valueCurve;

	public int[] valueIntArray;
}
