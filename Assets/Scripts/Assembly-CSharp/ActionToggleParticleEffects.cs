using UnityEngine;

public class ActionToggleParticleEffects : TutorialSubStepBase
{
	public ParticleSystem[] Particles;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		ParticleSystem[] particles = Particles;
		foreach (ParticleSystem particleSystem in particles)
		{
			if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
			else
			{
				particleSystem.Play();
			}
		}
		SetComplete(true);
	}
}
