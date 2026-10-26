using UnityEngine;

public class SwooshTest : MonoBehaviour
{
	private Animator animator;

	private GameObject trail;

	private void Start()
	{
		trail = GameObject.Find("MeleeTrail");
		trail.SetActive(false);
		if (!trail)
		{
			Debug.LogError("taraiail");
		}
	}

	private void OnMounted(GameObject parent)
	{
		animator = parent.GetComponent<AnimationController>().animator;
	}

	private void Update()
	{
		if (!animator)
		{
			return;
		}
		if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack") || animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack1") || animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack2") || animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack3") || animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack4"))
		{
			if (!trail.activeSelf)
			{
				trail.SetActive(true);
			}
		}
		else if (trail.activeSelf)
		{
			trail.SetActive(false);
		}
	}
}
