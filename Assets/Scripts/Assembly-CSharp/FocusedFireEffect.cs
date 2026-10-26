using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class FocusedFireEffect : EffectBaseTarget
{
	private AttributeModifier damageDealt;

	private AttributeModifier moneyReceived;

	private GameObject focusParticle;

	private float damageMultiplier = 1.25f;

	private float moneyMultiplier = 1.25f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 10;
		receiveTriggers = true;
		damageDealt = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, gameObject);
		moneyReceived = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.MoneyReceived, gameObject);
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			focusParticle = ParticleManager.Instance.CreateParticle("FxFocusedFire", Vector3.zero, Quaternion.identity);
			if (focusParticle != null)
			{
				focusParticle.transform.parent = gameObject.transform;
				focusParticle.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				focusParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		damageDealt.Alter(this, damageMultiplier);
		moneyReceived.Alter(this, moneyMultiplier);
	}

	protected override void OnRefreshed(EffectBaseTarget newEffect)
	{
		if (focusParticle != null)
		{
			focusParticle.GetComponent<ParticleSystem>().Stop();
			focusParticle.GetComponent<ParticleSystem>().Play();
		}
	}

	public override void OnReceivedTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.Kill && ParticleManager.Instance != null)
		{
			GameObject gameObject = ParticleManager.Instance.CreateParticle("FxMoneyGained", Vector3.zero, Quaternion.identity, ParticleDestroyMethod.Duration);
			if (gameObject != null)
			{
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				gameObject.GetComponent<ParticleSystem>().Play();
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (focusParticle != null)
		{
			Object.Destroy(focusParticle);
		}
		damageDealt.Restore(this);
		moneyReceived.Restore(this);
	}
}
