namespace Meta
{
	public struct LevelCompletedLog
	{
		public string levelName;

		public DifficultySetting difficulty;

		public LevelCompletedLog(string levelName, DifficultySetting difficulty)
		{
			this.levelName = levelName;
			this.difficulty = difficulty;
		}
	}
}
