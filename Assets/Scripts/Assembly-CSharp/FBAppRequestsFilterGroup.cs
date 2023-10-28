using System.Collections.Generic;

public sealed class FBAppRequestsFilterGroup : Dictionary<string, object>
{
	public FBAppRequestsFilterGroup(string name, List<string> user_ids)
	{
		this["name"] = name;
		this["user_ids"] = user_ids;
	}
}
