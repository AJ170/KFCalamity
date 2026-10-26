using EffectEvents;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float Speed = 10f;

	public float Lifetime = 1f;

	public float MaxDistance = 1000f;

	public bool DestroyOnDeath;

	private bool triggerImpact;

	private EffectSystem parentEffectSystem;

	private PhysicMaterial impactPhysicsMaterial;

	private Vector3 impactPosition = Vector3.zero;

	private float timeSinceSpawn;

	private float distanceTravelled;

	protected virtual void Update()
	{
		timeSinceSpawn += Time.deltaTime;
		float num = Speed * Time.deltaTime;
		base.transform.position += base.transform.forward * num;
		distanceTravelled += num;
		if (timeSinceSpawn > Lifetime || distanceTravelled > MaxDistance)
		{
			OnImpact();
		}
	}

	public virtual void Fire()
	{
		timeSinceSpawn = 0f;
		distanceTravelled = 0f;
		ClearImpact();
	}

	public void SetMaxDistance(float maxDistance)
	{
		MaxDistance = maxDistance;
	}

	public void SetLifetime(float lifetime)
	{
		Lifetime = lifetime;
	}

	public void SetTriggerImpact(EffectSystem parentEffectSystem, Collider collider, Vector3 impactPosition)
	{
		triggerImpact = true;
		this.parentEffectSystem = parentEffectSystem;
		this.impactPosition = impactPosition;
		if (collider != null)
		{
			impactPhysicsMaterial = collider.sharedMaterial;
		}
	}

	protected virtual void OnImpact()
	{
		if (triggerImpact)
		{
			if (parentEffectSystem != null)
			{
				BulletImpact e = new BulletImpact(impactPosition, -base.transform.forward, impactPhysicsMaterial);
				parentEffectSystem.TriggerInternal(e);
			}
			ClearImpact();
		}
		if (DestroyOnDeath)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void ClearImpact()
	{
		parentEffectSystem = null;
		impactPhysicsMaterial = null;
		impactPosition = Vector3.zero;
		triggerImpact = false;
	}
}
