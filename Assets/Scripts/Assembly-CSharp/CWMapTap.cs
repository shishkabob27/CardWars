using System.Collections.Generic;
using UnityEngine;

public class CWMapTap : MonoBehaviour
{
	public UIButtonTween ShowLockedErrorButtonTween;

	private List<CWMapQuestInfoSet> lockedQuests;

	private Collider cameraCollider;

	private Camera questMapUICamera;

	private bool shouldClearListOnError;

	private void Start()
	{
		CWCameraCollider cWCameraCollider = Object.FindObjectOfType(typeof(CWCameraCollider)) as CWCameraCollider;
		if (cWCameraCollider != null)
		{
			cameraCollider = cWCameraCollider.GetComponent<Collider>();
		}
		questMapUICamera = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("GUI_QuestMap"));
	}

	private void OnTap(TapGesture gesture)
	{
		if (questMapUICamera == null || !questMapUICamera.enabled || !FindLockedQuests())
		{
			return;
		}
		bool flag = false;
		if (cameraCollider != null)
		{
			flag = cameraCollider.enabled;
			cameraCollider.enabled = false;
		}
		int num = LayerMask.NameToLayer("GUI_QuestMap");
		Camera camera = NGUITools.FindCameraForLayer(num);
		Vector3 position = gesture.Position;
		position.z = camera.nearClipPlane;
		Ray ray = camera.ScreenPointToRay(position);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1 << num))
		{
			CWMapQuestInfoSet cWMapQuestInfoSet = null;
			for (int num2 = lockedQuests.Count - 1; num2 >= 0; num2--)
			{
				CWMapQuestInfoSet cWMapQuestInfoSet2 = lockedQuests[num2];
				if (cWMapQuestInfoSet2 == null)
				{
					lockedQuests.RemoveAt(num2);
				}
				else
				{
					Collider component = cWMapQuestInfoSet2.GetComponent<Collider>();
					if (component == null || component.enabled)
					{
						lockedQuests.RemoveAt(num2);
					}
					else
					{
						component.enabled = true;
					}
				}
			}
			camera = GetComponent<Camera>();
			position = gesture.Position;
			position.z = camera.nearClipPlane;
			ray = camera.ScreenPointToRay(position);
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1 << base.gameObject.layer) && hitInfo.collider != null)
			{
				CWMapQuestInfoSet cWMapQuestInfoSet3 = hitInfo.collider.gameObject.GetComponent(typeof(CWMapQuestInfoSet)) as CWMapQuestInfoSet;
				if (cWMapQuestInfoSet3 != null)
				{
					cWMapQuestInfoSet = cWMapQuestInfoSet3;
				}
			}
			foreach (CWMapQuestInfoSet lockedQuest in lockedQuests)
			{
				if (lockedQuest != null && lockedQuest.GetComponent<Collider>() != null)
				{
					lockedQuest.GetComponent<Collider>().enabled = false;
				}
			}
			if (cWMapQuestInfoSet != null)
			{
				Collider component2 = cWMapQuestInfoSet.GetComponent<Collider>();
				if (component2 != null && !component2.enabled && ShowLockedErrorButtonTween != null)
				{
					ShowLockedErrorButtonTween.Play(true);
				}
			}
		}
		if (cameraCollider != null)
		{
			cameraCollider.enabled = flag;
		}
	}

	public void InvalidateQuests()
	{
		lockedQuests = null;
	}

	private bool FindLockedQuests()
	{
		if (lockedQuests != null)
		{
			foreach (CWMapQuestInfoSet lockedQuest in lockedQuests)
			{
				if (lockedQuest == null)
				{
					lockedQuests = null;
					break;
				}
			}
		}
		if (lockedQuests == null)
		{
			lockedQuests = new List<CWMapQuestInfoSet>();
			Object[] array = Object.FindObjectsOfType(typeof(CWMapQuestInfoSet));
			if (array != null)
			{
				Object[] array2 = array;
				foreach (Object @object in array2)
				{
					CWMapQuestInfoSet cWMapQuestInfoSet = @object as CWMapQuestInfoSet;
					if (cWMapQuestInfoSet != null)
					{
						Collider component = cWMapQuestInfoSet.GetComponent<Collider>();
						if (component != null && !component.enabled)
						{
							lockedQuests.Add(cWMapQuestInfoSet);
						}
					}
				}
			}
		}
		return lockedQuests.Count != 0;
	}
}
