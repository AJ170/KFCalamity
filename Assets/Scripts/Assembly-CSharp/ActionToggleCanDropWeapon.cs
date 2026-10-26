public class ActionToggleCanDropWeapon : TutorialSubStepBase
{
	public class LockWeaponDropAction : PlayerAction
	{
		public LockWeaponDropAction()
			: base(ActionType.UseAbility)
		{
		}

		public override bool CanDropWeapon()
		{
			return false;
		}

		public void SetDone()
		{
			state = ActionState.Completed;
		}
	}

	private static LockWeaponDropAction weaponDropAction;

	public bool CanDrop;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		PlayerActionController component = TutorialManager.Soldier.GetComponent<PlayerActionController>();
		if (weaponDropAction == null && !CanDrop)
		{
			weaponDropAction = new LockWeaponDropAction();
			component.AddAction(weaponDropAction);
		}
		else if (weaponDropAction != null && CanDrop)
		{
			weaponDropAction.SetDone();
			weaponDropAction = null;
		}
		SetComplete(true);
	}
}
