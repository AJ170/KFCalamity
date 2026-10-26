using UnityEngine.AI;
using Calamity.Input;
using EffectEvents;
using EffectHelpers;
using UnityEngine;

public class ImpaleTargetEffect : DamageOverTimeEffect
{
	private Transform parentTransform;

	private Transform mountBone;

	private Vector3 offset = Vector3.zero;

	private float maxDuration = 1f;

	private AudioClip[] painSounds;

	private float volume = 1f;

	private EffectResistance constraintResistance;

	private SoldierInputController inputController;

	private PlayerAnimationController animationController;

	private MovementMotor motor;

	private GameObject particlePrefab;

	protected override void Awake()
	{
		base.Awake();
		repeatInterval = 0.25f;
		maxRepeats = 1;
		damagePerTick = 5;
		damageType = DamageType.Melee;
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.Impale;
	}

	protected override void OnActivated()
	{
		maxRepeats = (int)(maxDuration * (1f / repeatInterval));
		parentTransform = gameObject.transform;
		inputController = gameObject.GetComponent<SoldierInputController>();
		if (inputController != null)
		{
			inputController.SetLock(true, this);
		}
		animationController = gameObject.GetComponent<PlayerAnimationController>();
		if (animationController != null)
		{
			animationController.SetBool(PlayerAnimationTrigger.Impaled, true);
		}
		motor = gameObject.GetComponent<MovementMotor>();
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
		constraintResistance.SetTemporaryImmunity(this);
	}

	public override void Update()
	{
		base.Update();
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (base.Source != null && mountBone != null)
		{
			Vector3 position = parentTransform.position;
			Vector3 to = mountBone.TransformPoint(offset);
			to = Vector3.Lerp(position, to, 0.2f);
			parentTransform.position = to;
			Vector3 facingDirection = mountBone.position - position;
			facingDirection.y = 0f;
			facingDirection.Normalize();
			motor.SetFacingDirection(facingDirection);
		}
		else
		{
			deleted = true;
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		TriggerParticle();
		TriggerSound();
	}

	protected override void OnKill(GameObject attacker, GameObject victim)
	{
		base.OnKill(attacker, victim);
		if (base.Creator != null)
		{
			base.Creator.TriggerInternal(new Kill(victim), false);
		}
	}

	private void TriggerParticle()
	{
		if (!(particlePrefab != null))
		{
			return;
		}
		GameObject effect = EffectPool.GetEffect(particlePrefab);
		if (effect != null)
		{
			ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
			Transform transform = effect.transform;
			effect.SetActive(true);
			transform.position = parentTransform.TransformPoint(new Vector3(0f, 0f - offset.y + 0.2f, 0f));
			transform.rotation = Quaternion.LookRotation(parentTransform.forward, Vector3.up);
			if (particleSystem != null)
			{
				particleSystem.Play();
			}
		}
	}

	private void TriggerSound()
	{
		if (painSounds != null && painSounds.Length > 0)
		{
			int num = Random.Range(0, painSounds.Length);
			AudioClip audioClip = painSounds[num];
			if (audioClip != null)
			{
				AudioManager.PlayClipAtPoint(audioClip, parentTransform.position, volume, Time.timeScale);
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (inputController != null)
		{
			inputController.SetLock(false, this);
		}
		if (animationController != null)
		{
			animationController.SetBool(PlayerAnimationTrigger.Impaled, false);
		}
		constraintResistance.Restore(this);
		NavMeshHit hit;
		if (NavMesh.SamplePosition(parentTransform.position, out hit, 4f, 1))
		{
			parentTransform.position = hit.position;
		}
	}

	public void SetImpaleValues(Transform target, Vector3 offset, float duration, GameObject particlePrefab, AudioClip[] painSounds, float volume)
	{
		mountBone = target;
		this.offset = offset;
		maxDuration = duration;
		this.particlePrefab = particlePrefab;
		this.painSounds = painSounds;
		this.volume = volume;
	}
}
