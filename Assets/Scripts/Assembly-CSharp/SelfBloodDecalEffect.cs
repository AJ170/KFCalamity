using EventHelpers;
using UnityEngine;

public class SelfBloodDecalEffect : EffectBaseListener
{
	public GameObject[] BloodDecals;

	public bool DemandClearPosition = true;

	private BloodDecalManager bloodDecalManager;

	private LayerMask Layer;

	protected override void Awake()
	{
		base.Awake();
		bloodDecalManager = BloodDecalManager.Instance;
		Layer = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes semi-solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid (no Line of sight)"));
	}

	protected override void Execute(TriggerBase e)
	{
		if ((!NoCustomEvents() && !MatchCustomEvent(e.CustomName)) || !AnyDecals())
		{
			return;
		}
		Vector3 position = base.transform.position;
		Quaternion identity = Quaternion.identity;
		if (DemandClearPosition && bloodDecalManager != null && !bloodDecalManager.IsPositionClear(position))
		{
			return;
		}
		RaycastHit raycastHit = SimpleDecalHelper.FindAcceptedLocation(position, Layer);
		if (!(raycastHit.transform != null))
		{
			return;
		}
		GameObject gameObject = RandomBloodDecal();
		GameObject effect = EffectPool.GetEffect(gameObject);
		if (effect != null)
		{
			position = raycastHit.point + new Vector3(0f, 0.01f, 0f);
			identity = Quaternion.FromToRotation(gameObject.transform.up, raycastHit.normal);
			if (bloodDecalManager != null)
			{
				bloodDecalManager.RegisterBloodDecal(effect);
			}
			Transform transform = effect.transform;
			effect.SetActive(true);
			transform.position = position;
			transform.rotation = identity;
			transform.up *= -1f;
		}
	}

	private bool AnyDecals()
	{
		return BloodDecals != null && BloodDecals.Length > 0;
	}

	private GameObject RandomBloodDecal()
	{
		if (AnyDecals())
		{
			return BloodDecals[Random.Range(0, BloodDecals.Length)];
		}
		return null;
	}
}
