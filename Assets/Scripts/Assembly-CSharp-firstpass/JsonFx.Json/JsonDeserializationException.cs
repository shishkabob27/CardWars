using System;
using System.Runtime.Serialization;

namespace JsonFx.Json
{
	public class JsonDeserializationException : JsonSerializationException
	{
		private int index = -1;

		public int Index
		{
			get
			{
				return index;
			}
		}

		public JsonDeserializationException(string message, int index)
			: base(message)
		{
			this.index = index;
		}

		public JsonDeserializationException(string message, Exception innerException, int index)
			: base(message, innerException)
		{
			this.index = index;
		}

		public JsonDeserializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public void GetLineAndColumn(string source, out int line, out int col)
		{
			if (source == null)
			{
				throw new ArgumentNullException();
			}
			col = 1;
			line = 1;
			bool flag = false;
			for (int num = Math.Min(index, source.Length); num > 0; num--)
			{
				if (!flag)
				{
					col++;
				}
				if (source[num - 1] == '\n')
				{
					line++;
					flag = true;
				}
			}
		}
	}
}
