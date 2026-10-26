using EventHelpers;
using UnityEngine;

public class SelfFollowParticleEffect : EffectBaseListener
{
	public GameObject ParticlePrefab;

	public Transform Mount;

	public Vector3 MountOffset;

	public float Cooldown = 0.2f;

	private GameObject particleObject;

	private float cooldownCounter;

	protected override void Start()
	{
		base.Start();
		if (Mount == null)
		{
			Mount = base.transform;
		}
		if (ParticlePrefab != null)
		{
			particleObject = Object.Instantiate(ParticlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			particleObject.SetActive(false);
		}
		else
		{
			Transform transform = ((!base.transform.parent) ? base.transform : base.transform.parent);
			Debug.LogError(string.Format("SelfFollowParticleEffect:Start() Missing ParticlePrefab, {0} ({1})", base.name, transform.name));
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
		if (!(cooldownCounter <= 0f))
		{
			return;
		}
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && particleObject != null)
		{
			ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
			Transform transform = particleObject.transform;
			particleObject.SetActive(true);
			transform.position = Mount.position;
			transform.parent = Mount;
			transform.localPosition = MountOffset;
			if (particleSystem != null)
			{
				particleSystem.Stop();
				particleSystem.Play();
			}
			cooldownCounter = Cooldown;
		}
	}
}
