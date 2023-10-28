using UnityEngine;

public class ShowAvatarScript : MonoBehaviour
{
	private void Start()
	{
		UISprite[] componentsInChildren = base.gameObject.GetComponentsInChildren<UISprite>();
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			uISprite.spriteName = PlayerInfoScript.GetInstance().Avatar;
		}
	}
}
