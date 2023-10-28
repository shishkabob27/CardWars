public class KFFServerRequestResult : KFFDictionary
{
	public bool isValid()
	{
		if (entries.Count < 2)
		{
			return false;
		}
		KFFDictionaryEntry kFFDictionaryEntry = entries[0];
		if (kFFDictionaryEntry.key != "ERROR_ID")
		{
			return false;
		}
		int num = -1234567980;
		string value = kFFDictionaryEntry.value;
		if (value != null)
		{
			num = int.Parse(value);
		}
		if (num != 0)
		{
			return false;
		}
		kFFDictionaryEntry = entries[1];
		if (kFFDictionaryEntry.key != "ERROR_MSG")
		{
			return false;
		}
		return true;
	}
}
