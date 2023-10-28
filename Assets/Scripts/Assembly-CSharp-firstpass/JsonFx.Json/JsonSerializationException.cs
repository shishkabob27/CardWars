using System;
using System.Runtime.Serialization;

namespace JsonFx.Json
{
	public class JsonSerializationException : InvalidOperationException
	{
		public JsonSerializationException()
		{
		}

		public JsonSerializationException(string message)
			: base(message)
		{
		}

		public JsonSerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public JsonSerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
