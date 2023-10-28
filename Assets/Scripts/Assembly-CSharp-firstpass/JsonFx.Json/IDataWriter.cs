using System.IO;
using System.Text;

namespace JsonFx.Json
{
	public interface IDataWriter
	{
		Encoding ContentEncoding { get; }

		string ContentType { get; }

		string FileExtension { get; }

		void Serialize(TextWriter output, object data);
	}
}
