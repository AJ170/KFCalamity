using System.Collections.Generic;
using ActorEvents;
using UnityEngine;

public class ObscuredUnitsManager : MonoBehaviour
{
	private class UnitInfo
	{
		public GameObject Unit;

		public ActorEventController EventController;

		public bool Obscured;

		public UnitInfo(GameObject unit)
		{
			Unit = unit;
			EventController = Unit.GetComponent<ActorEventController>();
			Obscured = false;
		}
	}

	private static ObscuredUnitsManager instance;

	public Transform Observer;

	public int UpdatesPerSecond = 10;

	public Vector3 UnitOffset = new Vector3(0f, 0.85f, 0f);

	private int layerMask = -1;

	private List<UnitInfo> units = new List<UnitInfo>();

	private float timer;

	private void Awake()
	{
		instance = this;
		layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid (no Line of sight)")) | (1 << LayerMask.NameToLayer("BlocksAllButBullets-solid"));
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
		units.Clear();
	}

	private void FindCamera()
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (gameObject != null)
		{
			Observer = gameObject.transform;
		}
	}

	private void Update()
	{
		if (Observer == null)
		{
			FindCamera();
		}
		if (Observer != null)
		{
			timer = Mathf.Max(timer - Time.deltaTime, 0f);
			if (timer == 0f)
			{
				timer = 1f / (float)UpdatesPerSecond;
				UpdateUnitStatus();
			}
		}
	}

	private void UpdateUnitStatus()
	{
		UnitInfo unitInfo = null;
		for (int i = 0; i < units.Count; i++)
		{
			unitInfo = units[i];
			UpdateClearViewFromObserver(unitInfo);
		}
	}

	private void UpdateClearViewFromObserver(UnitInfo to)
	{
		if (Observer != null)
		{
			Vector3 position = Observer.position;
			Vector3 vector = to.Unit.transform.position + UnitOffset;
			Vector3 normalized = (vector - position).normalized;
			float distance = Vector3.Distance(position, vector);
			bool obscured = to.Obscured;
			bool flag = Physics.Raycast(position, normalized, distance, layerMask);
			if (obscured != flag)
			{
				to.Obscured = flag;
				OnUnitChangeObscurity(to);
			}
		}
	}

	private void OnUnitChangeObscurity(UnitInfo info)
	{
		ActorEventController eventController = info.EventController;
		if (eventController != null)
		{
			eventController.Trigger(new ObscurityChanged(info.Obscured));
		}
	}

	public static void RegisterUnit(GameObject unit)
	{
		if (instance != null)
		{
			instance.AddUnit(unit);
		}
	}

	public static void UnregisterUnit(GameObject unit)
	{
		if (instance != null)
		{
			instance.RemoveUnit(unit);
		}
	}

	private void AddUnit(GameObject unit)
	{
		if (unit != null)
		{
			units.Add(new UnitInfo(unit));
		}
	}

	private void RemoveUnit(GameObject unit)
	{
		UnitInfo unitInfo = UnitInfoByUnit(unit);
		if (unitInfo != null)
		{
			units.Remove(unitInfo);
		}
	}

	private UnitInfo UnitInfoByUnit(GameObject unit)
	{
		UnitInfo result = null;
		foreach (UnitInfo unit2 in units)
		{
			if (unit2.Unit == unit)
			{
				result = unit2;
				break;
			}
		}
		return result;
	}

	public bool IsUnitObscured(GameObject unit)
	{
		UnitInfo unitInfo = UnitInfoByUnit(unit);
		if (unitInfo != null)
		{
			return unitInfo.Obscured;
		}
		return false;
	}
}
