using UnityEngine;

public interface SLOTResoureRequest
{
	Object asset { get; }

	Coroutine asyncOp { get; }

	bool isDone { get; }
}
