using System;
using System.Collections.Generic;

namespace JsonFx.Json
{
	public class DataReaderProvider : IDataReaderProvider
	{
		private readonly IDictionary<string, IDataReader> ReadersByMime = new Dictionary<string, IDataReader>(StringComparer.OrdinalIgnoreCase);

		public DataReaderProvider(IEnumerable<IDataReader> readers)
		{
			if (readers == null)
			{
				return;
			}
			foreach (IDataReader reader in readers)
			{
				if (!string.IsNullOrEmpty(reader.ContentType))
				{
					ReadersByMime[reader.ContentType] = reader;
				}
			}
		}

		public IDataReader Find(string contentTypeHeader)
		{
			string key = DataWriterProvider.ParseMediaType(contentTypeHeader);
			if (ReadersByMime.ContainsKey(key))
			{
				return ReadersByMime[key];
			}
			return null;
		}
	}
}
