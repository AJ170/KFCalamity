using EventHelpers;
using UnityEngine;

public class ResetParticleEffect : EffectBaseListener
{
	public ParticleSystem Particle;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && Particle != null)
		{
			Particle.Clear();
		}
	}
}
