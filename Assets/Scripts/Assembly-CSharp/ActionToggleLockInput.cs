using Calamity.Input;

public class ActionToggleLockInput : TutorialSubStepBase
{
	public bool IsLocked = true;

	public bool OnlyAffectAbilities;

	public TutorialEventBase Event;

	private SoldierInputController inputController;

	private void Update()
	{
		if (base.IsComplete || !(inputController != null))
		{
			return;
		}
		bool flag = true;
		if (Event != null && !Event.IsComplete)
		{
			flag = false;
		}
		if (flag)
		{
			if (OnlyAffectAbilities)
			{
				inputController.SetAbilityLock(IsLocked, TutorialManager);
			}
			else
			{
				inputController.SetLock(IsLocked, TutorialManager);
			}
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		inputController = Player.GetComponent<SoldierInputController>();
	}
}
