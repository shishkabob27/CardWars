// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// ILoadable
using System.Collections;

public interface ILoadable
{
    IEnumerator Load();

    void Destroy();
}
