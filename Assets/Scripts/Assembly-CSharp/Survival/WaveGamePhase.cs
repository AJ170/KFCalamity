namespace Survival
{
	public class WaveGamePhase : global::WaveGamePhase
	{
		public WaveGamePhase(GameModeSurvival survival)
			: base(survival)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			PickupManager.Instance.SetupDynamicPickups();
		}

		public override void GotoWaveBreakPhase()
		{
			gamePhaseManager.SetPhase(new WaveBreakGamePhase(gameMode as GameModeSurvival));
		}
	}
}
