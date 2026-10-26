namespace Survival
{
	public class WaveBreakGamePhase : global::WaveBreakGamePhase
	{
		public WaveBreakGamePhase(GameModeSurvival survival)
			: base(survival)
		{
		}

		public override void OnEnter()
		{
			int num = waveManager.CurrentWaveIndex();
			if (num == -1)
			{
				commandQueue.Add(base.GameStartMessage, 5f);
				commandQueue.Add(PlaySpeech, 0.5f);
				commandQueue.Add(RestoreVolume, 4.5f);
				commandQueue.Add(base.EndPhase);
			}
			else
			{
				base.OnEnter();
			}
		}

		protected override void GotoBossPhase()
		{
			gamePhaseManager.SetPhase(new BossGamePhase(gameMode as GameModeSurvival));
		}

		protected override void GotoWavePhase()
		{
			gamePhaseManager.SetPhase(new WaveGamePhase(gameMode as GameModeSurvival));
		}

		private void PlaySpeech()
		{
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.SetTemporaryVolume(0.5f);
			}
			SpeechManager.PlayGlobalSpeech(SpeechManager.GlobalSpeechLine.Start);
		}

		private void RestoreVolume()
		{
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.SetTemporaryVolume(1f);
			}
		}
	}
}
