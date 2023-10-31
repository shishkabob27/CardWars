using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LoginOptions : MonoBehaviour
{

    public UIButtonTween ShowTween;

    public UIButtonTween HideTween;

    public GameObject TempPasswordLabel;

    public GameObject OKButton;

    private UIInput mUserNameInput;

    private UIInput mPasswordInput;

    private BoxCollider mOKButtonCollider;

    [method: MethodImpl(32)]
    public static event Action<string, string> LoginDone;

    private void Start()
    {
        UIInput[] inputs = base.gameObject.GetComponentsInChildren<UIInput>();

        foreach (var item in inputs)
        {
            if (item.gameObject.name == "Username_Val")
            {
                mUserNameInput = item;
            }
            else if (item.gameObject.name == "Password_Val")
            {
                mPasswordInput = item;
            }
        }
        if (null != OKButton)
        {
            mOKButtonCollider = OKButton.GetComponent<BoxCollider>();
        }

        (transform.Find("Labels/Label_Header").gameObject.GetComponent<UILabel>()).text = "Register/Login";

    }

    private void Update()
    {
        TempPasswordLabel.GetComponent<Text>().text = mPasswordInput.text;
    }

    public void OnClick()
    {
    }

    public void OnCancel()
    {
        if (LoginOptions.LoginDone != null)
        {
            LoginOptions.LoginDone(string.Empty, string.Empty);
        }
    }

    public void OnSubmit()
    {
        UIInput componentInChildren = GetComponentInChildren<UIInput>();
        if (null != componentInChildren)
        {
            OnSubmit(componentInChildren.text);
        }
    }

    public void OnSubmit(string inputString)
    {
        if (LoginOptions.LoginDone != null)
        {
            LoginOptions.LoginDone(mUserNameInput.text, mPasswordInput.text);
        }
        if ((bool)HideTween)
        {
            HideTween.Play(true);
        }
    }
}
