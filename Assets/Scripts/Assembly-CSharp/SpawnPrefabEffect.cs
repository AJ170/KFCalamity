using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SpawnPrefabEffect : EffectBaseListener
{
	public GameObject Prefab;

	public Vector3 Offset;

	public float ForwardOffset;

	public bool FaceForward;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (Prefab != null)
		{
			Vector3 position = base.transform.position;
			if (e.Id == TriggerId.Impact)
			{
				position = ((Impact)e).Position;
			}
			position += Offset + base.transform.forward * ForwardOffset;
			GameObject gameObject = Object.Instantiate(Prefab, position, Quaternion.identity) as GameObject;
			StandaloneEntity.OnCreated(gameObject, base.transform.root.gameObject);
			if (FaceForward)
			{
				Quaternion rotation = Quaternion.LookRotation(base.transform.root.forward);
				gameObject.transform.rotation = rotation;
			}
		}
		else
		{
			Debug.LogError(string.Format("SpawnPrefabEffect:Execute() -> Missing Prefab in {0}({1})", base.transform.name, base.transform.root.name));
		}
	}
}
