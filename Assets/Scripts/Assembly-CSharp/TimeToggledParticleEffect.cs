using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TimeToggledParticleEffect : EffectBaseListener
{
	public enum ProjectileBoneOffset
	{
		None = 0,
		RightMuzzle = 1,
		LeftMuzzle = 2
	}

	public string ParticleName;

	public bool Parent;

	public Vector3 Rotation;

	public Vector3 Offset;

	public Transform BoneOffset;

	public ProjectileBoneOffset PresetOffset;

	public float SpawnOffset = 0.3f;

	public ParticleDestroyMethod DestroyMethod;

	public float StopDelay;

	private float delayTimer;

	private GameObject particle;

	private bool Stopped;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			Vector3 position = base.transform.position;
			if (e.Id == TriggerId.Impact)
			{
				Impact impact = (Impact)e;
				position = impact.Position;
			}
			delayTimer = StopDelay;
			position += Offset;
			if (!particle)
			{
				SpawnParticle(position);
			}
			else if (Stopped)
			{
				particle.GetComponent<ParticleSystem>().Play();
				Stopped = false;
			}
		}
	}

	private void LocateMuzzle()
	{
		WeaponController component = base.gameObject.transform.parent.parent.GetComponent<WeaponController>();
		if (component != null)
		{
			Weapon activeWeapon = component.ActiveWeapon;
			if (activeWeapon != null)
			{
				if (PresetOffset == ProjectileBoneOffset.RightMuzzle)
				{
					BoneOffset = activeWeapon.PrimaryMuzzleBone;
				}
				else if (PresetOffset == ProjectileBoneOffset.LeftMuzzle)
				{
					BoneOffset = activeWeapon.SecondaryMuzzleBone;
				}
			}
		}
		if (BoneOffset == null)
		{
			BoneOffset = base.transform;
		}
	}

	private void Update()
	{
		if ((bool)particle)
		{
			if (delayTimer == 0f && !Stopped)
			{
				Stopped = true;
				particle.GetComponent<ParticleSystem>().Stop();
			}
			else
			{
				delayTimer = Mathf.Max(0f, delayTimer - Time.deltaTime);
			}
		}
	}

	private void SpawnParticle(Vector3 position)
	{
		ParticleManager instance = ParticleManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		Object.Destroy(particle);
		Quaternion quaternion = Quaternion.Euler(Rotation);
		particle = instance.CreateParticle(ParticleName, position, quaternion, DestroyMethod);
		if (particle != null)
		{
			if (PresetOffset != 0)
			{
				LocateMuzzle();
				Vector3 forward = BoneOffset.forward;
				Vector3 position2 = BoneOffset.position + forward * SpawnOffset;
				particle.transform.position = position2;
				particle.transform.rotation = Quaternion.LookRotation(forward);
				particle.transform.parent = BoneOffset.transform;
			}
			else if (Parent)
			{
				particle.transform.parent = base.gameObject.transform;
				particle.transform.localRotation = quaternion;
				particle.transform.localPosition = Offset;
			}
			if (particle != null)
			{
				particle.GetComponent<ParticleSystem>().Play();
				Stopped = false;
			}
		}
	}
}
