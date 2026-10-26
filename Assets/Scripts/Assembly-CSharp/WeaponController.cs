using UnityEngine;

public class WeaponController : MonoBehaviour
{
	protected Weapon activeWeapon;

	protected GameObject activeWeaponGameObject;

	protected GameObject activeWeaponRightHandGameObject;

	protected GameObject activeWeaponLeftHandGameObject;

	protected WeaponMountController mountController;

	private WeaponMountSignalSender mountSignalSender = new WeaponMountSignalSender();

	public Weapon ActiveWeapon
	{
		get
		{
			return activeWeapon;
		}
	}

	public GameObject ActiveWeaponGameObject
	{
		get
		{
			return activeWeaponGameObject;
		}
	}

	public WeaponMountSignalSender MountSignalSender
	{
		get
		{
			return mountSignalSender;
		}
	}

	protected virtual void Start()
	{
		mountController = GetComponent<WeaponMountController>();
	}

	public virtual bool SetActiveWeapon(GameObject weaponPrefab)
	{
		if (activeWeaponGameObject != null)
		{
			DestroyActiveWeapon();
		}
		GameObject gameObject = InstantiateWeapon(weaponPrefab);
		Weapon component = gameObject.GetComponent<Weapon>();
		if (component == null)
		{
			Object.Destroy(gameObject);
			Debug.LogError(string.Format("[WeaponController:SetActiveWeapon] Attempt to mount weapon {0} that lacks a Weapon component!", weaponPrefab.name));
			return false;
		}
		SetWeaponInstanceAsActive(component, gameObject);
		MountWeapon(gameObject);
		MountWeaponMeshes(component);
		return true;
	}

	protected virtual void DestroyActiveWeapon()
	{
		if (activeWeaponGameObject != null)
		{
			if (mountController != null)
			{
				mountController.ClearBothHands();
			}
			if (activeWeaponRightHandGameObject != null)
			{
				Object.Destroy(activeWeaponRightHandGameObject);
				activeWeaponRightHandGameObject = null;
			}
			if (activeWeaponLeftHandGameObject != null)
			{
				Object.Destroy(activeWeaponLeftHandGameObject);
				activeWeaponLeftHandGameObject = null;
			}
			Object.Destroy(activeWeaponGameObject);
			activeWeapon = null;
			activeWeaponGameObject = null;
		}
	}

	protected GameObject InstantiateWeapon(GameObject weaponPrefab)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(weaponPrefab);
		gameObject.name = weaponPrefab.name;
		return gameObject;
	}

	protected void MountWeapon(GameObject weaponInstance)
	{
		Transform transform = weaponInstance.transform;
		transform.parent = base.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		weaponInstance.BroadcastMessage("OnMounted", base.gameObject, SendMessageOptions.DontRequireReceiver);
		mountSignalSender.Send(weaponInstance);
	}

	protected void MountWeaponMeshes(Weapon weapon)
	{
		if (!(mountController != null))
		{
			return;
		}
		GameObject primaryMesh = weapon.PrimaryMesh;
		GameObject secondaryMesh = weapon.SecondaryMesh;
		WeaponOffhandModes offhandMode = weapon.OffhandMode;
		if (primaryMesh != null)
		{
			if (offhandMode == WeaponOffhandModes.MountInOffhand)
			{
				mountController.MountLeftHand(primaryMesh);
			}
			else
			{
				mountController.MountRightHand(primaryMesh);
			}
			activeWeaponRightHandGameObject = primaryMesh;
		}
		if (secondaryMesh != null)
		{
			if (offhandMode == WeaponOffhandModes.OffhandMesh || offhandMode == WeaponOffhandModes.OffhandWeapon)
			{
				mountController.MountLeftHand(secondaryMesh);
				activeWeaponLeftHandGameObject = secondaryMesh;
			}
			else
			{
				Debug.LogWarning(string.Format("[WeaponController:MountWeaponMeshes] Failed to properly mount secondary mesh for weapon {0}: Wrong offhand mode set.", weapon.Name));
			}
		}
	}

	protected void SetWeaponInstanceAsActive(Weapon weapon, GameObject weaponInstance)
	{
		activeWeapon = weapon;
		activeWeaponGameObject = weaponInstance;
	}

	public virtual bool CanAttack()
	{
		bool result = false;
		if (activeWeapon != null)
		{
			result = activeWeapon.CanAttack();
		}
		return result;
	}

	public virtual void Attack()
	{
		if (activeWeapon != null)
		{
			activeWeapon.Attack();
		}
	}
}
