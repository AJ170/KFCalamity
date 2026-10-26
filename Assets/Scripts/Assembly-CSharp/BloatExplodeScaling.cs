using UnityEngine;

public class BloatExplodeScaling : MonoBehaviour
{
	public SphereCollisionHitEffect SphereCollisionScript;

	public TriggerParticleEffect ParticleScript;

	private void Awake()
	{
		float num = ScalingByGameSettings.BloatExplodeRadiusModifierByDifficulty();
		if (SphereCollisionScript != null)
		{
			SphereCollisionScript.Radius *= num;
		}
		if (ParticleScript != null)
		{
			ParticleScript.Scale *= num;
		}
	}
}
