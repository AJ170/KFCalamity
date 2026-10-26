using Calamity.Input;
using UnityEngine;

public class ActionOpenShop : TutorialEventBase
{
	public int OpenShopDelay = 4;

	public int GiveMoney;

	public int HealthToRestore;

	private float timer = -1f;

	private SoldierInputController inputController;

	private void Start()
	{
	}

	private void Update()
	{
		if (timer != -1f)
		{
			timer = Mathf.Max(0f, timer - Time.deltaTime);
			if (timer == 0f)
			{
				TutorialManager.ToggleQuestLog(false);
				TutorialManager.DestroyTutorialMessage();
				TutorialManager.GuiManager.OpenShop();
				timer = -1f;
			}
		}
		GuiInGameManager guiManager = TutorialManager.GuiManager;
		if (guiManager.IsShopOpen() && guiManager.IsShopPhaseDone())
		{
			TutorialManager.ToggleQuestLog(true);
			guiManager.CloseShop();
			inputController.SetLock(false, this);
			SetComplete(true);
			base.enabled = false;
		}
	}

	private void GiveMoneyToPlayer()
	{
		PlayerStats playerStats = PlayerStatsManager.Instance.StatsByPlayerId(Player.GetComponent<Soldier>().PlayerID);
		playerStats.AddMoney(GiveMoney);
		Player.GetComponent<PlayerGUILink>().OnMoneyGained(GiveMoney);
	}

	private void RestoreHealth()
	{
		Health component = Player.GetComponent<Health>();
		if (component != null)
		{
			component.RestoreHealthExact(HealthToRestore);
		}
	}

	public override void OnStart()
	{
		GiveMoneyToPlayer();
		RestoreHealth();
		timer = OpenShopDelay;
		TutorialManager.GuiManager.DisplayMessage(Localizer.GetString("gui_message_open_shop"), OpenShopDelay);
		inputController = Player.GetComponent<SoldierInputController>();
		inputController.SetLock(true, this);
	}
}
