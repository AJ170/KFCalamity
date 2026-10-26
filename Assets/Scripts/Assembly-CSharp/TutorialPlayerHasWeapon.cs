public class TutorialPlayerHasWeapon : TutorialEventBase
{
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
			if (weaponController.ActiveWeapon != null && weaponController.ActiveWeapon.Name == WeaponName)
			{
				SetComplete(true);
			}
		}
		else if (weaponController.HasWeapon(WeaponName))
		{
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		weaponController = Player.GetComponent<PlayerWeaponController>();
	}
}
