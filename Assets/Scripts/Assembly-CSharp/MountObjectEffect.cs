using EventHelpers;
using UnityEngine;

public class MountObjectEffect : EffectBaseListener
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

	public Transform MountObject;

	public BoneOffset MountBone = BoneOffset.Parent;

	public Vector3 Offset = Vector3.zero;

	public float Duration = 1f;

	private GameObject parent;

	private Transform parentTransform;

	private float counter;

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
		parentTransform = parent.transform;
	}

	private void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				MountObject.parent = base.transform;
				MountObject.gameObject.SetActive(false);
			}
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && MountObject != null)
		{
			MountObject.parent = GetMountParent();
			MountObject.localPosition = Offset;
			MountObject.localRotation = Quaternion.identity;
			MountObject.gameObject.SetActive(true);
			MountObject.SendMessage("OnMounted", parent, SendMessageOptions.DontRequireReceiver);
			counter = Duration;
		}
	}

	public Transform GetMountParent()
	{
		Transform transform = null;
		switch (MountBone)
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
				transform = ((MountBone != BoneOffset.RightMuzzle) ? weapon.SecondaryMuzzleBone : weapon.PrimaryMuzzleBone);
			}
			break;
		}
		case BoneOffset.RightWeapon:
		case BoneOffset.LeftWeapon:
		{
			WeaponMountController mountController = GetMountController();
			if (mountController != null)
			{
				transform = ((MountBone != BoneOffset.RightWeapon) ? mountController.LeftWeaponBone : mountController.RightWeaponBone);
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
