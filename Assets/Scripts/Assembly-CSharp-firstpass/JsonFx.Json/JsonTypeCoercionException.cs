using System;
using System.Runtime.Serialization;

namespace JsonFx.Json
{
	public class JsonTypeCoercionException : ArgumentException
	{
		public JsonTypeCoercionException()
		{
		}

		public JsonTypeCoercionException(string message)
			: base(message)
		{
		}

		public JsonTypeCoercionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public JsonTypeCoercionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
