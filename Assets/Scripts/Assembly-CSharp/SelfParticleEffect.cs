using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SelfParticleEffect : EffectBaseListener
{
	public GameObject ParticlePrefab;

	public Transform Mount;

	public float Cooldown = 0.1f;

	private float cooldownCounter;

	protected override void Start()
	{
		base.Start();
		if (Mount == null)
		{
			Mount = base.transform;
		}
	}

	private void Update()
	{
		if (cooldownCounter > 0f)
		{
			cooldownCounter -= Time.deltaTime;
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((!NoCustomEvents() && !flag) || !(ParticlePrefab != null) || !(cooldownCounter <= 0f))
		{
			return;
		}
		GameObject effect = EffectPool.GetEffect(ParticlePrefab);
		if (!(effect != null))
		{
			return;
		}
		ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
		Transform transform = effect.transform;
		effect.SetActive(true);
		transform.position = Mount.position;
		if (e.Id == TriggerId.BeingHit)
		{
			BeingHit beingHit = (BeingHit)e;
			Vector3 vector = beingHit.FromDirection;
			if (vector == Vector3.zero)
			{
				vector = Mount.forward;
			}
			transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
		}
		if (particleSystem != null)
		{
			particleSystem.Play();
		}
		cooldownCounter = Cooldown;
	}
}
