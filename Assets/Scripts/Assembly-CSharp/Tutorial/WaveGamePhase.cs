namespace Tutorial
{
	public class WaveGamePhase : global::WaveGamePhase
	{
		public WaveGamePhase(GameModeTutorial tutorial)
			: base(tutorial)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void GotoWaveBreakPhase()
		{
			gamePhaseManager.SetPhase(new WaveBreakGamePhase(gameMode as GameModeTutorial, false));
		}
	}
}
