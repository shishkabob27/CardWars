namespace JsonFx.Json
{
	public interface IDataWriterProvider
	{
		IDataWriter DefaultDataWriter { get; }

		IDataWriter Find(string extension);

		IDataWriter Find(string acceptHeader, string contentTypeHeader);
	}
}
