using FarseerPhysics.Common;
using UnityEngine;

namespace FarseerPhysics
{
	public static class VectorExtensionMethods
	{
		public static Transform2D To2D(this Transform tf)
		{
			Transform2D result = default(Transform2D);
			result.Set(tf.position, tf.rotation.eulerAngles.z);
			return result;
		}
	}
}
