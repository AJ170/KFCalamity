using EffectHelpers;
using UnityEngine;

public class StunEffect : EffectBaseTarget
{
	private AIController aiController;

	private GameObject stunParticle;

	protected override void Awake()
	{
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 4;
		aiController = gameObject.GetComponent<AIController>();
	}

	protected override void OnActivated()
	{
		TriggerStunAIEvent();
		if (ParticleManager.Instance != null)
		{
			stunParticle = ParticleManager.Instance.CreateParticle("FxNewWave", Vector3.zero, Quaternion.identity);
			if (stunParticle != null)
			{
				stunParticle.transform.parent = gameObject.transform;
				stunParticle.transform.localPosition = new Vector3(0f, 0.05f, 0f);
				stunParticle.GetComponent<ParticleSystem>().Play();
			}
		}
	}

	protected override void OnRefreshed(EffectBaseTarget newEffect)
	{
		TriggerStunAIEvent();
	}

	private void TriggerStunAIEvent()
	{
		if (aiController != null)
		{
			aiController.TriggerAIEvent(new StunAIEvent(repeatInterval * (float)maxRepeats));
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (stunParticle != null)
		{
			Object.Destroy(stunParticle);
		}
	}
}
