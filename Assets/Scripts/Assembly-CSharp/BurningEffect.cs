using EffectHelpers;
using Ragdoll;
using UnityEngine;

public class BurningEffect : DamageOverTimeEffect
{
	private GameObject burnParticle;

	private RagdollController ragdollController;

	private AppliedRagdollEffect ragdollEffect;

	protected override void Awake()
	{
		base.Awake();
		buffClass = BuffClass.Fire;
		repeatInterval = 0.5f;
		maxRepeats = 12;
		damagePerTick = 10;
		damageType = DamageType.Fire;
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			burnParticle = ParticleManager.Instance.CreateParticle("FxOnFireSmall", Vector3.zero, Quaternion.identity);
			if (burnParticle != null)
			{
				burnParticle.transform.parent = gameObject.transform;
				burnParticle.transform.localPosition = new Vector3(0f, 1.5f, 0f);
				burnParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		ragdollController = gameObject.GetComponent<RagdollController>();
		if (ragdollController != null)
		{
			ragdollEffect = new AppliedRagdollEffect(RagdollEffectType.Burning, -1f);
			ragdollController.ApplyEffect(ragdollEffect);
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (burnParticle != null)
		{
			Object.Destroy(burnParticle);
		}
		if (ragdollController != null && ragdollEffect != null)
		{
			ragdollController.RemoveEffect(ragdollEffect);
		}
	}

	protected override void OnKill(GameObject attacker, GameObject victim)
	{
		if (Random.value < 0.143f)
		{
			SpeechManager.PlaySpeech(SpeechManager.SpeechLine.SpecimenBurnedToDeath, attacker);
		}
		base.OnKill(attacker, victim);
	}
}
