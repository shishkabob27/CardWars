// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// SLOTResoureRequest
using UnityEngine;

public interface SLOTResoureRequest
{
	Object asset { get; }

	Coroutine asyncOp { get; }

	bool isDone { get; }
}
