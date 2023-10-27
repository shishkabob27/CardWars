using UnityEngine;
using Multiplayer;

public class AsyncData<T> : MonoBehaviour
{
	public class AsyncMP_Data
	{
		public T MP_Data;
		public ResponseFlag flag;
	}

	public AsyncMP_Data Asyncdata;
}
