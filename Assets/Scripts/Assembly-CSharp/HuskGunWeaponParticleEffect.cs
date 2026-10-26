using EventHelpers;
using UnityEngine;

public class HuskGunWeaponParticleEffect : EffectBaseListener
{
	public Transform Mount;

	public ParticleSystem StartingSystem;

	public ParticleSystem FullSystem;

	protected override void Start()
	{
		base.Start();
		if (effectSystem != null)
		{
			effectSystem.EventSystem.Subscribe(TriggerId.ChargeUpMax, Execute, this);
			effectSystem.EventSystem.Subscribe(TriggerId.ChargeUpRelease, Execute, this);
			effectSystem.EventSystem.Subscribe(TriggerId.ChargeUpCancel, Execute, this);
		}
		if (Mount == null)
		{
			Mount = base.transform;
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			if (e.Id == TriggerId.ChargeUpStart)
			{
				OnChargeUpStart();
			}
			else if (e.Id == TriggerId.ChargeUpMax)
			{
				OnChargeUpMax();
			}
			else if (e.Id == TriggerId.ChargeUpRelease || e.Id == TriggerId.ChargeUpCancel)
			{
				OnChargeUpRelease();
			}
		}
	}

	private void OnChargeUpStart()
	{
		StartingSystem.gameObject.SetActive(true);
		StartingSystem.Clear();
	}

	private void OnChargeUpMax()
	{
		StartingSystem.gameObject.SetActive(false);
		FullSystem.gameObject.SetActive(true);
	}

	private void OnChargeUpRelease()
	{
		StartingSystem.gameObject.SetActive(false);
		FullSystem.gameObject.SetActive(false);
	}
}
