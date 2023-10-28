using System.Collections;

public interface ILoadable
{
	IEnumerator Load();

	void Destroy();
}
