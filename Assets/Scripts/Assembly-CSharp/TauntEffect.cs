using EffectHelpers;
using UnityEngine;

public class TauntEffect : EffectBaseTarget
{
	private float duration = 5f;

	private AITargetHandler targetHandler;

	private bool oldFixateSetting;

	private GameObject particle;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.None;
		repeatInterval = duration;
		maxRepeats = 1;
		targetHandler = gameObject.GetComponent<AITargetHandler>();
	}

	protected override void OnActivated()
	{
		if (!(targetHandler != null) || !(base.Source != null))
		{
			return;
		}
		oldFixateSetting = targetHandler.FixateTargets;
		targetHandler.FixateTargets = true;
		targetHandler.SetTarget(base.Source);
		if (ParticleManager.Instance != null)
		{
			particle = ParticleManager.Instance.CreateParticle("FxTaunted", Vector3.zero, Quaternion.identity);
			if (particle != null)
			{
				particle.transform.parent = gameObject.transform;
				particle.transform.localPosition = new Vector3(0f, 2.5f, 0f);
				particle.GetComponent<ParticleSystem>().Play();
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (targetHandler != null)
		{
			targetHandler.FixateTargets = oldFixateSetting;
		}
		if (particle != null)
		{
			Object.Destroy(particle);
		}
	}
}
