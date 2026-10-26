using EventHelpers;
using UnityEngine;

public class TimedLaserBlockEffect : EffectBaseListener
{
	public float Duration = 1f;

	private PlayerWeaponController weaponController;

	private float counter;

	private void OnMounted(GameObject parent)
	{
		weaponController = parent.GetComponent<PlayerWeaponController>();
	}

	private void Update()
	{
		if (counter >= 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f && weaponController != null)
			{
				weaponController.SetLaserActive(true, this);
			}
		}
	}

	private void OnDestroy()
	{
		if (counter >= 0f && weaponController != null)
		{
			weaponController.SetLaserActive(true, this);
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && weaponController != null)
		{
			if (counter <= 0f)
			{
				weaponController.SetLaserActive(false, this);
			}
			counter = Duration;
		}
	}
}
