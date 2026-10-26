namespace Tutorial
{
	public class GameOverGamePhase : global::GameOverGamePhase
	{
		public GameOverGamePhase(Game.Context context, bool victorious)
			: base(context, victorious)
		{
		}

		public override void OnEnter()
		{
			if (victorious)
			{
				LevelCompleted();
			}
			TutorialManager.HideQuestLog();
			commandQueue.Add(DisplayEndScreen, 1f);
			commandQueue.Add(base.LockPlayerInput);
		}

		protected override void DisplayEndScreen()
		{
			EndScreenType endType = EndScreenType.TutorialLose;
			if (victorious)
			{
				endType = EndScreenType.TutorialWin;
				TutorialSpeechManager.Play(TutorialSpeechLine.Win);
			}
			else
			{
				TutorialSpeechManager.Play(TutorialSpeechLine.Lose);
			}
			context.GUIManager.DisplayEndScreen(endType);
		}

		private void LevelCompleted()
		{
			MetaManager instance = MetaManager.Instance;
			if (instance != null)
			{
				GameSettings gameSettings = GameState.Instance.GameSettings;
				OptionManager instance2 = OptionManager.Instance;
				instance.LevelCompleted(gameSettings.LevelName, DifficultySetting.Normal);
				if (instance2 != null && instance2.TutorialPopup == 0)
				{
					instance2.TutorialPopup = 1;
					instance2.SetSaveData(instance2.Serialize());
				}
			}
		}
	}
}
