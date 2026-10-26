using System.Collections.Generic;
using UnityEngine;

public class AimFeedbackController
{
	private class AimParticleEffect
	{
		public GameObject Prefab;

		public GameObject Instance;

		public Transform Transform;

		public ParticleController Controller;

		private Color playerColor;

		private bool useColor;

		public AimParticleEffect(GameObject prefab)
		{
			Prefab = prefab;
			useColor = false;
		}

		public AimParticleEffect(GameObject prefab, Color playerColor)
		{
			Prefab = prefab;
			this.playerColor = playerColor;
			useColor = true;
		}

		public void CheckRespawn()
		{
			if (Instance == null && Prefab != null)
			{
				Instance = (GameObject)Object.Instantiate(Prefab);
				Instance.SetActive(false);
				Transform = Instance.transform;
				Controller = Instance.GetComponent<ParticleController>();
				if (useColor && Controller != null)
				{
					Controller.SetColor(playerColor);
				}
			}
		}

		public void Reset()
		{
			if (Instance != null)
			{
				Transform.parent = null;
				Instance.SetActive(false);
			}
		}

		public void Attach(Transform target, Vector3 offset)
		{
			CheckRespawn();
			if (Instance != null)
			{
				Transform.parent = target;
				Transform.localPosition = offset;
				Instance.SetActive(true);
				if (Controller != null)
				{
					Controller.Reset();
					Controller.Play();
				}
			}
		}

		public void Shutdown()
		{
			if (Instance != null)
			{
				Transform.parent = null;
				Object.Destroy(Instance);
				Instance = null;
				Transform = null;
				Controller = null;
			}
		}
	}

	public enum AimEffectType
	{
		None = 0,
		Self = 1,
		BodyStatic = 2,
		BodyAim = 3,
		Head = 4
	}

	private const float multipleAimOffset = 0.4f;

	private GameObject selfAimEffectPrefab;

	private GameObject targetBodyStaticAimEffectPrefab;

	private GameObject targetBodyAimEffectPrefab;

	private GameObject targetHeadAimEffectPrefab;

	private Dictionary<AimEffectType, AimParticleEffect> effects = new Dictionary<AimEffectType, AimParticleEffect>();

	private AudioClip aimClip;

	private AudioClip headAimClip;

	private float volume;

	private GameObject currentTarget;

	private AimBones targetAimBones;

	private Transform soldierTransform;

	private AimEffectType currentlyActiveEffect;

	private Color playerColorSelf = Color.red;

	private Color playerColorTarget = Color.red;

	public AimEffectType CurrentActiveEffect
	{
		get
		{
			return currentlyActiveEffect;
		}
	}

	public AimFeedbackController(GameObject soldier, GameObject selfPrefab, GameObject targetBodyStaticPrefab, GameObject targetBodyAimPrefab, GameObject targetHeadAimPrefab, AudioClip aimClip, AudioClip headAimClip, float volume)
	{
		soldierTransform = soldier.transform;
		selfAimEffectPrefab = selfPrefab;
		targetBodyStaticAimEffectPrefab = targetBodyStaticPrefab;
		targetBodyAimEffectPrefab = targetBodyAimPrefab;
		targetHeadAimEffectPrefab = targetHeadAimPrefab;
		this.aimClip = aimClip;
		this.headAimClip = headAimClip;
		this.volume = volume;
		SetupPlayerColor(soldier);
		SetupEffects();
	}

	public void Shutdown()
	{
		foreach (AimParticleEffect value in effects.Values)
		{
			value.Shutdown();
		}
	}

	private void SetupPlayerColor(GameObject soldierObject)
	{
		Soldier component = soldierObject.GetComponent<Soldier>();
		if (component != null)
		{
			int num = component.PlayerID;
			if (num < 0 || num >= 4)
			{
				Debug.Log("Invalid player ID in AimFeedbackController: " + num);
				num = 0;
			}
			playerColorSelf = GlobalHelpers.AimFeedbackSelfColorByID(num);
			playerColorTarget = GlobalHelpers.PlayerColorByID(num);
		}
	}

	private void SetupEffects()
	{
		effects.Add(AimEffectType.Self, new AimParticleEffect(selfAimEffectPrefab, playerColorSelf));
		effects.Add(AimEffectType.BodyStatic, new AimParticleEffect(targetBodyStaticAimEffectPrefab, playerColorTarget));
		effects.Add(AimEffectType.BodyAim, new AimParticleEffect(targetBodyAimEffectPrefab, playerColorTarget));
		effects.Add(AimEffectType.Head, new AimParticleEffect(targetHeadAimEffectPrefab, playerColorTarget));
	}

	public void SetTarget(GameObject target, AimBones targetAimBones)
	{
		if (currentTarget != null)
		{
			ClearTarget();
		}
		else if (currentlyActiveEffect != 0)
		{
			ClearTargetFeedback();
		}
		currentTarget = target;
		this.targetAimBones = targetAimBones;
	}

	public void ClearTarget()
	{
		currentTarget = null;
		targetAimBones = null;
		ClearTargetFeedback();
		currentlyActiveEffect = AimEffectType.None;
	}

	public void ClearTargetFeedback()
	{
		if (currentlyActiveEffect != 0)
		{
			effects[currentlyActiveEffect].Reset();
		}
	}

	public void Attach(AimEffectType type)
	{
		switch (type)
		{
		default:
			if (currentlyActiveEffect != 0)
			{
				Detach(currentlyActiveEffect);
			}
			break;
		case AimEffectType.Self:
			break;
		case AimEffectType.None:
			return;
		}
		Transform targetBone = GetTargetBone(type);
		if (targetBone != null)
		{
			Vector3 offset = GetOffset(type, targetBone);
			effects[type].Attach(targetBone, offset);
			if (type != AimEffectType.Self)
			{
				currentlyActiveEffect = type;
			}
		}
		PlayAudioEffect(type);
	}

	public void Detach(AimEffectType type)
	{
		if (type != 0)
		{
			effects[type].Reset();
			if (currentlyActiveEffect == type)
			{
				currentlyActiveEffect = AimEffectType.None;
			}
		}
	}

	private Transform GetTargetBone(AimEffectType type)
	{
		Transform result = null;
		if ((currentTarget != null && targetAimBones != null) || type == AimEffectType.Self)
		{
			switch (type)
			{
			case AimEffectType.BodyStatic:
			case AimEffectType.BodyAim:
			case AimEffectType.Head:
				result = targetAimBones.FeedbackMount;
				break;
			case AimEffectType.Self:
				result = soldierTransform;
				break;
			}
		}
		return result;
	}

	private Vector3 GetOffset(AimEffectType type, Transform target)
	{
		if (type == AimEffectType.Self)
		{
			return Vector3.zero;
		}
		Vector3 zero = Vector3.zero;
		if (target.childCount > 0)
		{
			zero.y = (float)target.childCount * 0.4f;
		}
		return zero;
	}

	private void PlayAudioEffect(AimEffectType type)
	{
		AudioClip audioClip = null;
		switch (type)
		{
		case AimEffectType.BodyStatic:
		case AimEffectType.BodyAim:
			audioClip = aimClip;
			break;
		case AimEffectType.Head:
			audioClip = headAimClip;
			break;
		}
		if (audioClip != null)
		{
			AudioManager.PlayClipAtPoint(audioClip, soldierTransform.position, volume, Time.timeScale);
		}
	}
}
