public class LineOfSightSource : LineOfSightComponent
{
	protected override void Register()
	{
		LineOfSightManager.Instance.RegisterSource(this);
	}

	protected override void Unregister()
	{
		LineOfSightManager.Instance.UnregisterSource(this);
	}
}
