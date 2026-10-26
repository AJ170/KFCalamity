using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SimpleDecalEffect : EffectBaseListener
{
	public GameObject SimpleDecalPrefab;

	public Vector3 Rotation;

	public Vector3 Offset;

	public LayerMask Layer;

	public float RayStartHeight = 0.1f;

	public float RayLength = 0.5f;

	protected override void Execute(TriggerBase e)
	{
		if (SimpleDecalPrefab == null)
		{
			return;
		}
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Quaternion identity = Quaternion.identity;
		if (e.Id == TriggerId.Impact)
		{
			Impact impact = (Impact)e;
			position = impact.Position;
		}
		RaycastHit raycastHit = SimpleDecalHelper.FindAcceptedLocation(position, Layer, RayStartHeight, RayLength);
		if (raycastHit.transform != null)
		{
			position = raycastHit.point + Offset + new Vector3(0f, 0.01f, 0f);
			identity = Quaternion.FromToRotation(SimpleDecalPrefab.transform.up, raycastHit.normal) * Quaternion.Euler(Rotation);
			GameObject effect = EffectPool.GetEffect(SimpleDecalPrefab);
			if (effect != null)
			{
				Transform transform = effect.transform;
				effect.SetActive(true);
				transform.position = position;
				transform.rotation = identity;
			}
			else
			{
				Debug.LogWarning(string.Format("SimpleDecalEffect:Execute() Decal {0} not found in pool. Instantiate decal", SimpleDecalPrefab.name));
				Object.Instantiate(SimpleDecalPrefab, position, identity);
			}
		}
	}
}
