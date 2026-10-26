namespace ActorEvents
{
	public class UseAbility : ActorEventBase
	{
		private Ability ability;

		public Ability Ability
		{
			get
			{
				return ability;
			}
		}

		public UseAbility(Ability ability)
			: base(ActorEventID.UseAbility)
		{
			this.ability = ability;
		}
	}
}
