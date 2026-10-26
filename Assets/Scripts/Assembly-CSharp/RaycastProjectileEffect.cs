using System.Collections;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class RaycastProjectileEffect : EffectBaseListener
{
	public string Name;

	public float Delay;

	public float Range = 10f;

	public Transform BoneOffset;

	public LayerMask Layer;

	public LayerMask HitLayer = 2816;

	public bool TriggerProjectile = true;

	public bool TriggerImpact = true;

	public bool TriggerRemoteHit = true;

	protected Transform myTransform;

	protected override void Start()
	{
		base.Start();
		myTransform = base.transform;
		if (BoneOffset == null)
		{
			SetMuzzle();
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			Attack attackEvent = null;
			if (e.Id == TriggerId.Attack)
			{
				attackEvent = (Attack)e;
			}
			if (Delay > 0f)
			{
				StartCoroutine(WaitForDelayBeforeFire(attackEvent));
			}
			else
			{
				PerformRaycast(attackEvent);
			}
		}
	}

	private IEnumerator WaitForDelayBeforeFire(Attack attackEvent)
	{
		yield return new WaitForSeconds(Delay);
		PerformRaycast(attackEvent);
	}

	protected virtual void PerformRaycast(Attack attackEvent)
	{
		Vector3 position = BoneOffset.position;
		Vector3 forward = BoneOffset.forward;
		FireRaycast(position, forward, attackEvent);
	}

	protected void FireRaycast(Vector3 position, Vector3 direction, Attack attackEvent)
	{
		RaycastHit hitInfo;
		Physics.Raycast(position, direction, out hitInfo, Range, Layer.value);
		Transform transform = hitInfo.transform;
		if (transform != null)
		{
			if (transform != myTransform && HitLayerMatch(transform.gameObject.layer))
			{
				TriggerHitEvents(transform, direction, attackEvent);
			}
			TriggerProjectileEvent(direction, hitInfo.point, hitInfo.collider);
			TriggerImpactEvent(new Impact(Name, hitInfo.point, hitInfo.collider, -direction));
		}
		else
		{
			TriggerProjectileEvent(direction, position + direction * Range);
			TriggerImpactEvent(new Impact(Name, position + direction * Range));
		}
	}

	protected void TriggerHitEvents(Transform hitTransform, Vector3 direction, Attack attackEvent)
	{
		if (attackEvent != null && attackEvent.IsAiming && attackEvent.IsAimingAtHead && attackEvent.AimTarget != null && attackEvent.AimTarget.transform == hitTransform)
		{
			effectSystem.TriggerInternal(new Hit(Name, hitTransform, true));
		}
		else
		{
			effectSystem.TriggerInternal(new Hit(Name, hitTransform));
		}
		if (effectSystem.SystemType == EffectSystemType.Weapon)
		{
			effectSystem.TriggerGlobal(new WeaponHit(hitTransform));
		}
		if (TriggerRemoteHit)
		{
			effectSystem.TriggerRemote(hitTransform.gameObject, new BeingHit(myTransform.root.gameObject, hitTransform, -direction));
		}
	}

	protected void TriggerImpactEvent(Impact e)
	{
		if (TriggerImpact)
		{
			effectSystem.TriggerInternal(e);
		}
	}

	protected void TriggerProjectileEvent(Vector3 direction, Vector3 target, Collider collider = null)
	{
		if (TriggerProjectile)
		{
			effectSystem.TriggerInternal(new Projectile(BoneOffset, direction, target, collider));
		}
	}

	protected virtual void SetMuzzle()
	{
		BoneOffset = myTransform;
	}

	protected bool HitLayerMatch(int layer)
	{
		return (HitLayer.value & (1 << layer)) != 0;
	}
}
