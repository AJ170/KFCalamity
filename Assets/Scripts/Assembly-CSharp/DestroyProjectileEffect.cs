using EventHelpers;

public class DestroyProjectileEffect : EffectBaseListener
{
	public bool OnlyShortCircuit;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		PhysicalProjectile componentInParents = base.gameObject.GetComponentInParents<PhysicalProjectile>();
		if (componentInParents != null)
		{
			if (OnlyShortCircuit)
			{
				componentInParents.ShortCircuitProjectile();
			}
			else
			{
				componentInParents.DestroyProjectile();
			}
		}
	}
}
