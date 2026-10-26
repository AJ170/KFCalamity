using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	public class ProgressionLogs
	{
		private static ProgressionLogs instance;

		private List<PerkLevelUpLog> perkLevelUps = new List<PerkLevelUpLog>();

		private List<WeaponUnlockLog> weaponUnlocks = new List<WeaponUnlockLog>();

		private List<WeaponLevelUpLog> weaponLevelUps = new List<WeaponLevelUpLog>();

		private LevelCompletedLog? levelCompleted;

		public static ProgressionLogs Instance
		{
			get
			{
				return instance;
			}
		}

		public ProgressionLogs()
		{
			instance = this;
		}

		public void Shutdown()
		{
			instance = null;
		}

		public void Reset()
		{
			perkLevelUps.Clear();
			weaponUnlocks.Clear();
			weaponLevelUps.Clear();
			levelCompleted = null;
		}

		public void PerkLevelUp(PerkData data)
		{
			perkLevelUps.Add(new PerkLevelUpLog((Perk)data.Perk, data.Level));
		}

		public void WeaponUnlock(WeaponData data)
		{
			weaponUnlocks.Add(new WeaponUnlockLog(data.Name));
		}

		public void WeaponLevelUp(WeaponData data)
		{
			weaponLevelUps.Add(new WeaponLevelUpLog(data.Name, data.Level));
		}

		public void LevelCompleted(LevelData data, DifficultySetting difficulty)
		{
			levelCompleted = new LevelCompletedLog(data.Name, difficulty);
		}

		public List<PerkLevelUpLog> GetPerkLevelUps()
		{
			return perkLevelUps;
		}

		public List<WeaponUnlockLog> GetWeaponUnlocks()
		{
			return weaponUnlocks;
		}

		public List<WeaponLevelUpLog> GetWeaponLevelUps()
		{
			return weaponLevelUps;
		}

		public LevelCompletedLog? GetLevelCompleted()
		{
			return levelCompleted;
		}

		public void DumpToConsole()
		{
			Debug.Log("-- Perk Level Ups --");
			foreach (PerkLevelUpLog perkLevelUp in perkLevelUps)
			{
				Debug.Log(string.Format("Perk {0}, level {1}", perkLevelUp.perk.ToString(), perkLevelUp.level));
			}
			Debug.Log("-- End --");
			Debug.Log("-- Weapon Unlocks--");
			foreach (WeaponUnlockLog weaponUnlock in weaponUnlocks)
			{
				Debug.Log(string.Format("Weapon {0}", weaponUnlock.weaponName));
			}
			Debug.Log("-- End --");
			Debug.Log("-- Weapon Level Ups --");
			foreach (WeaponLevelUpLog weaponLevelUp in weaponLevelUps)
			{
				Debug.Log(string.Format("Weapon {0}, level {1}", weaponLevelUp.weaponName, weaponLevelUp.level));
			}
			Debug.Log("-- End --");
			Debug.Log("-- Level Completed --");
			if (levelCompleted.HasValue)
			{
				LevelCompletedLog value = levelCompleted.Value;
				Debug.Log(string.Format("Level {0} completed on difficulty {1}", value.levelName, value.difficulty));
			}
			Debug.Log("-- End --");
		}
	}
}
