using System;
using Prime31;

public class GPGSnapshot
{
	private string data;

	public GPGSnapshotMetadata metadata;

	public bool hasDataAvailable
	{
		get
		{
			return data != null;
		}
	}

	public byte[] snapshotData
	{
		get
		{
			return (data == null) ? null : Convert.FromBase64String(data);
		}
	}

	public override string ToString()
	{
		return JsonFormatter.prettyPrint(Json.encode(this));
	}
}
