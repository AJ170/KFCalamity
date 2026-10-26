using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	public class WeaponData : MetaData
	{
		private string name;

		private int experience;

		private int level;

		public string Name
		{
			get
			{
				return name;
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

		public WeaponData(string name)
		{
			this.name = name;
			experience = 0;
			level = 1;
			hasNewData = true;
		}

		public override string FileName()
		{
			return name;
		}

		public override bool CanSave()
		{
			return base.CanSave() && !string.IsNullOrEmpty(name);
		}

		public override List<string> Serialize()
		{
			List<string> list = new List<string>();
			list.Add(SerializeData("name", name));
			list.Add(SerializeData("experience", experience));
			list.Add(SerializeData("level", level));
			return list;
		}

		protected override void OnDeserializeLine(string name, string value)
		{
			switch (name)
			{
			case "name":
				this.name = value;
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
			Debug.Log("name " + name);
			Debug.Log("experience " + experience);
			Debug.Log("level " + level);
		}
	}
}
