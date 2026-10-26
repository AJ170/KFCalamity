using TutorialHelpers;
using UnityEngine;

public class TutorialPlayerAction : TutorialEventBase
{
	public TutorialPlayerActions PlayerAction;

	public int NumberValue;

	public string TextValue;

	public string FailDialogKey;

	public TutorialSpeechLine FailDialogSpeechLine = TutorialSpeechLine.None;

	private TutorialPlayerActionBase playerAction;

	private TutorialQuest tutorialQuest;

	private Health playerHealth;

	private void SetupPlayerAction()
	{
		switch (PlayerAction)
		{
		case TutorialPlayerActions.Move:
			playerAction = new TutorialPlayerActionMove(Player, NumberValue);
			break;
		case TutorialPlayerActions.LookAround:
			playerAction = new TutorialPlayerActionLookAround(Player, NumberValue);
			break;
		case TutorialPlayerActions.Aim:
			playerAction = new TutorialPlayerActionAim(Player);
			break;
		case TutorialPlayerActions.Dodge:
			playerAction = new TutorialPlayerActionDodge(Player);
			break;
		case TutorialPlayerActions.Attack:
			playerAction = new TutorialPlayerActionAttack(Player);
			break;
		case TutorialPlayerActions.SwitchWeapon:
			playerAction = new TutorialPlayerActionSwitchWeapon(Player, TextValue);
			break;
		case TutorialPlayerActions.Heal:
			playerAction = new TutorialPlayerActionHeal(Player);
			break;
		case TutorialPlayerActions.HealTarget:
			playerAction = new TutorialPlayerActionHealTarget(Player);
			break;
		case TutorialPlayerActions.PerkAbility:
			playerAction = new TutorialPLayerActionPerkAbility(Player);
			break;
		case TutorialPlayerActions.DropWeapon:
			playerAction = new TutorialPlayerActionDropWeapon(Player, TextValue);
			break;
		case TutorialPlayerActions.Reload:
			playerAction = new TutorialPlayerActionReload(Player);
			break;
		default:
			Debug.LogError("OnPlayerAction:SetupPlayerAction() - Unknown Player Action: '" + playerAction.ToString() + "'");
			break;
		}
		playerAction.SetCompletedCallback(OnCompleted);
		playerAction.SetFailedCallback(OnFailed);
	}

	private void OnCompleted()
	{
		SetComplete(true);
	}

	private void OnFailed()
	{
		if (!string.IsNullOrEmpty(FailDialogKey))
		{
			TutorialManager.RunSpeech(Localizer.GetString(FailDialogKey), 0.02f, null);
			if (FailDialogSpeechLine != TutorialSpeechLine.None)
			{
				TutorialSpeechManager.Play(FailDialogSpeechLine);
			}
		}
	}

	public override void OnStart()
	{
		SetupPlayerAction();
		playerAction.OnStartEvent();
		playerHealth = Player.GetComponent<Health>();
		tutorialQuest = GetComponent<TutorialQuest>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!base.IsComplete)
		{
			if (tutorialQuest != null && tutorialQuest.QuestID != null)
			{
				playerAction.SetQuestID(tutorialQuest.QuestID);
				tutorialQuest = null;
			}
			if (playerAction != null && Player != null && !playerHealth.Dead)
			{
				playerAction.Update();
			}
		}
	}
}
