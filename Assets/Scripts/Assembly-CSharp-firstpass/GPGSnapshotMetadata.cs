using Prime31;

public class GPGSnapshotMetadata
{
	public double lastModifiedTimestamp;

	public string description;

	public string name;

	public override string ToString()
	{
		return JsonFormatter.prettyPrint(Json.encode(this));
	}
}
