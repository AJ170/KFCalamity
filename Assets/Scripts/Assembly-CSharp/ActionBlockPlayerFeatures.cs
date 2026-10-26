public class ActionBlockPlayerFeatures : TutorialSubStepBase
{
	public bool BlockZedTime = true;

	public bool BlockKillstreak = true;

	public override void OnStart()
	{
		KillStatController component = Player.GetComponent<KillStatController>();
		if (component != null)
		{
			component.BlockKillStreak = BlockKillstreak;
		}
		ZedTimeManager.BlockZedTime = BlockZedTime;
		SetComplete(true);
	}
}
