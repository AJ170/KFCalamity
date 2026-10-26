using EffectEvents;
using EventHelpers;
using UnityEngine;

public class MaterialImpactEffect : EffectBaseListener
{
	public bool Sound = true;

	public bool Particle = true;

	public float Volume = 0.5f;

	public bool LetTimeAffectPitch = true;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (e.Id == TriggerId.Impact)
		{
			Impact impact = (Impact)e;
			Collider collider = impact.Collider;
			if (collider != null)
			{
				PhysicMaterial sharedMaterial = collider.sharedMaterial;
				MaterialEffect(sharedMaterial, impact.Position, impact.FromDirection);
			}
		}
		else if (e.Id == TriggerId.Hit)
		{
			Hit hit = (Hit)e;
			Collider collider2 = hit.HitTransform.GetComponent<Collider>();
			if (collider2 != null)
			{
				PhysicMaterial sharedMaterial2 = collider2.sharedMaterial;
				MaterialEffect(sharedMaterial2, base.transform.position, -base.transform.forward);
			}
		}
		else if (e.Id == TriggerId.BulletImpact)
		{
			BulletImpact bulletImpact = (BulletImpact)e;
			PhysicMaterial material = bulletImpact.Material;
			MaterialEffect(material, bulletImpact.Position, bulletImpact.FromDirection);
		}
	}

	private void MaterialEffect(PhysicMaterial material, Vector3 position, Vector3 fromDirection)
	{
		if (material != null)
		{
			if (Sound)
			{
				TriggerSound(material, position);
			}
			if (Particle)
			{
				TriggerParticle(material, position, fromDirection);
			}
		}
	}

	private void TriggerSound(PhysicMaterial physicsMaterial, Vector3 position)
	{
		AudioClip impactSound = MaterialImpactManager.GetImpactSound(physicsMaterial);
		if (impactSound != null)
		{
			if (LetTimeAffectPitch)
			{
				AudioManager.PlayClipAtPoint(impactSound, position, Volume, Time.timeScale);
			}
			else
			{
				AudioManager.PlayClipAtPoint(impactSound, position, Volume);
			}
		}
	}

	private void TriggerParticle(PhysicMaterial physicsMaterial, Vector3 position, Vector3 fromDirection)
	{
		GameObject impactEffect = MaterialImpactManager.GetImpactEffect(physicsMaterial);
		if (impactEffect != null)
		{
			impactEffect.SetActive(true);
			impactEffect.transform.position = position;
			impactEffect.transform.rotation = Quaternion.LookRotation(fromDirection, Vector3.up);
			ParticleSystem particleSystem = impactEffect.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				particleSystem.Play();
			}
		}
	}
}
