public class GachaKey
{
	public string Type { get; set; }

	public int Count { get; set; }

	public GachaKey(string t, int c)
	{
		Type = t;
		Count = c;
	}
}
