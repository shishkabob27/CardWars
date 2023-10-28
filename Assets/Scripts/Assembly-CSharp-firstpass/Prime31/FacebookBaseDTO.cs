namespace Prime31
{
	public class FacebookBaseDTO
	{
		public override string ToString()
		{
			return JsonFormatter.prettyPrint(Json.encode(this)) ?? string.Empty;
		}
	}
}
