namespace Survival
{
	public class BossGamePhase : WaveGamePhase
	{
		private CommandQueue commandQueue;

		public BossGamePhase(GameModeSurvival survival)
			: base(survival)
		{
			commandQueue = new CommandQueue();
		}

		public override void OnEnter()
		{
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.StartMusicSet("Boss");
			}
			commandQueue.Add(StartWave, 1f);
		}

		public override void Update()
		{
			commandQueue.Update();
		}

		public override void OnExit()
		{
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.StopMusicSet("Boss");
			}
		}

		private void StartWave()
		{
			base.OnEnter();
		}

		protected override void SetWaveInfo()
		{
			if (guiManager != null)
			{
				guiManager.OnWaveSpawn();
				guiManager.SetBossWave();
			}
		}

		protected override void DisplayWaveMessage()
		{
			if (guiManager != null)
			{
				guiManager.DisplayMessage(Localizer.GetString("gui_message_boss_spawn"), 3f, MessageBoard.FontSize.Large);
			}
		}
	}
}
