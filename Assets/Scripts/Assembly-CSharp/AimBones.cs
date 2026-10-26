using UnityEngine;

public class AimBones : MonoBehaviour
{
	public Transform Body;

	public Transform Head;

	public Transform FeedbackMount;

	private void Start()
	{
		if (FeedbackMount == null)
		{
			if (Head != null)
			{
				FeedbackMount = Head;
			}
			else if (Body != null)
			{
				FeedbackMount = Body;
			}
			else
			{
				FeedbackMount = base.transform;
			}
		}
	}

	private void OnDestroy()
	{
		Body = null;
		Head = null;
		FeedbackMount = null;
	}
}
