public class GameSettings
{
	private string levelName;

	private string guiLevelName;

	private GameModeSetting gameMode = GameModeSetting.Survival;

	private DifficultySetting difficulty = DifficultySetting.Normal;

	private GameLengthSetting gameLength;

	private int numPlayers = 1;

	private bool levelChosenAtRandom;

	public GameModeSetting GameMode
	{
		get
		{
			return gameMode;
		}
		set
		{
			gameMode = value;
		}
	}

	public string LevelName
	{
		get
		{
			return levelName;
		}
		set
		{
			levelName = value;
		}
	}

	public string GuiLevelName
	{
		get
		{
			return guiLevelName;
		}
		set
		{
			guiLevelName = value;
		}
	}

	public DifficultySetting Difficulty
	{
		get
		{
			return difficulty;
		}
		set
		{
			difficulty = value;
		}
	}

	public GameLengthSetting GameLength
	{
		get
		{
			return gameLength;
		}
		set
		{
			gameLength = value;
		}
	}

	public int NumPlayers
	{
		get
		{
			return numPlayers;
		}
		set
		{
			numPlayers = value;
		}
	}

	public bool LevelChosenAtRandom
	{
		get
		{
			return levelChosenAtRandom;
		}
		set
		{
			levelChosenAtRandom = value;
		}
	}

	public GameSettings()
	{
	}

	public GameSettings(GameModeSetting gameMode, string levelName, DifficultySetting difficulty, GameLengthSetting gameLength, int numPlayers)
	{
		this.gameMode = gameMode;
		this.levelName = levelName;
		this.difficulty = difficulty;
		this.gameLength = gameLength;
		this.numPlayers = numPlayers;
	}
}
