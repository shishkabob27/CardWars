using System.Collections.Generic;

namespace Prime31
{
	public class JsonObject : Dictionary<string, object>
	{
		public override string ToString()
		{
			return JsonFormatter.prettyPrint(SimpleJson.encode(this)) ?? string.Empty;
		}
	}
}
