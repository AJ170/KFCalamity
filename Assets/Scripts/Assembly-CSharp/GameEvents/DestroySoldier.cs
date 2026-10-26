using EventHelpers;
using UnityEngine;

namespace GameEvents
{
	public class DestroySoldier : EventBase
	{
		private GameObject soldier;

		private Player player;

		public GameObject Soldier
		{
			get
			{
				return soldier;
			}
		}

		public Player Player
		{
			get
			{
				return player;
			}
		}

		public DestroySoldier(GameObject soldier, Player player)
			: base(EventId.DestroySoldier)
		{
			this.soldier = soldier;
			this.player = player;
		}
	}
}
