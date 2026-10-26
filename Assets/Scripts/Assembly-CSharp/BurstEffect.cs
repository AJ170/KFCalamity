using EventHelpers;
using UnityEngine;

public class BurstEffect : EffectBaseListener
{
	public Weapon Weapon;

	public Ability Ability;

	public int BurstLength = 3;

	public float BurstDelay = 1f;

	public float BurstReset = 0.5f;

	private float baseCooldown = -1f;

	private int burstCounter;

	private float delayCounter;

	protected override void Start()
	{
		base.Start();
		if (Ability != null)
		{
			baseCooldown = Ability.Cooldown;
		}
		else if (Weapon != null)
		{
			baseCooldown = Weapon.RoundCooldown;
		}
	}

	private void Update()
	{
		if (delayCounter > 0f)
		{
			delayCounter -= Time.deltaTime;
			if (delayCounter <= 0f)
			{
				SetCooldown(baseCooldown);
				burstCounter = 0;
			}
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			burstCounter++;
			if (burstCounter >= BurstLength)
			{
				SetCooldown(BurstDelay);
				burstCounter = 0;
			}
			delayCounter = BurstReset;
		}
	}

	private void SetCooldown(float cooldown)
	{
		if (Ability != null)
		{
			Ability.Cooldown = cooldown;
		}
		else if (Weapon != null)
		{
			Weapon.RoundCooldown = cooldown;
		}
	}
}
