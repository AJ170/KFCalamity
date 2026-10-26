using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory : MonoBehaviour
{
	public GameObject[] WeaponDatabase;

	private static WeaponFactory instance;

	public static WeaponFactory Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private GameObject FindGameObjectInDatabase(string name)
	{
		GameObject[] weaponDatabase = WeaponDatabase;
		foreach (GameObject gameObject in weaponDatabase)
		{
			if (name == gameObject.name)
			{
				return gameObject;
			}
		}
		Debug.LogError("WeaponFactory:FindGameObjectInDatabase -> Failed to find '" + name + "' in database");
		return null;
	}

	public List<Weapon> WeaponTemplates()
	{
		List<Weapon> list = new List<Weapon>();
		GameObject[] weaponDatabase = WeaponDatabase;
		foreach (GameObject gameObject in weaponDatabase)
		{
			Weapon component = gameObject.GetComponent<Weapon>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public Weapon WeaponTemplate(string name)
	{
		GameObject gameObject = FindGameObjectInDatabase(name);
		if (gameObject != null)
		{
			Weapon component = gameObject.GetComponent<Weapon>();
			if (component == null)
			{
				Debug.LogError(string.Format("WeaponFactory:WeaponTemplate() -> Failed to find Weapon script in {0}", gameObject.ToString()));
			}
			return component;
		}
		return null;
	}

	public List<Weapon> WeaponTemplatesBySlot(WeaponSlot slot)
	{
		List<Weapon> list = new List<Weapon>();
		Weapon weapon = null;
		GameObject[] weaponDatabase = WeaponDatabase;
		foreach (GameObject gameObject in weaponDatabase)
		{
			weapon = gameObject.GetComponent<Weapon>();
			if (weapon != null && weapon.Slot == slot)
			{
				list.Add(weapon);
			}
		}
		return list;
	}

	public List<Weapon> WeaponTemplatesByPerk(Perk perk)
	{
		List<Weapon> list = new List<Weapon>();
		Weapon weapon = null;
		GameObject[] weaponDatabase = WeaponDatabase;
		foreach (GameObject gameObject in weaponDatabase)
		{
			weapon = gameObject.GetComponent<Weapon>();
			if (weapon != null && weapon.SoldierPerk == perk)
			{
				list.Add(weapon);
			}
		}
		return list;
	}

	public GameObject WeaponPrefabByName(string name)
	{
		return FindGameObjectInDatabase(name);
	}
}
