using UnityEngine;

public class TutorialPlayerActionSwitchWeapon : TutorialPlayerActionBase
{
	private string specificWeapon;

	private string startWeapon;

	private WeaponController weaponController;

	public TutorialPlayerActionSwitchWeapon(GameObject player, string specificWeapon)
		: base(player)
	{
		this.specificWeapon = specificWeapon;
	}

	public override void OnStartEvent()
	{
		weaponController = Player.GetComponent<PlayerWeaponController>();
		if (weaponController.ActiveWeapon != null)
		{
			startWeapon = weaponController.ActiveWeapon.name;
		}
	}

	public override void Update()
	{
		string text = ((!(weaponController.ActiveWeapon == null)) ? weaponController.ActiveWeapon.name : string.Empty);
		if (string.IsNullOrEmpty(specificWeapon) && text != startWeapon)
		{
			OnComplete();
		}
		if (text == specificWeapon)
		{
			OnComplete();
		}
	}
}
