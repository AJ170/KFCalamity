namespace Tutorial
{
	public class WavesDoneGamePhase : GamePhaseBase
	{
		public WavesDoneGamePhase(GameModeBase gameMode)
			: base(gameMode.GamePhaseManagerRef())
		{
		}
	}
}
