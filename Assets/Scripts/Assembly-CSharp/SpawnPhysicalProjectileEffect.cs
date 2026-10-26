using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SpawnPhysicalProjectileEffect : EffectBaseListener
{
	public enum ProjectileBoneOffset
	{
		None = 0,
		RightMuzzle = 1,
		LeftMuzzle = 2
	}

	public GameObject Projectile;

	public Transform BoneOffset;

	public ProjectileBoneOffset PresetOffset;

	public float SpawnOffset = 0.3f;

	public bool CheckMuzzleClearance;

	public float MuzzleClearance = 0.1f;

	private GameObject parent;

	private static LayerMask? clearanceMask;

	protected override void Start()
	{
		base.Start();
	}

	protected virtual void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && Projectile != null)
		{
			if (BoneOffset == null)
			{
				LocateMuzzle();
			}
			Vector3 forward = BoneOffset.forward;
			Vector3 position = BoneOffset.position + forward * SpawnOffset;
			if (CheckMuzzleClearance)
			{
				position = PerformMuzzleClearanceCheck(position, forward);
			}
			if (e.Id == TriggerId.ChargeUpRelease)
			{
				FireChargedProjectile(position, forward, e);
			}
			else if (e.Id == TriggerId.Attack)
			{
				Attack attackEvent = (Attack)e;
				FireProjectile(position, forward, attackEvent);
			}
			else
			{
				FireProjectile(position, forward);
			}
		}
	}

	protected void FireProjectile(Vector3 position, Vector3 direction, Attack attackEvent = null)
	{
		GameObject effect = EffectPool.GetEffect(Projectile);
		if (!(effect != null))
		{
			return;
		}
		Transform transform = effect.transform;
		transform.position = position;
		transform.rotation = Quaternion.LookRotation(direction);
		effect.SetActive(true);
		effect.BroadcastMessage("OnMounted", base.transform.root.gameObject, SendMessageOptions.DontRequireReceiver);
		PhysicalProjectile component = effect.GetComponent<PhysicalProjectile>();
		if (component != null)
		{
			component.Reset();
			if (effectSystem != null)
			{
				component.SetEventDelegator(effectSystem.EventDelegator);
			}
			if (attackEvent != null && attackEvent.IsAiming && attackEvent.IsAimingAtHead && component.CanTriggerHeadshot)
			{
				component.SetHeadshotTarget(attackEvent.AimTarget);
			}
			component.Fire();
		}
	}

	private void FireChargedProjectile(Vector3 position, Vector3 direction, TriggerBase e)
	{
		ChargeUpRelease chargeUpRelease = (ChargeUpRelease)e;
		GameObject effect = EffectPool.GetEffect(Projectile);
		if (!(effect != null))
		{
			return;
		}
		Transform transform = effect.transform;
		transform.position = position;
		transform.rotation = Quaternion.LookRotation(direction);
		effect.SetActive(true);
		effect.BroadcastMessage("OnMounted", base.transform.root.gameObject, SendMessageOptions.DontRequireReceiver);
		ChargedProjectile component = effect.GetComponent<ChargedProjectile>();
		if (component != null)
		{
			component.Reset();
			if (effectSystem != null)
			{
				component.SetEventDelegator(effectSystem.EventDelegator);
			}
			component.Fire(chargeUpRelease.Bonus);
			return;
		}
		PhysicalProjectile component2 = effect.GetComponent<PhysicalProjectile>();
		if (component2 != null)
		{
			component2.Reset();
			if (effectSystem != null)
			{
				component2.SetEventDelegator(effectSystem.EventDelegator);
			}
			component2.Fire();
		}
	}

	protected void LocateMuzzle()
	{
		WeaponController component = parent.GetComponent<WeaponController>();
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

	protected Vector3 PerformMuzzleClearanceCheck(Vector3 position, Vector3 direction)
	{
		Vector3 origin = position + direction * (0f - MuzzleClearance);
		if (!clearanceMask.HasValue)
		{
			SetupClearanceMask();
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, direction, out hitInfo, MuzzleClearance, clearanceMask.Value.value))
		{
			float num = MuzzleClearance - hitInfo.distance;
			return position - direction * num;
		}
		return position;
	}

	private void SetupClearanceMask()
	{
		clearanceMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes semi-solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid (no Line of sight)"));
	}
}
