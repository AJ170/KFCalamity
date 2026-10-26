using System.Collections;
using EventHelpers;
using UnityEngine;

public class PhysicsArcEffect : EffectBaseListener
{
	public string Name;

	public float Delay;

	public float Height = 5f;

	public float Force = 5f;

	public GameObject PhysicsObject;

	public PhysicsArcOffset Offset;

	private WeaponMountController weaponMountController;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			StartCoroutine(PerformPhysicsArc());
		}
	}

	private void OnMounted(GameObject parent)
	{
		if (Offset == PhysicsArcOffset.RightWeaponBone || Offset == PhysicsArcOffset.LeftWeaponBone)
		{
			weaponMountController = parent.GetComponent<WeaponMountController>();
		}
	}

	private IEnumerator PerformPhysicsArc()
	{
		if (PhysicsObject != null)
		{
			if (Delay > 0f)
			{
				yield return new WaitForSeconds(Delay);
			}
			Vector3 position;
			Vector3 direction;
			if (Offset == PhysicsArcOffset.RightWeaponBone || Offset == PhysicsArcOffset.LeftWeaponBone)
			{
				Transform weaponTransform = WeaponTransform();
				position = weaponTransform.position;
				direction = base.transform.forward;
			}
			else
			{
				position = base.transform.position;
				direction = base.transform.forward;
			}
			GameObject clone = Object.Instantiate(PhysicsObject, position, Quaternion.identity) as GameObject;
			StandaloneEntity.OnCreated(clone, base.transform.root.gameObject);
			if (clone.GetComponent<Rigidbody>() != null)
			{
				clone.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, Height, 0f) + direction * Force, ForceMode.Impulse);
			}
			else
			{
				Debug.LogError(string.Format("PhysicsArcEffect:PerformPhysicsArc() {0} has no rigidbody", clone.ToString()));
			}
		}
	}

	private Transform WeaponTransform()
	{
		Transform transform = null;
		if (weaponMountController != null)
		{
			if (Offset == PhysicsArcOffset.RightWeaponBone)
			{
				transform = weaponMountController.RightWeaponBone;
			}
			else if (Offset == PhysicsArcOffset.LeftWeaponBone)
			{
				transform = weaponMountController.LeftWeaponBone;
			}
		}
		if (transform == null)
		{
			transform = base.transform;
		}
		return transform;
	}
}
