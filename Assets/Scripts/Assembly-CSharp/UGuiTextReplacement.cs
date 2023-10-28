using System.Collections.Generic;

public class UGuiTextReplacement
{
	private Dictionary<string, string> replacements = new Dictionary<string, string>();

	private static UGuiTextReplacement instance;

	public static UGuiTextReplacement Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new UGuiTextReplacement();
			}
			return instance;
		}
	}

	public void Set(string inputLocKey, string outputText)
	{
		replacements[inputLocKey] = outputText;
	}

	public string Get(string inputLocKey)
	{
		string value = null;
		if (replacements.TryGetValue(inputLocKey, out value))
		{
			return value;
		}
		return KFFLocalization.Get(inputLocKey);
	}

	public void Destroy()
	{
		instance = null;
	}
}
