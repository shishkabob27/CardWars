using System.IO;

public class NgFile
{
	public static string PathSeparatorNormalize(string path)
	{
		char[] array = path.ToCharArray();
		for (int i = 0; i < path.Length; i++)
		{
			if (path[i] == '/' || path[i] == '\\')
			{
				array[i] = '/';
			}
		}
		path = new string(array);
		return path;
	}

	public static string CombinePath(string path1, string path2)
	{
		return PathSeparatorNormalize(Path.Combine(path1, path2));
	}

	public static string CombinePath(string path1, string path2, string path3)
	{
		return PathSeparatorNormalize(Path.Combine(Path.Combine(path1, path2), path3));
	}

	public static string GetSplit(string path, int nIndex)
	{
		if (nIndex < 0)
		{
			return path;
		}
		string[] array = path.Split('/', '\\');
		if (nIndex < array.Length)
		{
			return array[nIndex];
		}
		return string.Empty;
	}

	public static string GetFilename(string path)
	{
		int num = path.Length - 1;
		while (0 <= num)
		{
			if (path[num] == '/' || path[num] == '\\')
			{
				if (num == path.Length - 1)
				{
					return string.Empty;
				}
				return TrimFileExt(path.Substring(num + 1));
			}
			num--;
		}
		return TrimFileExt(path);
	}

	public static string GetFilenameExt(string path)
	{
		int num = path.Length - 1;
		while (0 <= num)
		{
			if (path[num] == '/' || path[num] == '\\')
			{
				if (num == path.Length - 1)
				{
					return string.Empty;
				}
				return path.Substring(num + 1);
			}
			num--;
		}
		return path;
	}

	public static string GetFileExt(string path)
	{
		int num = path.Length - 1;
		while (0 <= num)
		{
			if (path[num] == '.')
			{
				return path.Substring(num + 1);
			}
			num--;
		}
		return string.Empty;
	}

	public static string TrimFilenameExt(string path)
	{
		int num = path.Length - 1;
		while (0 <= num)
		{
			if (path[num] == '/' || path[num] == '\\')
			{
				return path.Substring(0, num);
			}
			num--;
		}
		return string.Empty;
	}

	public static string TrimFileExt(string filename)
	{
		int num = filename.Length - 1;
		while (0 <= num)
		{
			if (filename[num] == '.')
			{
				return filename.Substring(0, num);
			}
			num--;
		}
		return filename;
	}

	public static string TrimLastFolder(string path)
	{
		int num = path.Length - 1;
		while (0 <= num)
		{
			if ((path[num] == '/' || path[num] == '\\') && num != path.Length - 1)
			{
				return path.Substring(0, num);
			}
			num--;
		}
		return string.Empty;
	}

	public static string GetLastFolder(string path)
	{
		int num = path.Length - 1;
		while (0 <= num)
		{
			if ((path[num] == '/' || path[num] == '\\') && num != path.Length - 1)
			{
				if (path[path.Length - 1] == '/' || path[path.Length - 1] == '\\')
				{
					return path.Substring(num + 1, path.Length - num - 2);
				}
				return path.Substring(num + 1, path.Length - num - 1);
			}
			num--;
		}
		return path;
	}

	public static bool CompareExtName(string srcPath, string tarLowerExt, bool bCheckCase)
	{
		if (bCheckCase)
		{
			return GetFilenameExt(srcPath).ToLower() == tarLowerExt;
		}
		return GetFilenameExt(srcPath) == tarLowerExt;
	}
}
