using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SphereCollisionAffectEffect : SphereCollisionBase
{
	public bool CanAffectFriendly = true;

	protected override void Start()
	{
		base.Start();
		friendlyFire = CanAffectFriendly;
	}

	protected override void Execute(TriggerBase e)
	{
		Vector3 position;
		if (e.Id == TriggerId.Impact)
		{
			Impact impact = (Impact)e;
			position = impact.Position;
		}
		else
		{
			position = base.transform.position;
		}
		Collider[] array = Physics.OverlapSphere(position, Radius, Layer);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (HitLayerMatch(collider.gameObject.layer) && IsColliderAccepted(collider))
			{
				effectSystem.TriggerInternal(new Affect(Name, collider.transform));
			}
		}
	}
}
