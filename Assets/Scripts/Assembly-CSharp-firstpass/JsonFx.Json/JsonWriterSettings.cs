using System;

namespace JsonFx.Json
{
	public class JsonWriterSettings
	{
		private WriteDelegate<DateTime> dateTimeSerializer;

		private int maxDepth = 25;

		private string newLine = Environment.NewLine;

		private bool prettyPrint;

		private string tab = "\t";

		private string typeHintName;

		private bool useXmlSerializationAttributes;

		public virtual string TypeHintName
		{
			get
			{
				return typeHintName;
			}
			set
			{
				typeHintName = value;
			}
		}

		public virtual bool PrettyPrint
		{
			get
			{
				return prettyPrint;
			}
			set
			{
				prettyPrint = value;
			}
		}

		public virtual string Tab
		{
			get
			{
				return tab;
			}
			set
			{
				tab = value;
			}
		}

		public virtual string NewLine
		{
			get
			{
				return newLine;
			}
			set
			{
				newLine = value;
			}
		}

		public virtual int MaxDepth
		{
			get
			{
				return maxDepth;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("MaxDepth must be a positive integer as it controls the maximum nesting level of serialized objects.");
				}
				maxDepth = value;
			}
		}

		public virtual bool UseXmlSerializationAttributes
		{
			get
			{
				return useXmlSerializationAttributes;
			}
			set
			{
				useXmlSerializationAttributes = value;
			}
		}

		public virtual WriteDelegate<DateTime> DateTimeSerializer
		{
			get
			{
				return dateTimeSerializer;
			}
			set
			{
				dateTimeSerializer = value;
			}
		}
	}
}
