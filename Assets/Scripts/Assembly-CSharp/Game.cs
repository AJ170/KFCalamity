public class Game
{
	public class Context
	{
		private GameLink game;

		private EventSystem eventSystem;

		private StateManager stateManager;

		private LevelManager levelManager;

		private GameUnitManager gameUnitManager;

		private PlayerManager playerManager;

		private GuiInGameManager guiManager;

		public GameLink Game
		{
			get
			{
				return game;
			}
			set
			{
				game = value;
			}
		}

		public EventSystem EventSystem
		{
			get
			{
				return eventSystem;
			}
			set
			{
				eventSystem = value;
			}
		}

		public StateManager StateManager
		{
			get
			{
				return stateManager;
			}
			set
			{
				stateManager = value;
			}
		}

		public LevelManager LevelManager
		{
			get
			{
				return levelManager;
			}
			set
			{
				levelManager = value;
			}
		}

		public GameUnitManager GameUnitManager
		{
			get
			{
				return gameUnitManager;
			}
			set
			{
				gameUnitManager = value;
			}
		}

		public PlayerManager PlayerManager
		{
			get
			{
				return playerManager;
			}
			set
			{
				playerManager = value;
			}
		}

		public GuiInGameManager GUIManager
		{
			get
			{
				return guiManager;
			}
			set
			{
				guiManager = value;
			}
		}
	}

	private GameLink _gameLink;

	private EventSystem _eventSystem;

	private StateManager _stateManager;

	private LevelManager _levelManager;

	private PlayerManager _playerManager;

	private PlayerStatsManager _playerStatsManager;

	private SoldierManager _soldierManager;

	private Context _context = new Context();

	public Game()
	{
		_gameLink = new GameLink(this);
		_eventSystem = new EventSystem();
		_stateManager = new StateManager();
		_levelManager = new LevelManager();
		_playerManager = new PlayerManager(_eventSystem);
		_playerStatsManager = new PlayerStatsManager(_eventSystem);
		_soldierManager = new SoldierManager(_eventSystem);
		_context.Game = _gameLink;
		_context.EventSystem = _eventSystem;
		_context.StateManager = _stateManager;
		_context.LevelManager = _levelManager;
		_context.PlayerManager = _playerManager;
		_stateManager.AddState("StartUp", new StartUpState(_context));
	}

	public void Update()
	{
		_stateManager.Update();
		_levelManager.Update();
	}

	public void ResetSystems()
	{
		_eventSystem.UnsubscribeAll();
		_playerStatsManager.ResetAllStats();
		_soldierManager.Clear();
		_playerStatsManager.RegisterEvents();
		_soldierManager.RegisterEvents();
	}
}
