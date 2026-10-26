using System.Collections.Generic;
using Ragdoll;
using UnityEngine;

public class RagdollTargets : MonoBehaviour
{
	public Transform Head;

	public Transform Chest;

	public Transform Pelvis;

	public Transform LeftOverArm;

	public Transform LeftForeArm;

	public Transform RightOverArm;

	public Transform RightForeArm;

	public Transform LeftThigh;

	public Transform LeftCalf;

	public Transform RightThigh;

	public Transform RightCalf;

	public GameObject DecapitatedHeadPrefab;

	public GameObject GibPrefab;

	public bool CanBurn = true;

	public bool CanBeCharred = true;

	public List<GameObject> DelayedActivations = new List<GameObject>();

	private List<AppliedRagdollEffect> appliedEffects = new List<AppliedRagdollEffect>();

	private bool firstUpdate = true;

	private MaterialSelector materialSelector;

	private GameObject burnParticle;

	private ParticleSystem burnParticleSystem;

	private bool isOnFire;

	private bool isCharred;

	private void Start()
	{
		foreach (GameObject delayedActivation in DelayedActivations)
		{
			delayedActivation.SetActive(true);
		}
	}

	private void FixedUpdate()
	{
		if (!firstUpdate)
		{
			return;
		}
		Rigidbody rigidbody = GetRigidbody(RagdollBodyParts.Chest);
		foreach (AppliedRagdollEffect appliedEffect in appliedEffects)
		{
			Vector3 vector = ((!(rigidbody != null)) ? (Vector3.Normalize(base.transform.position - appliedEffect.Source) * appliedEffect.Force) : (Vector3.Normalize(rigidbody.position - appliedEffect.Source) * appliedEffect.Force));
			switch (appliedEffect.Type)
			{
			case RagdollEffectType.DirectForce:
				if (rigidbody != null)
				{
					rigidbody.AddForce(vector, ForceMode.Impulse);
				}
				break;
			case RagdollEffectType.ExplosiveForce:
				if (rigidbody != null)
				{
					rigidbody.AddForceAtPosition(vector, appliedEffect.Source, ForceMode.Impulse);
				}
				break;
			case RagdollEffectType.HeadshotDecapitate:
				Decapitate(vector);
				break;
			case RagdollEffectType.HeadshotGib:
				Gib();
				break;
			case RagdollEffectType.Burning:
				SetOnFire(true);
				SetCharred(true);
				break;
			}
		}
		appliedEffects.Clear();
		firstUpdate = false;
	}

	private void OnDisable()
	{
		Reset();
	}

	public void ApplyEffect(AppliedRagdollEffect effect)
	{
		appliedEffects.Add(effect);
		firstUpdate = true;
	}

	public void SetCharred(bool charred, bool force = false)
	{
		if (materialSelector == null)
		{
			materialSelector = GetComponent<MaterialSelector>();
		}
		if (materialSelector != null && (force || charred != isCharred))
		{
			int material = (charred ? 1 : 0);
			materialSelector.SetMaterial(material);
		}
		isCharred = charred;
	}

	public void SetOnFire(bool onFire)
	{
		if (onFire && burnParticle == null && ParticleManager.Instance != null)
		{
			burnParticle = ParticleManager.Instance.CreateParticle("FxOnFireSmall", Vector3.zero, Quaternion.identity);
			burnParticleSystem = burnParticle.GetComponent<ParticleSystem>();
			Transform transform = burnParticle.transform;
			Transform bodyPart = GetBodyPart(RagdollBodyParts.Chest);
			if (bodyPart != null)
			{
				transform.parent = bodyPart;
			}
			else
			{
				transform.parent = base.transform;
			}
			transform.localPosition = Vector3.zero;
		}
		if (burnParticle != null)
		{
			burnParticle.SetActive(onFire);
			if (onFire && !isOnFire && burnParticleSystem != null)
			{
				burnParticleSystem.Clear();
			}
		}
		isOnFire = onFire;
	}

	public void Reset()
	{
		if (Head != null && !Head.gameObject.activeSelf)
		{
			Head.localScale = Vector3.one;
			Head.gameObject.SetActive(true);
		}
		SetCharred(false, true);
		SetOnFire(false);
	}

	public Transform GetBodyPart(RagdollBodyParts part)
	{
		Transform result = null;
		switch (part)
		{
		case RagdollBodyParts.Head:
			result = Head;
			break;
		case RagdollBodyParts.Chest:
			result = Chest;
			break;
		case RagdollBodyParts.Pelvis:
			result = Pelvis;
			break;
		case RagdollBodyParts.LeftOverArm:
			result = LeftOverArm;
			break;
		case RagdollBodyParts.LeftForeArm:
			result = LeftForeArm;
			break;
		case RagdollBodyParts.RightOverArm:
			result = RightOverArm;
			break;
		case RagdollBodyParts.RightForeArm:
			result = RightForeArm;
			break;
		case RagdollBodyParts.LeftThigh:
			result = LeftThigh;
			break;
		case RagdollBodyParts.LeftCalf:
			result = LeftCalf;
			break;
		case RagdollBodyParts.RightThigh:
			result = RightThigh;
			break;
		case RagdollBodyParts.RightCalf:
			result = RightCalf;
			break;
		}
		return result;
	}

	private Rigidbody GetRigidbody(RagdollBodyParts part)
	{
		Transform bodyPart = GetBodyPart(part);
		if (bodyPart != null)
		{
			return bodyPart.GetComponent<Rigidbody>();
		}
		return null;
	}

	private void Decapitate(Vector3 direction)
	{
		if (!(DecapitatedHeadPrefab != null) || !(Head != null))
		{
			return;
		}
		GameObject effect = EffectPool.GetEffect(DecapitatedHeadPrefab);
		if (effect != null)
		{
			Transform transform = effect.transform;
			transform.position = Head.position;
			transform.rotation = Head.rotation;
			effect.SetActive(true);
			Rigidbody rigidbody = effect.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.AddForce(direction / 2f, ForceMode.Impulse);
			}
			Lifetime component = effect.GetComponent<Lifetime>();
			if (component != null)
			{
				component.Reset();
			}
			HideHead();
		}
	}

	private void Gib()
	{
		if (!(GibPrefab != null) || !(Head != null))
		{
			return;
		}
		GameObject effect = EffectPool.GetEffect(GibPrefab);
		if (effect != null)
		{
			Transform transform = effect.transform;
			transform.position = Head.position;
			transform.rotation = Head.rotation;
			effect.SetActive(true);
			ParticleSystem component = effect.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Clear();
				component.Play();
			}
			HideHead();
		}
	}

	private void HideHead()
	{
		if (Head != null)
		{
			Head.localScale = Vector3.zero;
			Head.gameObject.SetActive(false);
		}
	}
}
