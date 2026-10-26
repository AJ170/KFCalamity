public class GameLink
{
	private Game game;

	public GameLink(Game game)
	{
		this.game = game;
	}

	public void ResetSystems()
	{
		game.ResetSystems();
	}
}
