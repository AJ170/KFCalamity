using UnityEngine;

public class TriggerAnimationOnStart : MonoBehaviour
{
	public string TriggerName;

	public Animator Animator;

	private void Start()
	{
		if (Animator != null && !string.IsNullOrEmpty(TriggerName))
		{
			Animator.SetBool(TriggerName, true);
		}
	}

	private void Update()
	{
		if (Animator != null && !string.IsNullOrEmpty(TriggerName) && Animator.IsInTransition(0))
		{
			Animator.SetBool(TriggerName, false);
			base.enabled = false;
		}
	}
}
