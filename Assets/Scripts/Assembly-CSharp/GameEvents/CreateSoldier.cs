using EventHelpers;
using UnityEngine;

namespace GameEvents
{
	public class CreateSoldier : EventBase
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

		public CreateSoldier(GameObject soldier, Player player)
			: base(EventId.CreateSoldier)
		{
			this.soldier = soldier;
			this.player = player;
		}
	}
}
