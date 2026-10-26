using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialImpactManager : MonoBehaviour
{
	[Serializable]
	public class MaterialImpact
	{
		public PhysicMaterial Material;

		public AudioClip[] ImpactSounds;

		public GameObject[] ImpactEffects;
	}

	public MaterialImpact[] Materials;

	private static Dictionary<PhysicMaterial, MaterialImpact> impactMaterials;

	private void Awake()
	{
		impactMaterials = new Dictionary<PhysicMaterial, MaterialImpact>();
		MaterialImpact[] materials = Materials;
		foreach (MaterialImpact materialImpact in materials)
		{
			impactMaterials.Add(materialImpact.Material, materialImpact);
		}
	}

	public static AudioClip GetImpactSound(PhysicMaterial material)
	{
		AudioClip result = null;
		if (material != null && impactMaterials != null && impactMaterials.ContainsKey(material))
		{
			MaterialImpact materialImpact = impactMaterials[material];
			result = GetRandomObjectFromArray(materialImpact.ImpactSounds);
		}
		return result;
	}

	public static GameObject GetImpactEffect(PhysicMaterial material)
	{
		GameObject result = null;
		if (material != null && impactMaterials != null && impactMaterials.ContainsKey(material))
		{
			MaterialImpact materialImpact = impactMaterials[material];
			GameObject randomObjectFromArray = GetRandomObjectFromArray(materialImpact.ImpactEffects);
			if (randomObjectFromArray != null)
			{
				result = EffectPool.GetEffect(randomObjectFromArray);
			}
		}
		return result;
	}

	private static T GetRandomObjectFromArray<T>(T[] array) where T : class
	{
		T result = (T)null;
		if (array.Length > 0)
		{
			result = array[UnityEngine.Random.Range(0, array.Length)];
		}
		return result;
	}
}
