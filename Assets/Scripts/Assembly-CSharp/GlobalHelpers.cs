using UnityEngine.AI;
using System;
using System.Collections.Generic;
using EffectHelpers;
using UnityEngine;

public struct GlobalHelpers
{
	public static List<Vector3> CircleVectors(Vector3 center, int numVectors, float radius)
	{
		List<Vector3> list = new List<Vector3>();
		if (numVectors <= 1)
		{
			list.Add(center);
			return list;
		}
		float num = (float)Math.PI * 2f / (float)numVectors;
		float num2 = num * 0.5f;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < numVectors; i++)
		{
			zero = new Vector3(center.x, center.y, center.z);
			num2 += num;
			zero.x += Mathf.Cos(num2) * radius;
			zero.z += Mathf.Sin(num2) * radius;
			list.Add(zero);
		}
		return list;
	}

	public static bool ValidNavMeshPosition(Vector3 position, float radius = 0.5f)
	{
		NavMeshHit hit;
		return NavMesh.SamplePosition(position, out hit, radius, 1);
	}

	public static Vector3 SampleNavMeshPosition(Vector3 position, float radius = 0.5f)
	{
		Vector3 newPos;
		SampleNavMeshPosition(position, out newPos, radius);
		return newPos;
	}

	public static bool SampleNavMeshPosition(Vector3 position, out Vector3 newPos, float radius = 0.5f)
	{
		newPos = position;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(position, out hit, radius, 1))
		{
			newPos = hit.position;
		}
		return hit.hit;
	}

	public static CallbackItem<Health> AddDeathReceiver(GameObject go, Action<Health> callback)
	{
		if (go != null && callback != null)
		{
			Health componentInRoot = go.GetComponentInRoot<Health>();
			if (componentInRoot != null)
			{
				return componentInRoot.OnDeathSignal.AddReceiver(callback);
			}
		}
		return null;
	}

	public static CallbackItem<Health, int, int, float> AddDamageTakenReceiver(GameObject go, Action<Health, int, int, float> callback)
	{
		if (go != null && callback != null)
		{
			Health componentInRoot = go.GetComponentInRoot<Health>();
			if (componentInRoot != null)
			{
				return componentInRoot.OnDamageTakenSignal.AddReceiver(callback);
			}
		}
		return null;
	}

	public static bool IsInLayerMask(GameObject obj, LayerMask mask)
	{
		return (mask.value & (1 << obj.layer)) > 0;
	}

	public static bool IsInBitMask(int mask, int bit)
	{
		return (mask & bit) > 0;
	}

	public static bool RegisterEffectEventListener(GameObject go, IEffectTriggerListener listener)
	{
		EffectEventDelegator componentInRoot = go.GetComponentInRoot<EffectEventDelegator>();
		if (componentInRoot != null)
		{
			componentInRoot.RegisterListener(listener);
		}
		return componentInRoot != null;
	}

	public static CallbackItem<GameObject> AddWeaponMountReceiver(GameObject go, Action<GameObject> callback)
	{
		if (go != null && callback != null)
		{
			WeaponController componentInRoot = go.GetComponentInRoot<WeaponController>();
			if (componentInRoot != null)
			{
				return componentInRoot.MountSignalSender.AddReceiver(callback);
			}
		}
		return null;
	}

	public static List<MaterialData> FindMaterials<T>(Transform transform) where T : Renderer
	{
		List<MaterialData> list = new List<MaterialData>();
		Renderer[] componentsInChildren = transform.GetComponentsInChildren<T>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			T val = (T)array[i];
			Material material = val.material;
			if (material != null)
			{
				list.Add(new MaterialData(material));
			}
		}
		return list;
	}

	public static bool RegisterAbilityCritertion(GameObject go, IAbilityCriterion criterion)
	{
		if (go != null && criterion != null)
		{
			Ability componentInParents = go.GetComponentInParents<Ability>();
			if (componentInParents != null)
			{
				componentInParents.RegisterCriterion(criterion);
			}
		}
		return false;
	}

	public static IMetaDamageBonuses FindMetaDamageBonusesUpwards(GameObject go, int iterations = 3)
	{
		Component[] array = null;
		int num = 0;
		Transform transform = go.transform;
		while (transform != null && num < iterations)
		{
			array = transform.gameObject.GetComponents<Component>();
			Component[] array2 = array;
			foreach (Component component in array2)
			{
				if (component is IMetaDamageBonuses)
				{
					return component as IMetaDamageBonuses;
				}
			}
			transform = transform.transform.parent;
			num++;
		}
		return null;
	}

	public static Color PlayerColorByID(int playerID)
	{
		Color result = Color.red;
		switch (playerID)
		{
		case 0:
			result = Color.red;
			break;
		case 1:
			result = Color.blue;
			break;
		case 2:
			result = Color.green;
			break;
		case 3:
			result = Color.magenta;
			break;
		}
		return result;
	}

	public static Color AimFeedbackSelfColorByID(int playerID)
	{
		Color result = Color.red;
		switch (playerID)
		{
		case 0:
			result = new Color(1f, 0.204f, 0.114f);
			break;
		case 1:
			result = new Color(0f, 0.49f, 1f);
			break;
		case 2:
			result = new Color(0.447f, 1f, 0f);
			break;
		case 3:
			result = new Color(1f, 0.173f, 0.55f);
			break;
		}
		return result;
	}
}
