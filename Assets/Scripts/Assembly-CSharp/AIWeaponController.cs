using UnityEngine;

public class AIWeaponController : WeaponController
{
	public GameObject DefaultWeapon;

	protected override void Start()
	{
		base.Start();
		if (DefaultWeapon != null)
		{
			SetActiveWeapon(DefaultWeapon);
		}
	}

	private void OnDestroy()
	{
		DestroyActiveWeapon();
	}
}
