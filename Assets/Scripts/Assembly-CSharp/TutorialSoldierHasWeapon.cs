using UnityEngine;

public class TutorialSoldierHasWeapon : TutorialEventBase
{
	public GameObject Soldier;

	public string WeaponName;

	public bool MustBeEquipped;

	private PlayerWeaponController weaponController;

	private void Start()
	{
	}

	private void Update()
	{
		if (MustBeEquipped)
		{
			if (weaponController.ActiveWeapon != null && weaponController.ActiveWeapon.Name == WeaponName && !base.IsComplete)
			{
				SetComplete(true);
			}
		}
		else if (weaponController.HasWeapon(WeaponName) && !base.IsComplete)
		{
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		if (Soldier == null)
		{
			Soldier = GameObject.FindGameObjectWithTag("Soldier");
		}
		weaponController = Soldier.GetComponent<PlayerWeaponController>();
	}
}
