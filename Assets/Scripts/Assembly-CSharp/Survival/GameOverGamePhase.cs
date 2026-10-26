using System.Collections.Generic;
using Acheivements;

namespace Survival
{
	public class GameOverGamePhase : global::GameOverGamePhase
	{
		private int elapsedTime;

		public GameOverGamePhase(Game.Context context, bool victorious)
			: base(context, victorious)
		{
		}

		public override void OnEnter()
		{
			elapsedTime = (int)GameState.Instance.GameModeManager.ElapsedTime;
			string musicSetName;
			if (victorious)
			{
				musicSetName = "Victory";
				LevelCompleted();
			}
			else
			{
				musicSetName = "Loss";
			}
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.StartMusicSet(musicSetName);
			}
			ProcessAcheivements();
			commandQueue.Add(DisplayEndScreen, 3f);
			commandQueue.Add(base.LockPlayerInput);
		}

		private void LevelCompleted()
		{
			MetaManager instance = MetaManager.Instance;
			if (instance != null)
			{
				GameSettings gameSettings = GameState.Instance.GameSettings;
				instance.LevelCompleted(gameSettings.LevelName, gameSettings.Difficulty);
				LifetimeStatsHandler.AddLevelCompleted(gameSettings.Difficulty);
			}
		}

		private void ProcessAcheivements()
		{
			List<PlayerStats> allStats = PlayerStatsManager.Instance.GetAllStats();
			int num = 0;
			int num2 = 0;
			foreach (PlayerStats item in allStats)
			{
				num += item.Kills;
				num2 += item.HealingDealt;
			}
			AcheivementManager.Trigger(new KillsEvent(num));
			AcheivementManager.Trigger(new HealingEvent(num2));
			if (victorious)
			{
				AcheivementManager.Trigger(new TimeEvent(elapsedTime));
			}
			SaveAchievements();
		}

		private void SaveAchievements()
		{
			MetaManager instance = MetaManager.Instance;
			if (instance != null)
			{
				GameSettings gameSettings = GameState.Instance.GameSettings;
				List<AcheivementBase> progressionLogs = AcheivementManager.Instance.GetProgressionLogs();
				for (int i = 0; i < progressionLogs.Count; i++)
				{
					AcheivementBase acheivementBase = progressionLogs[i];
					instance.UpdateAchievement(gameSettings.LevelName, acheivementBase.ID, acheivementBase.Status);
				}
			}
		}

		public override void OnExit()
		{
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.PlayLevelMusicSet();
			}
		}

		protected override void DisplayEndScreen()
		{
			base.DisplayEndScreen();
			if (victorious)
			{
				SpeechManager.PlayGlobalSpeech(SpeechManager.GlobalSpeechLine.Win);
			}
			else
			{
				SpeechManager.PlayGlobalSpeech(SpeechManager.GlobalSpeechLine.Lose);
			}
		}
	}
}
