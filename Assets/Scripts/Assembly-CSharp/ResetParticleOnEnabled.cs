using UnityEngine;

public class ResetParticleOnEnabled : MonoBehaviour
{
	public ParticleSystem Particle;

	public bool TriggerPlay;

	private void OnEnable()
	{
		if (Particle != null)
		{
			Particle.Clear();
			if (TriggerPlay)
			{
				Particle.Play();
			}
		}
	}
}
