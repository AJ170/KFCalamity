using UnityEngine;

public class TutorialChangeTags : MonoBehaviour
{
	public string NewTag = string.Empty;

	private void Awake()
	{
		SetTagOnChildren(base.transform);
	}

	private void SetTagOnChildren(Transform objectTransform)
	{
		objectTransform.tag = NewTag;
		foreach (Transform item in objectTransform)
		{
			SetTagOnChildren(item);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
