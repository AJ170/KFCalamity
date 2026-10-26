using System.Collections.Generic;
using UnityEngine;

public class LaserController
{
	private int playerID;

	private List<LaserSight> activeLaserSights = new List<LaserSight>();

	private bool laserInitialized;

	private List<object> laserBlocks = new List<object>();

	private bool laserActive;

	public LaserController(GameObject gameObject)
	{
		Soldier component = gameObject.GetComponent<Soldier>();
		if (component != null)
		{
			playerID = component.PlayerID;
		}
	}

	public void Update()
	{
		if (laserActive && !laserInitialized)
		{
			InitializeLasers();
		}
	}

	public void SetLaserActive(bool active, object source)
	{
		if (!active)
		{
			laserBlocks.Add(source);
		}
		else
		{
			laserBlocks.Remove(source);
		}
		laserActive = laserBlocks.Count == 0;
		foreach (LaserSight activeLaserSight in activeLaserSights)
		{
			activeLaserSight.enabled = laserActive;
		}
	}

	public void ForceLasersActive(bool active)
	{
		laserActive = active;
		foreach (LaserSight activeLaserSight in activeLaserSights)
		{
			activeLaserSight.enabled = active;
		}
		if (active && laserBlocks.Count > 0)
		{
			laserBlocks.Clear();
		}
	}

	public void OnWeaponEquipped(GameObject weaponObject)
	{
		LaserSight[] components = weaponObject.GetComponents<LaserSight>();
		LaserSight[] array = components;
		foreach (LaserSight item in array)
		{
			activeLaserSights.Add(item);
		}
		laserInitialized = false;
	}

	public void OnWeaponUnequipped()
	{
		activeLaserSights.Clear();
	}

	private void InitializeLasers()
	{
		foreach (LaserSight activeLaserSight in activeLaserSights)
		{
			activeLaserSight.SpawnLaser(playerID);
		}
		laserInitialized = true;
	}

	public void OnHeadshotAim(bool aiming)
	{
		float widthMultiplier = ((!aiming) ? 1f : 3f);
		foreach (LaserSight activeLaserSight in activeLaserSights)
		{
			activeLaserSight.SetWidthMultiplier(widthMultiplier);
		}
	}
}
