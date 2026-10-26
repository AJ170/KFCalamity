using System;
using System.Collections.Generic;
using Ragdoll;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public bool Enabled = true;

	public GameObject Ragdoll;

	public GameObject AlternateRagdoll;

	public Transform Mesh;

	public bool DestroyGameObject = true;

	public bool CopyPose = true;

	public bool UseLegacyPoseCopy = true;

	public GibType GibThreshold;

	public GameObject GibPrefab;

	public Transform GibOffset;

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

	private List<AppliedRagdollEffect> appliedEffects = new List<AppliedRagdollEffect>();

	private void Start()
	{
		if (Mesh == null)
		{
			Mesh = base.transform;
		}
	}

	private void Update()
	{
		UpdateEffects();
	}

	private void OnDeath(Health health)
	{
		if (Enabled)
		{
			if (ShouldBeGibbed())
			{
				Gib();
			}
			else
			{
				EnableRagdoll();
			}
			if (DestroyGameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		Mesh = null;
		GibOffset = null;
	}

	private void EnableRagdoll()
	{
		GameObject ragdollPrefab = GetRagdollPrefab();
		if (!(ragdollPrefab != null))
		{
			return;
		}
		GameObject gameObject = EffectPool.GetEffect(ragdollPrefab);
		if (gameObject == null)
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(ragdollPrefab, base.transform.position, base.transform.rotation);
		}
		if (!(gameObject != null))
		{
			return;
		}
		Transform transform = base.transform;
		Transform transform2 = gameObject.transform;
		transform2.position = transform.position;
		transform2.rotation = transform.rotation;
		Lifetime component = gameObject.GetComponent<Lifetime>();
		if (component != null)
		{
			component.Reset();
		}
		Dissolve component2 = gameObject.GetComponent<Dissolve>();
		if (component2 != null)
		{
			component2.Reset();
		}
		RagdollTargets component3 = gameObject.GetComponent<RagdollTargets>();
		if (component3 != null)
		{
			component3.Reset();
			foreach (AppliedRagdollEffect appliedEffect in appliedEffects)
			{
				component3.ApplyEffect(appliedEffect);
			}
		}
		if (CopyPose)
		{
			if (component3 != null && !UseLegacyPoseCopy)
			{
				CopyPoseToRagdoll(component3);
			}
			else
			{
				CopyPoseRecursive(Mesh, transform2);
			}
		}
		gameObject.SetActive(true);
	}

	private void CopyPoseRecursive(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		foreach (Transform item in dst)
		{
			Transform transform2 = src.Find(item.name);
			if ((bool)transform2)
			{
				CopyPoseRecursive(transform2, item);
			}
		}
	}

	private void CopyPoseToRagdoll(RagdollTargets ragdoll)
	{
		ragdoll.transform.position = Mesh.position;
		ragdoll.transform.rotation = Mesh.rotation;
		foreach (int value in Enum.GetValues(typeof(RagdollBodyParts)))
		{
			Transform bodyPart = GetBodyPart((RagdollBodyParts)value);
			Transform bodyPart2 = ragdoll.GetBodyPart((RagdollBodyParts)value);
			if (bodyPart != null && bodyPart2 != null)
			{
				bodyPart2.position = bodyPart.position;
				bodyPart2.rotation = bodyPart.rotation;
			}
		}
	}

	private GameObject GetRagdollPrefab()
	{
		bool flag = false;
		foreach (AppliedRagdollEffect appliedEffect in appliedEffects)
		{
			if (appliedEffect.Type == RagdollEffectType.UpperBodyGib)
			{
				flag = true;
			}
		}
		if (flag && AlternateRagdoll != null)
		{
			return AlternateRagdoll;
		}
		return Ragdoll;
	}

	private Transform GetBodyPart(RagdollBodyParts part)
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

	private void UpdateEffects()
	{
		if (appliedEffects.Count <= 0)
		{
			return;
		}
		for (int num = appliedEffects.Count - 1; num >= 0; num--)
		{
			AppliedRagdollEffect appliedRagdollEffect = appliedEffects[num];
			if (appliedRagdollEffect.Lifetime != -1f)
			{
				appliedRagdollEffect.Lifetime -= Time.deltaTime;
				if (appliedRagdollEffect.Lifetime <= 0f)
				{
					appliedEffects.RemoveAt(num);
				}
			}
		}
	}

	public void ApplyEffect(AppliedRagdollEffect effect)
	{
		appliedEffects.Add(effect);
	}

	public void RemoveEffect(AppliedRagdollEffect effect)
	{
		appliedEffects.Remove(effect);
	}

	public bool ShouldBeGibbed()
	{
		if (GibPrefab == null || GibThreshold == GibType.None)
		{
			return false;
		}
		foreach (AppliedRagdollEffect appliedEffect in appliedEffects)
		{
			if (appliedEffect.Type == RagdollEffectType.GreaterGib || (appliedEffect.Type == RagdollEffectType.LesserGib && GibThreshold == GibType.Lesser))
			{
				return true;
			}
		}
		return false;
	}

	private void Gib()
	{
		if (GibOffset == null)
		{
			GibOffset = base.transform;
		}
		GameObject effect = EffectPool.GetEffect(GibPrefab);
		if (effect != null)
		{
			Transform transform = effect.transform;
			transform.position = GibOffset.position;
			effect.SetActive(true);
			ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				particleSystem.Clear();
				particleSystem.Play();
			}
		}
	}
}
