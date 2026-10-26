using EventHelpers;
using UnityEngine;

public class AddImpaleTargetEffect : AddComponentEffect
{
	public enum BoneOffset
	{
		None = 0,
		Parent = 1,
		RightWeapon = 2,
		LeftWeapon = 3,
		RightMuzzle = 4,
		LeftMuzzle = 5
	}

	public BoneOffset MountTarget;

	public Vector3 Offset = Vector3.zero;

	public float Duration = 1f;

	public GameObject BloodPrefab;

	public AudioClip[] PainSounds;

	public float Volume = 1f;

	private Transform parentTransform;

	private GameObject Source
	{
		get
		{
			GameObject gameObject = parent;
			if (gameObject == null)
			{
				gameObject = base.transform.root.gameObject;
			}
			return gameObject;
		}
	}

	private void OnMounted(GameObject parent)
	{
		base.parent = parent;
		parentTransform = parent.transform;
	}

	protected override void Execute(TriggerBase e)
	{
		if (!NoCustomEvents() && !MatchCustomEvent(e.CustomName))
		{
			return;
		}
		GameObject source = Source;
		if (!string.IsNullOrEmpty(ComponentName))
		{
			GameObject gameObject = FindTargetObject(e);
			if (gameObject != null)
			{
				EffectController componentInRoot = gameObject.GetComponentInRoot<EffectController>();
				if (!(componentInRoot != null))
				{
					return;
				}
				EffectBaseTarget effectBaseTarget = componentInRoot.CreateEffect(ComponentName, source, effectSystem);
				if (effectBaseTarget != null)
				{
					ImpaleTargetEffect impaleTargetEffect = effectBaseTarget as ImpaleTargetEffect;
					if (impaleTargetEffect != null)
					{
						impaleTargetEffect.SetImpaleValues(GetMountParent(), Offset, Duration, BloodPrefab, PainSounds, Volume);
					}
				}
			}
			else
			{
				Debug.LogError("AddComponentEffect:Execute -> Couldn't find target. From " + source.name);
			}
		}
		else
		{
			Debug.LogError("AddComponentEffect:Execute -> Invalid component name. From " + source.name);
		}
	}

	public Transform GetMountParent()
	{
		Transform transform = null;
		switch (MountTarget)
		{
		case BoneOffset.None:
			transform = base.transform;
			break;
		case BoneOffset.Parent:
			transform = parentTransform;
			break;
		case BoneOffset.RightMuzzle:
		case BoneOffset.LeftMuzzle:
		{
			Weapon weapon = GetWeapon();
			if (weapon != null)
			{
				transform = ((MountTarget != BoneOffset.RightMuzzle) ? weapon.SecondaryMuzzleBone : weapon.PrimaryMuzzleBone);
			}
			break;
		}
		case BoneOffset.RightWeapon:
		case BoneOffset.LeftWeapon:
		{
			WeaponMountController mountController = GetMountController();
			if (mountController != null)
			{
				transform = ((MountTarget != BoneOffset.RightWeapon) ? mountController.LeftWeaponBone : mountController.RightWeaponBone);
			}
			break;
		}
		default:
			transform = base.transform;
			break;
		}
		if (transform == null)
		{
			transform = base.transform;
		}
		return transform;
	}

	private Weapon GetWeapon()
	{
		if (parent != null)
		{
			WeaponController component = parent.GetComponent<WeaponController>();
			if (component != null)
			{
				return component.ActiveWeapon;
			}
		}
		return null;
	}

	private WeaponMountController GetMountController()
	{
		if (parent != null)
		{
			return parent.GetComponent<WeaponMountController>();
		}
		return null;
	}
}
