using EventHelpers;
using UnityEngine;

public class WeaponRecoilEffect : EffectBaseListener
{
	public float Multiplier = 1f;

	public int MaxStacks = 1;

	public float StackFallOffTime = 0.5f;

	private int numStacks;

	private float counter;

	private AttributeModifier spreadModifier;

	protected override void Start()
	{
		base.Start();
		spreadModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Spread, base.gameObject);
	}

	protected override void Execute(TriggerBase e)
	{
		if (NoCustomEvents() || MatchCustomEvent(e.CustomName))
		{
			AddStack();
		}
	}

	private void Update()
	{
		UpdateFallOffTime();
	}

	private void AddStack()
	{
		if (numStacks < MaxStacks)
		{
			numStacks++;
			AlterSpread();
		}
		counter = 0f;
	}

	private void UpdateFallOffTime()
	{
		if (numStacks > 0)
		{
			counter = Mathf.Min(counter + Time.deltaTime, StackFallOffTime);
			if (counter == StackFallOffTime)
			{
				numStacks--;
				AlterSpread();
				counter = 0f;
			}
		}
	}

	private void AlterSpread()
	{
		spreadModifier.Restore(this);
		if (numStacks > 0)
		{
			spreadModifier.Alter(this, Mathf.Pow(Multiplier, numStacks));
		}
	}

	protected override void Shutdown()
	{
		base.Shutdown();
		if (spreadModifier != null)
		{
			spreadModifier.Restore(this);
		}
	}
}
