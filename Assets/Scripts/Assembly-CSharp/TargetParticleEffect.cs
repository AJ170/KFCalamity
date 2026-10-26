using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TargetParticleEffect : EffectBaseListener
{
	public GameObject ParticlePrefab;

	public Vector3 Offset = Vector3.up;

	public float Cooldown = 0.1f;

	public bool AlignFromSource;

	private Transform parent;

	private float cooldownCounter;

	private void Update()
	{
		if (cooldownCounter > 0f)
		{
			cooldownCounter -= Time.deltaTime;
		}
	}

	private void OnMounted(GameObject parent)
	{
		this.parent = parent.transform;
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && ParticlePrefab != null && cooldownCounter <= 0f && e.Id == TriggerId.Hit)
		{
			Hit hit = (Hit)e;
			SpawnParticle(hit.HitTransform);
		}
	}

	public void SpawnParticle(Transform target)
	{
		if (!(target != null))
		{
			return;
		}
		GameObject effect = EffectPool.GetEffect(ParticlePrefab);
		if (effect != null)
		{
			ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
			Transform transform = effect.transform;
			effect.SetActive(true);
			Vector3 vector = (transform.position = target.position);
			if (AlignFromSource)
			{
				Vector3 vector2 = vector - parent.position;
				vector2.y = 0f;
				transform.rotation = Quaternion.LookRotation(vector2.normalized, Vector3.up);
			}
			if (particleSystem != null)
			{
				particleSystem.Stop();
				particleSystem.Play();
			}
			cooldownCounter = Cooldown;
		}
	}
}
