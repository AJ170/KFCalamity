using System;
using UnityEngine;

public class TutorialPlayerActionDropWeapon : TutorialPlayerActionBase
{
	private PlayerWeaponController weaponController;

	private InventoryWeaponSlot invSlot;

	private QuestLog questLog;

	private GuiInGameManager ingameManager;

	private bool slotHasContainedWeapon;

	private bool isInitializedForTouch;

	private bool wasWeaponTrayVisible;

	private bool hasSymbolBeenChanged;

	public TutorialPlayerActionDropWeapon(GameObject player, string inventorySlotEnum)
		: base(player)
	{
		invSlot = (InventoryWeaponSlot)(int)Enum.Parse(typeof(InventoryWeaponSlot), inventorySlotEnum);
	}

	public override void OnStartEvent()
	{
		weaponController = Player.GetComponent<PlayerWeaponController>();
		slotHasContainedWeapon = weaponController.HasWeapon(invSlot);
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			InitializeForTouch();
		}
	}

	private void InitializeForTouch()
	{
		questLog = UnityEngine.Object.FindObjectOfType<QuestLog>();
		ingameManager = GuiInGameManager.Instance;
		isInitializedForTouch = true;
	}

	private void UpdateTouch()
	{
		if (!isInitializedForTouch)
		{
			InitializeForTouch();
		}
	}

	public override void Update()
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			UpdateTouch();
		}
		if (!slotHasContainedWeapon)
		{
			slotHasContainedWeapon = weaponController.HasWeapon(invSlot);
		}
		else if (slotHasContainedWeapon && !weaponController.HasWeapon(invSlot))
		{
			OnComplete();
		}
	}
}
