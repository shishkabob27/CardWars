using UnityEngine;

public class NgMaterial
{
	public static bool IsMaterialColor(Material mat)
	{
		string[] array = new string[3] { "_Color", "_TintColor", "_EmisColor" };
		if (mat != null)
		{
			string[] array2 = array;
			foreach (string propertyName in array2)
			{
				if (mat.HasProperty(propertyName))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static string GetMaterialColorName(Material mat)
	{
		string[] array = new string[3] { "_Color", "_TintColor", "_EmisColor" };
		if (mat != null)
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (mat.HasProperty(text))
				{
					return text;
				}
			}
		}
		return null;
	}

	public static Color GetMaterialColor(Material mat)
	{
		return GetMaterialColor(mat, Color.white);
	}

	public static Color GetMaterialColor(Material mat, Color defaultColor)
	{
		string[] array = new string[3] { "_Color", "_TintColor", "_EmisColor" };
		if (mat != null)
		{
			string[] array2 = array;
			foreach (string propertyName in array2)
			{
				if (mat.HasProperty(propertyName))
				{
					return mat.GetColor(propertyName);
				}
			}
		}
		return defaultColor;
	}

	public static void SetMaterialColor(Material mat, Color color)
	{
		string[] array = new string[3] { "_Color", "_TintColor", "_EmisColor" };
		if (!(mat != null))
		{
			return;
		}
		string[] array2 = array;
		foreach (string propertyName in array2)
		{
			if (mat.HasProperty(propertyName))
			{
				mat.SetColor(propertyName, color);
			}
		}
	}

	public static bool IsSameMaterial(Material mat1, Material mat2, bool bCheckAddress)
	{
		if (bCheckAddress && mat1 != mat2)
		{
			return false;
		}
		if (mat2 == null)
		{
			return false;
		}
		if (mat1.shader != mat2.shader)
		{
			return false;
		}
		if (mat1.mainTexture != mat2.mainTexture)
		{
			return false;
		}
		if (mat1.mainTextureOffset != mat2.mainTextureOffset)
		{
			return false;
		}
		if (mat1.mainTextureScale != mat2.mainTextureScale)
		{
			return false;
		}
		if (!IsSameColorProperty(mat1, mat2, "_Color"))
		{
			return false;
		}
		if (!IsSameColorProperty(mat1, mat2, "_TintColor"))
		{
			return false;
		}
		if (!IsSameColorProperty(mat1, mat2, "_EmisColor"))
		{
			return false;
		}
		if (!IsSameFloatProperty(mat1, mat2, "_InvFade"))
		{
			return false;
		}
		if (IsMaskTexture(mat1) != IsMaskTexture(mat2))
		{
			return false;
		}
		if (IsMaskTexture(mat1) && GetMaskTexture(mat1) != GetMaskTexture(mat2))
		{
			return false;
		}
		return true;
	}

	public static void CopyMaterialArgument(Material srcMat, Material tarMat)
	{
		tarMat.mainTexture = srcMat.mainTexture;
		tarMat.mainTextureOffset = srcMat.mainTextureOffset;
		tarMat.mainTextureScale = srcMat.mainTextureScale;
		if (IsMaskTexture(srcMat) && IsMaskTexture(tarMat))
		{
			SetMaskTexture(tarMat, GetMaskTexture(srcMat));
		}
		SetMaterialColor(tarMat, GetMaterialColor(srcMat, new Color(0.5f, 0.5f, 0.5f, 0.5f)));
	}

	public static bool IsSameColorProperty(Material mat1, Material mat2, string propertyName)
	{
		bool flag = mat1.HasProperty(propertyName);
		bool flag2 = mat2.HasProperty(propertyName);
		if (flag && flag2)
		{
			return mat1.GetColor(propertyName) == mat2.GetColor(propertyName);
		}
		return !flag && !flag2;
	}

	public static void CopyColorProperty(Material srcMat, Material tarMat, string propertyName)
	{
		bool flag = srcMat.HasProperty(propertyName);
		bool flag2 = tarMat.HasProperty(propertyName);
		if (flag && flag2)
		{
			tarMat.SetColor(propertyName, srcMat.GetColor(propertyName));
		}
	}

	public static bool IsSameFloatProperty(Material mat1, Material mat2, string propertyName)
	{
		bool flag = mat1.HasProperty(propertyName);
		bool flag2 = mat2.HasProperty(propertyName);
		if (flag && flag2)
		{
			return mat1.GetFloat(propertyName) == mat2.GetFloat(propertyName);
		}
		return !flag && !flag2;
	}

	public static Texture GetTexture(Material mat, bool bMask)
	{
		if (mat == null)
		{
			return null;
		}
		if (bMask)
		{
			if (IsMaskTexture(mat))
			{
				return mat.GetTexture("_Mask");
			}
			return null;
		}
		return mat.mainTexture;
	}

	public static void SetMaskTexture(Material mat, bool bMask, Texture newTexture)
	{
		if (!(mat == null))
		{
			if (bMask)
			{
				SetMaskTexture(mat, newTexture);
			}
			else
			{
				mat.mainTexture = newTexture;
			}
		}
	}

	public static bool IsMaskTexture(Material tarMat)
	{
		return tarMat.HasProperty("_Mask");
	}

	public static void SetMaskTexture(Material tarMat, Texture maskTex)
	{
		tarMat.SetTexture("_Mask", maskTex);
	}

	public static Texture GetMaskTexture(Material mat)
	{
		if (mat == null || !mat.HasProperty("_Mask"))
		{
			return null;
		}
		return mat.GetTexture("_Mask");
	}
}
