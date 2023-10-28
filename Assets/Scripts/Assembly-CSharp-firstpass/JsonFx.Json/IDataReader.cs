using System;
using System.IO;

namespace JsonFx.Json
{
	public interface IDataReader
	{
		string ContentType { get; }

		object Deserialize(TextReader input, Type data);
	}
}
