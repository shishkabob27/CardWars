namespace JsonFx.Json
{
	internal enum JsonToken
	{
		End = 0,
		Undefined = 1,
		Null = 2,
		False = 3,
		True = 4,
		NaN = 5,
		PositiveInfinity = 6,
		NegativeInfinity = 7,
		Number = 8,
		String = 9,
		ArrayStart = 10,
		ArrayEnd = 11,
		ObjectStart = 12,
		ObjectEnd = 13,
		NameDelim = 14,
		ValueDelim = 15,
		UnquotedName = 16,
	}
}
