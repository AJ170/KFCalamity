using EventHelpers;
using UnityEngine;

public class HuntingShotgunSecondaryFireEffect : EffectBaseListener
{
	public Weapon Primary;

	public float Delay = 0.1f;

	private PlayerWeaponController weaponController;

	private float counter;

	private void OnMounted(GameObject parent)
	{
		weaponController = parent.GetComponent<PlayerWeaponController>();
	}

	private void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				FirePrimary();
			}
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && Primary != null)
		{
			FirePrimary();
			if (Primary.LoadedAmmunition > 0)
			{
				counter = Delay;
			}
			else
			{
				counter = 0f;
			}
		}
	}

	private void FirePrimary()
	{
		if (weaponController != null)
		{
			weaponController.Attack();
		}
	}
}
