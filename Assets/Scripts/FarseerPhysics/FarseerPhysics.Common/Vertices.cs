using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FarseerPhysics.Common
{
	public class Vertices : List<Vector2>
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < base.Count; i++)
			{
				stringBuilder.Append(base[i].ToString());
				if (i < base.Count - 1)
				{
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
