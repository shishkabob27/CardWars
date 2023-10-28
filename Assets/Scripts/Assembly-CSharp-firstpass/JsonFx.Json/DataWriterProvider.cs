using System;
using System.Collections.Generic;
using System.IO;

namespace JsonFx.Json
{
	public class DataWriterProvider : IDataWriterProvider
	{
		private readonly IDataWriter DefaultWriter;

		private readonly IDictionary<string, IDataWriter> WritersByExt = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);

		private readonly IDictionary<string, IDataWriter> WritersByMime = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);

		public IDataWriter DefaultDataWriter
		{
			get
			{
				return DefaultWriter;
			}
		}

		public DataWriterProvider(IEnumerable<IDataWriter> writers)
		{
			if (writers == null)
			{
				return;
			}
			foreach (IDataWriter writer in writers)
			{
				if (DefaultWriter == null)
				{
					DefaultWriter = writer;
				}
				if (!string.IsNullOrEmpty(writer.ContentType))
				{
					WritersByMime[writer.ContentType] = writer;
				}
				if (!string.IsNullOrEmpty(writer.ContentType))
				{
					string key = NormalizeExtension(writer.FileExtension);
					WritersByExt[key] = writer;
				}
			}
		}

		public IDataWriter Find(string extension)
		{
			extension = NormalizeExtension(extension);
			if (WritersByExt.ContainsKey(extension))
			{
				return WritersByExt[extension];
			}
			return null;
		}

		public IDataWriter Find(string acceptHeader, string contentTypeHeader)
		{
			foreach (string item in ParseHeaders(acceptHeader, contentTypeHeader))
			{
				if (WritersByMime.ContainsKey(item))
				{
					return WritersByMime[item];
				}
			}
			return null;
		}

		public static IEnumerable<string> ParseHeaders(string accept, string contentType)
		{
			string mime2;
			foreach (string type in SplitTrim(accept, ','))
			{
				mime2 = ParseMediaType(type);
				if (!string.IsNullOrEmpty(mime2))
				{
					yield return mime2;
				}
			}
			mime2 = ParseMediaType(contentType);
			if (!string.IsNullOrEmpty(mime2))
			{
				yield return mime2;
			}
		}

		public static string ParseMediaType(string type)
		{
			using (IEnumerator<string> enumerator = SplitTrim(type, ';').GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return string.Empty;
		}

		private static IEnumerable<string> SplitTrim(string source, char ch)
		{
			if (string.IsNullOrEmpty(source))
			{
				yield break;
			}
			int length = source.Length;
			int prev = 0;
			int next = 0;
			while (prev < length && next >= 0)
			{
				next = source.IndexOf(ch, prev);
				if (next < 0)
				{
					next = length;
				}
				string part = source.Substring(prev, next - prev).Trim();
				if (part.Length > 0)
				{
					yield return part;
				}
				prev = next + 1;
			}
		}

		private static string NormalizeExtension(string extension)
		{
			if (string.IsNullOrEmpty(extension))
			{
				return string.Empty;
			}
			return Path.GetExtension(extension);
		}
	}
}
