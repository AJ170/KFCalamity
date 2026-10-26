using EffectEvents;
using UnityEngine;

public class CrossbowBolt : PhysicalProjectile
{
	public float AttachDuration = 1f;

	private float counter = -1f;

	public override void Fire()
	{
		base.Fire();
		counter = -1f;
	}

	protected override void Update()
	{
		if (counter == -1f)
		{
			base.Update();
			return;
		}
		counter = Mathf.Max(counter - Time.deltaTime, 0f);
		if (counter == 0f)
		{
			DestroyProjectile();
		}
	}

	protected override void OnImpact(GameObject target)
	{
		if (!impacted)
		{
			if (target != null && target.GetComponent<Health>() != null)
			{
				DestroyProjectile();
			}
			else
			{
				counter = AttachDuration;
			}
			impacted = true;
		}
	}

	protected override void TriggerHitEvents(Transform hitTransform)
	{
		base.TriggerHitEvents(hitTransform);
		TriggerRemote(hitTransform.gameObject, new BeingHit(transform.root.gameObject, hitTransform, -transform.forward));
	}
}
