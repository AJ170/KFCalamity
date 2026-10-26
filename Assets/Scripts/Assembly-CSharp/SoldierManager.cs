using System.Collections.Generic;
using Calamity.Input;
using EventHelpers;
using GameEvents;
using UnityEngine;

public class SoldierManager
{
	private static SoldierManager instance;

	private EventSystem eventSystem;

	private List<SoldierItem> soldiers = new List<SoldierItem>();

	public static SoldierManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int NumAliveSoldiers
	{
		get
		{
			return soldiers.Count;
		}
	}

	public SoldierManager(EventSystem eventSystem)
	{
		instance = this;
		this.eventSystem = eventSystem;
		RegisterEvents();
	}

	public void RegisterEvents()
	{
		eventSystem.Subscribe(EventId.CreateSoldier, OnCreateSoldier, this);
		eventSystem.Subscribe(EventId.DestroyUnit, OnDestroyUnit, this);
	}

	private void OnCreateSoldier(EventBase e)
	{
		CreateSoldier createSoldier = (CreateSoldier)e;
		AddSoldier(createSoldier.Player, createSoldier.Soldier);
	}

	private void AddSoldier(Player player, GameObject soldierObject)
	{
		soldiers.Add(new SoldierItem(player, soldierObject));
	}

	private void OnDestroyUnit(EventBase e)
	{
		DestroyUnit destroyUnit = (DestroyUnit)e;
		SoldierItem soldierItem = SoldierItemByObject(destroyUnit.UnitObject);
		if (soldierItem != null)
		{
			RemoveSoldier(soldierItem);
		}
	}

	private void RemoveSoldier(SoldierItem soldier)
	{
		soldiers.Remove(soldier);
		eventSystem.Trigger(new DestroySoldier(soldier.SoldierObject, soldier.Player));
		if (NumAliveSoldiers == 0)
		{
			eventSystem.Trigger(new AllSoldiersKilled());
		}
	}

	public SoldierItem SoldierItemByObject(GameObject unitObject)
	{
		foreach (SoldierItem soldier in soldiers)
		{
			if (soldier.SoldierObject == unitObject)
			{
				return soldier;
			}
		}
		return null;
	}

	public List<SoldierItem> GetAliveSoldiers()
	{
		return soldiers;
	}

	public Vector3 RandomSoldierPosition()
	{
		if (NumAliveSoldiers > 0)
		{
			SoldierItem soldierItem = soldiers[Random.Range(0, NumAliveSoldiers)];
			return soldierItem.SoldierObject.transform.position;
		}
		return Vector3.zero;
	}

	public void Clear()
	{
		soldiers.Clear();
	}

	public void SetLockedInput(bool locked, object source, bool exclusive = false)
	{
		foreach (SoldierItem soldier in soldiers)
		{
			SoldierInputController component = soldier.SoldierObject.GetComponent<SoldierInputController>();
			if (component != null && (!exclusive || (exclusive && !component.HasMovementLock(source))))
			{
				component.SetLock(locked, source);
			}
		}
	}

	public void SetInvulnerable(bool invulnerable)
	{
		Health health = null;
		foreach (SoldierItem soldier in soldiers)
		{
			health = soldier.SoldierObject.GetComponent<Health>();
			if (health != null)
			{
				health.Invulnerable = invulnerable;
			}
		}
	}

	public static bool ContainsPerk(Perk perk)
	{
		if (instance != null)
		{
			Soldier soldier = null;
			foreach (SoldierItem soldier2 in instance.soldiers)
			{
				soldier = soldier2.SoldierObject.GetComponent<Soldier>();
				if (soldier != null && soldier.Perk == perk)
				{
					return true;
				}
			}
		}
		return false;
	}
}
