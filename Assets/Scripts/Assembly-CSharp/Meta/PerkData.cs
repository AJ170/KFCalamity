using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	public class PerkData : MetaData
	{
		private int perk;

		private int experience;

		private int level;

		public int Perk
		{
			get
			{
				return perk;
			}
		}

		public int Experience
		{
			get
			{
				return experience;
			}
		}

		public int Level
		{
			get
			{
				return level;
			}
		}

		public PerkData(int perk)
		{
			this.perk = perk;
			experience = 0;
			level = 1;
			hasNewData = true;
		}

		public override string FileName()
		{
			return ((Perk)perk).ToString();
		}

		public override bool CanSave()
		{
			return base.CanSave() && perk != -1;
		}

		public override List<string> Serialize()
		{
			List<string> list = new List<string>();
			list.Add(SerializeData("perk", perk));
			list.Add(SerializeData("experience", experience));
			list.Add(SerializeData("level", level));
			return list;
		}

		protected override void OnDeserializeLine(string name, string value)
		{
			switch (name)
			{
			case "perk":
				perk = Convert.ToInt32(value);
				break;
			case "experience":
				experience = Convert.ToInt32(value);
				break;
			case "level":
				level = Convert.ToInt32(value);
				break;
			}
		}

		public int AddExperience(int experience)
		{
			this.experience += experience;
			hasNewData = true;
			return this.experience;
		}

		public int LevelUp()
		{
			level++;
			hasNewData = true;
			return level;
		}

		public void SetData(int level, int experience)
		{
			this.level = level;
			this.experience = experience;
			hasNewData = true;
		}

		public override void DebugPrint()
		{
			Debug.Log("perk " + perk);
			Debug.Log("experience " + experience);
			Debug.Log("level " + level);
		}
	}
}
