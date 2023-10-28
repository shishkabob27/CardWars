// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// STDErrorRetry
using UnityEngine;

public class STDErrorRetry : MonoBehaviour
{
    public STDErrorDialog.OnClickedDelegate callback;

    public void OnClick()
    {
        if (callback != null)
        {
            callback();
        }
    }
}
