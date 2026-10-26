using UnityEngine;

public class PatriarchHealEffect : HealOverTimeEffect
{
	private GameObject healParticle;

	protected override void Awake()
	{
		base.Awake();
		repeatInterval = 0.5f;
		maxRepeats = 10;
	}

	protected override void OnActivated()
	{
		int num = 1;
		AIAbilityController component = gameObject.GetComponent<AIAbilityController>();
		if (component != null)
		{
			Ability ability = component.AbilityByName("PatriarchHealMove");
			if (ability != null)
			{
				num = ability.ChargesLeft();
			}
		}
		GuiInGameManager instance = GuiInGameManager.Instance;
		if (instance != null)
		{
			instance.bossBar.DecreaseSyringes();
		}
		healthPerTick = 0;
		switch (num)
		{
		case 2:
			healthPerTick = (int)((double)healthComponent.MaxHealth * 0.85 - (double)healthComponent.CurrentHealth) / 10;
			break;
		case 1:
			healthPerTick = (int)((double)healthComponent.MaxHealth * 0.7 - (double)healthComponent.CurrentHealth) / 10;
			break;
		case 0:
			healthPerTick = (int)((double)healthComponent.MaxHealth * 0.55 - (double)healthComponent.CurrentHealth) / 10;
			break;
		}
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
