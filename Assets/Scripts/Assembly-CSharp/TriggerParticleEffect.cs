using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TriggerParticleEffect : EffectBaseListener
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

	public float Scale = 1f;

	public Transform BoneOffset;

	public ProjectileBoneOffset PresetOffset;

	public float SpawnOffset = 0.3f;

	public ParticleDestroyMethod DestroyMethod;

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
			position += Offset;
			SpawnParticle(position);
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

	private void SpawnParticle(Vector3 position)
	{
		ParticleManager instance = ParticleManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(Rotation);
		GameObject gameObject = instance.CreateParticle(ParticleName, position, quaternion, DestroyMethod);
		if (gameObject != null)
		{
			Transform transform = gameObject.transform;
			if (PresetOffset != 0)
			{
				LocateMuzzle();
				Vector3 forward = BoneOffset.forward;
				Vector3 position2 = BoneOffset.position + forward * SpawnOffset;
				transform.position = position2;
				transform.rotation = Quaternion.LookRotation(forward);
			}
			else if (Parent)
			{
				transform.parent = base.gameObject.transform;
				transform.localRotation = quaternion;
				transform.localPosition = Offset;
			}
			transform.localScale = new Vector3(Scale, Scale, Scale);
			if (gameObject != null)
			{
				gameObject.GetComponent<ParticleSystem>().Play();
			}
			if (DestroyMethod != 0)
			{
				Debug.LogError(string.Format("TriggerParticleEffect:SpawnParticle() {0} not supported", DestroyMethod));
			}
		}
	}
}
