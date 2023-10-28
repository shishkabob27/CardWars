namespace Facebook
{
	public class HttpMethod
	{
		private static HttpMethod getMethod = new HttpMethod("GET");

		private static HttpMethod postMethod = new HttpMethod("POST");

		private static HttpMethod deleteMethod = new HttpMethod("DELETE");

		private string methodValue;

		public static HttpMethod GET
		{
			get
			{
				return getMethod;
			}
		}

		public static HttpMethod POST
		{
			get
			{
				return postMethod;
			}
		}

		public static HttpMethod DELETE
		{
			get
			{
				return deleteMethod;
			}
		}

		private HttpMethod(string value)
		{
			methodValue = value;
		}

		public override string ToString()
		{
			return methodValue;
		}
	}
}
