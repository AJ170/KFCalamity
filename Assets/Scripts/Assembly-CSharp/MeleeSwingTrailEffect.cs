using System.Collections;
using EventHelpers;
using UnityEngine;

public class MeleeSwingTrailEffect : EffectBaseListener
{
	public GameObject Trail;

	public float Delay;

	public float Duration = 1f;

	protected override void Start()
	{
		if (Trail != null)
		{
			base.Start();
			Trail.SetActive(false);
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			StartCoroutine(ActivateTrail());
			StartCoroutine(DeactivateTrail());
		}
	}

	private IEnumerator ActivateTrail()
	{
		if (Trail != null)
		{
			if (Delay > 0f)
			{
				yield return new WaitForSeconds(Delay);
			}
			Trail.SetActive(true);
		}
	}

	private IEnumerator DeactivateTrail()
	{
		if (Trail != null)
		{
			if (Delay > 0f || Duration > 0f)
			{
				yield return new WaitForSeconds(Delay + Duration);
			}
			Trail.SetActive(false);
		}
	}
}
