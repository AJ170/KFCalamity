using UnityEngine;

public class SoldierItem
{
	private Player player;

	private GameObject soldier;

	public Player Player
	{
		get
		{
			return player;
		}
	}

	public GameObject SoldierObject
	{
		get
		{
			return soldier;
		}
	}

	public SoldierItem(Player player, GameObject soldier)
	{
		this.player = player;
		this.soldier = soldier;
	}

	public bool IsAlive()
	{
		return soldier != null;
	}
}
