using System.Collections.Generic;
using GameEvents;
using UnityEngine;

public class GameUnitManager : MonoBehaviour
{
	private static GameUnitManager instance;

	public GameObject[] UnitDatabase;

	private List<GameObject> units = new List<GameObject>();

	private Dictionary<ZedType, string> zedNames;

	private Game.Context context;

	public static GameUnitManager Instance
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

	private void Start()
	{
		SetupZedNames();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public void SetContext(Game.Context gameContext)
	{
		context = gameContext;
	}

	public GameObject CreateUnit(ZedType type, Vector3 position, Quaternion rotation)
	{
		return CreateUnit(zedNames[type], position, rotation);
	}

	public GameObject CreateUnit(string name, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = FindGameObjectInDatabase(name);
		if (gameObject != null)
		{
			GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject, position, rotation);
			gameObject2.name = gameObject.name;
			units.Add(gameObject2);
			OnUnitCreated(gameObject2);
			return gameObject2;
		}
		return null;
	}

	private void OnUnitDeath(Health health)
	{
		DestroyUnit(health.gameObject);
	}

	private void OnUnitCreated(GameObject unit)
	{
		if (unit != null)
		{
			RegisterHealthDeathSignalReceiver(unit);
			TriggerCreateUnitEvent(unit);
		}
	}

	private void RegisterHealthDeathSignalReceiver(GameObject unit)
	{
		if (unit != null)
		{
			Health component = unit.GetComponent<Health>();
			if (component != null)
			{
				component.DestroyOnDeath = false;
				component.OnDeathSignal.AddReceiver(OnUnitDeath);
			}
		}
	}

	private void TriggerCreateUnitEvent(GameObject unit)
	{
		if (unit != null && context != null)
		{
			context.EventSystem.Trigger(new CreateUnit(unit));
		}
	}

	public bool DestroyUnit(GameObject unit)
	{
		if (unit != null && units.Contains(unit))
		{
			units.Remove(unit);
			OnDestroyUnit(unit);
			Object.Destroy(unit);
			return true;
		}
		return false;
	}

	private void OnDestroyUnit(GameObject unit)
	{
		if (unit != null)
		{
			TriggerDestroyUnitEvent(unit);
		}
	}

	private void TriggerDestroyUnitEvent(GameObject unit)
	{
		if (unit != null && context != null)
		{
			context.EventSystem.Trigger(new DestroyUnit(unit));
		}
	}

	private GameObject FindGameObjectInDatabase(string name)
	{
		GameObject[] unitDatabase = UnitDatabase;
		foreach (GameObject gameObject in unitDatabase)
		{
			if (name == gameObject.name)
			{
				return gameObject;
			}
		}
		Debug.LogError("GameUnitManager:FindGameObjectInDatabase -> Failed to find '" + name + "' in database");
		return null;
	}

	public string NameByUnit(GameObject unit)
	{
		string text = unit.name;
		GameObject[] unitDatabase = UnitDatabase;
		foreach (GameObject gameObject in unitDatabase)
		{
			if (text.IndexOf(gameObject.name) >= 0)
			{
				return gameObject.name;
			}
		}
		return null;
	}

	private void SetupZedNames()
	{
		zedNames = new Dictionary<ZedType, string>();
		zedNames.Add(ZedType.Clot, "Clot");
		zedNames.Add(ZedType.Gorefast, "Gorefast");
		zedNames.Add(ZedType.Bloat, "Bloat");
		zedNames.Add(ZedType.Crawler, "Crawler");
		zedNames.Add(ZedType.Siren, "Siren");
		zedNames.Add(ZedType.Stalker, "Stalker");
		zedNames.Add(ZedType.Husk, "Husk");
		zedNames.Add(ZedType.Scrake, "Scrake");
		zedNames.Add(ZedType.Fleshpound, "Fleshpound");
		zedNames.Add(ZedType.Patriarch, "Patriarch");
	}

	public ZedType? NameToZedType(string name)
	{
		foreach (KeyValuePair<ZedType, string> zedName in zedNames)
		{
			if (zedName.Value == name)
			{
				return zedName.Key;
			}
		}
		return null;
	}
}
