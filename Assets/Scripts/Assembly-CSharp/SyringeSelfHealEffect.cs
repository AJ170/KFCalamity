using UnityEngine;

public class SyringeSelfHealEffect : HealOverTimeEffect
{
	private GameObject healParticle;

	protected override void Awake()
	{
		base.Awake();
		repeatInterval = 0.5f;
		maxRepeats = 10;
		healthPerTick = 5;
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			healParticle = ParticleManager.Instance.CreateParticle("FxHealthRegen", Vector3.zero, Quaternion.identity);
			if (healParticle != null)
			{
				healParticle.transform.parent = gameObject.transform;
				healParticle.transform.localPosition = new Vector3(0f, 1.5f, 0f);
				healParticle.GetComponent<ParticleSystem>().Play();
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (healParticle != null)
		{
			Object.Destroy(healParticle);
		}
	}
}
