using EventHelpers;

namespace EffectHelpers
{
	public class ReceiveTriggerHandler : IEffectTriggerListener
	{
		private EffectBaseTarget effect;

		public ReceiveTriggerHandler(EffectBaseTarget effect)
		{
			this.effect = effect;
			effect.Owner.EventDelegator.RegisterListener(this);
		}

		public void Shutdown()
		{
			effect.Owner.EventDelegator.UnregisterListener(this);
		}

		public void OnEffectTrigger(TriggerBase e)
		{
			if (!effect.IsDeleted)
			{
				effect.OnReceivedTrigger(e);
			}
		}
	}
}
