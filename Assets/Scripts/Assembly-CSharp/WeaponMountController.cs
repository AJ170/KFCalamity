using UnityEngine;

public class WeaponMountController : MonoBehaviour
{
	public Transform RightWeaponBone;

	public Transform LeftWeaponBone;

	public LineOfSightTarget lineOfSightTarget;

	private GameObject rightHandMount;

	private GameObject leftHandMount;

	private void Start()
	{
		if (RightWeaponBone == null)
		{
			RightWeaponBone = base.transform;
		}
	}

	private void OnDestroy()
	{
		RightWeaponBone = null;
		LeftWeaponBone = null;
		lineOfSightTarget = null;
		rightHandMount = null;
		leftHandMount = null;
	}

	public void MountRightHand(GameObject mountObject)
	{
		if (RightWeaponBone != null)
		{
			Transform transform = mountObject.transform;
			transform.parent = RightWeaponBone;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			rightHandMount = mountObject;
			if (lineOfSightTarget != null)
			{
				lineOfSightTarget.AddRenderers(mountObject.GetComponentsInChildren<Renderer>());
			}
		}
		else
		{
			Debug.LogError(string.Format("[WeaponMountController:MountRightHand] Failed to mount object {0} on parent {1}: No right weapon bone set.", mountObject.name, base.gameObject.name));
		}
	}

	public void MountLeftHand(GameObject mountObject)
	{
		if (LeftWeaponBone != null)
		{
			Transform transform = mountObject.transform;
			transform.parent = LeftWeaponBone;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			leftHandMount = mountObject;
			if (lineOfSightTarget != null)
			{
				lineOfSightTarget.AddRenderers(mountObject.GetComponentsInChildren<Renderer>());
			}
		}
		else
		{
			Debug.LogError(string.Format("[WeaponMountController:MountLeftHand] Failed to mount object {0} on parent {1}: No Left weapon bone set.", mountObject.name, base.gameObject.name));
		}
	}

	public void ClearBothHands()
	{
		if (lineOfSightTarget != null)
		{
			if (rightHandMount != null)
			{
				lineOfSightTarget.RemoveRenderers(rightHandMount.GetComponentsInChildren<Renderer>());
			}
			if (leftHandMount != null)
			{
				lineOfSightTarget.RemoveRenderers(leftHandMount.GetComponentsInChildren<Renderer>());
			}
		}
		rightHandMount = null;
		leftHandMount = null;
	}
}
