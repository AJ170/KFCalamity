using EventHelpers;

public class TriggerPatriarchZedSpawnEffect : EffectBaseListener
{
	public BossMiniWave Wave1;

	public BossMiniWave Wave2;

	public BossMiniWave Wave3;

	private SpawnManager spawnManager;

	private int executeCount;

	protected override void Start()
	{
		base.Start();
		Wave1.Setup();
		Wave2.Setup();
		Wave3.Setup();
		spawnManager = SpawnManager.Instance;
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && spawnManager != null)
		{
			executeCount++;
			if (executeCount == 1)
			{
				spawnManager.AddBossMiniWave(Wave1);
			}
			else if (executeCount == 2)
			{
				spawnManager.AddBossMiniWave(Wave2);
			}
			else
			{
				spawnManager.AddBossMiniWave(Wave3);
			}
		}
	}
}
