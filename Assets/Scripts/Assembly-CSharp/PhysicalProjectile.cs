using EffectEvents;
using UnityEngine;

public class PhysicalProjectile : EffectSystem
{
	public float Speed = 10f;

	public float Lifetime = 10f;

	public float MaxDistance = 500f;

	public bool DestroyOnDeath;

	public bool UsePiercing;

	public float PierceStrength = 1f;

	public bool UseRigidbody;

	public bool ScaleSpeedByDifficulty;

	public bool TriggerHit = true;

	public bool CanTriggerHeadshot;

	public LayerMask HitLayer = 2816;

	public bool TriggerRemoteHit;

	public bool TriggerImpact = true;

	protected float timeSinceSpawn;

	protected Vector3 startPosition = Vector3.zero;

	private float totalPiercing;

	protected bool impacted;

	private bool isAimedAtHead;

	private GameObject aimTarget;

	protected new Transform transform;

	private bool defaultTriggerHit;

	private bool defaultTriggerImpact;

	protected override void Awake()
	{
		base.Awake();
		defaultTriggerHit = TriggerHit;
		defaultTriggerImpact = TriggerImpact;
		transform = base.transform;
	}

	protected override void Start()
	{
		if (ScaleSpeedByDifficulty)
		{
			Speed *= ScalingByGameSettings.ProjectileSpeedByDifficulty();
		}
	}

	protected virtual void Update()
	{
		timeSinceSpawn += Time.deltaTime;
		UpdateMovement();
		UpdateImpact();
	}

	protected virtual void UpdateMovement()
	{
		if (!UseRigidbody)
		{
			float num = Speed * Time.deltaTime;
			transform.position += transform.forward * num;
		}
	}

	private void UpdateImpact()
	{
		if (impacted)
		{
			return;
		}
		float num = Vector3.Distance(startPosition, transform.position);
		if (timeSinceSpawn > Lifetime || num > MaxDistance)
		{
			if (TriggerImpact)
			{
				TriggerImpactEvent(new Impact(transform.position, null, -transform.forward));
			}
			OnImpact(null);
		}
	}

	public virtual void Fire()
	{
		if (UseRigidbody)
		{
			base.GetComponent<Rigidbody>().velocity = transform.forward * Speed;
		}
		startPosition = transform.position;
	}

	public void Reset()
	{
		TriggerHit = defaultTriggerHit;
		TriggerImpact = defaultTriggerImpact;
		eventDelegator = null;
		timeSinceSpawn = 0f;
		totalPiercing = 0f;
		impacted = false;
		isAimedAtHead = false;
		aimTarget = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (impacted)
		{
			return;
		}
		Transform transform = other.transform;
		if (TriggerHit && transform != this.transform && HitLayerMatch(transform.gameObject.layer))
		{
			TriggerHitEvents(transform);
		}
		if (UsePiercing)
		{
			Health component = transform.GetComponent<Health>();
			if (component != null)
			{
				totalPiercing += component.PiercingResistance;
			}
			else
			{
				totalPiercing = float.MaxValue;
			}
			if (totalPiercing >= PierceStrength)
			{
				if (TriggerImpact)
				{
					Vector3 position = base.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
					TriggerImpactEvent(new Impact(position, other, -this.transform.forward));
				}
				OnImpact(other.gameObject);
			}
		}
		else
		{
			if (TriggerImpact)
			{
				Vector3 position2 = base.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
				TriggerImpactEvent(new Impact("Projectile", position2, other, -this.transform.forward));
			}
			OnImpact(other.gameObject);
		}
	}

	protected virtual void OnImpact(GameObject target)
	{
		if (!impacted)
		{
			DestroyProjectile();
			impacted = true;
		}
	}

	protected virtual void TriggerHitEvents(Transform hitTransform)
	{
		bool headshot = false;
		if (CanTriggerHeadshot && isAimedAtHead && aimTarget != null && hitTransform.gameObject == aimTarget)
		{
			headshot = true;
		}
		TriggerInternal(new Hit("Projectile", hitTransform, headshot));
		if (TriggerRemoteHit)
		{
			TriggerRemote(hitTransform.gameObject, new BeingHit(transform.root.gameObject, hitTransform, -transform.forward));
		}
	}

	protected void TriggerImpactEvent(Impact e)
	{
		TriggerInternal(e);
	}

	public void DestroyProjectile()
	{
		if (DestroyOnDeath)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void ShortCircuitProjectile()
	{
		if (base.gameObject.activeInHierarchy)
		{
			TriggerHit = false;
			TriggerImpact = false;
		}
	}

	public void SetEventDelegator(EffectEventDelegator delegator)
	{
		eventDelegator = delegator;
	}

	public void SetHeadshotTarget(GameObject aimTarget)
	{
		isAimedAtHead = true;
		this.aimTarget = aimTarget;
	}

	protected bool HitLayerMatch(int layer)
	{
		return (HitLayer.value & (1 << layer)) != 0;
	}
}
