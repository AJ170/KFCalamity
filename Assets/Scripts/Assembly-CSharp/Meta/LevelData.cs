using System;
using System.Collections.Generic;
using Acheivements;
using UnityEngine;

namespace Meta
{
	public class LevelData : MetaData
	{
		private string name;

		private bool beginnerCompleted;

		private bool normalCompleted;

		private bool hardCompleted;

		private bool hellCompleted;

		private bool speedBronzeAchieved;

		private bool speedSilverAchieved;

		private bool speedGoldAchieved;

		private bool zedBronzeAchieved;

		private bool zedSilverAchieved;

		private bool zedGoldAchieved;

		private bool docBronzeAchieved;

		private bool docSilverAchieved;

		private bool docGoldAchieved;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public bool BeginnerCompleted
		{
			get
			{
				return beginnerCompleted;
			}
		}

		public bool NormalCompleted
		{
			get
			{
				return normalCompleted;
			}
		}

		public bool HardCompleted
		{
			get
			{
				return hardCompleted;
			}
		}

		public bool HellCompleted
		{
			get
			{
				return hellCompleted;
			}
		}

		public bool SpeedBronzeAchieved
		{
			get
			{
				return speedBronzeAchieved;
			}
		}

		public bool SpeedSilverAchieved
		{
			get
			{
				return speedSilverAchieved;
			}
		}

		public bool SpeedGoldAchieved
		{
			get
			{
				return speedGoldAchieved;
			}
		}

		public bool ZedBronzeAchieved
		{
			get
			{
				return zedBronzeAchieved;
			}
		}

		public bool ZedSilverAchieved
		{
			get
			{
				return zedSilverAchieved;
			}
		}

		public bool ZedGoldAchieved
		{
			get
			{
				return zedGoldAchieved;
			}
		}

		public bool DocBronzeAchieved
		{
			get
			{
				return docBronzeAchieved;
			}
		}

		public bool DocSilverAchieved
		{
			get
			{
				return docSilverAchieved;
			}
		}

		public bool DocGoldAchieved
		{
			get
			{
				return docGoldAchieved;
			}
		}

		public LevelData(string name)
		{
			this.name = name;
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
			list.Add(SerializeData("beginnerCompleted", beginnerCompleted));
			list.Add(SerializeData("normalCompleted", normalCompleted));
			list.Add(SerializeData("hardCompleted", hardCompleted));
			list.Add(SerializeData("hellCompleted", hellCompleted));
			list.Add(SerializeData("speedBronzeAchieved", speedBronzeAchieved));
			list.Add(SerializeData("speedSilverAchieved", speedSilverAchieved));
			list.Add(SerializeData("speedGoldAchieved", speedGoldAchieved));
			list.Add(SerializeData("zedBronzeAchieved", zedBronzeAchieved));
			list.Add(SerializeData("zedSilverAchieved", zedSilverAchieved));
			list.Add(SerializeData("zedGoldAchieved", zedGoldAchieved));
			list.Add(SerializeData("docBronzeAchieved", docBronzeAchieved));
			list.Add(SerializeData("docSilverAchieved", docSilverAchieved));
			list.Add(SerializeData("docGoldAchieved", docGoldAchieved));
			return list;
		}

		protected override void OnDeserializeLine(string name, string value)
		{
			switch (name)
			{
			case "name":
				this.name = value;
				break;
			case "beginnerCompleted":
				beginnerCompleted = Convert.ToBoolean(value);
				break;
			case "normalCompleted":
				normalCompleted = Convert.ToBoolean(value);
				break;
			case "hardCompleted":
				hardCompleted = Convert.ToBoolean(value);
				break;
			case "hellCompleted":
				hellCompleted = Convert.ToBoolean(value);
				break;
			case "speedBronzeAchieved":
				speedBronzeAchieved = Convert.ToBoolean(value);
				break;
			case "speedSilverAchieved":
				speedSilverAchieved = Convert.ToBoolean(value);
				break;
			case "speedGoldAchieved":
				speedGoldAchieved = Convert.ToBoolean(value);
				break;
			case "zedBronzeAchieved":
				zedBronzeAchieved = Convert.ToBoolean(value);
				break;
			case "zedSilverAchieved":
				zedSilverAchieved = Convert.ToBoolean(value);
				break;
			case "zedGoldAchieved":
				zedGoldAchieved = Convert.ToBoolean(value);
				break;
			case "docBronzeAchieved":
				docBronzeAchieved = Convert.ToBoolean(value);
				break;
			case "docSilverAchieved":
				docSilverAchieved = Convert.ToBoolean(value);
				break;
			case "docGoldAchieved":
				docGoldAchieved = Convert.ToBoolean(value);
				break;
			}
		}

		public void LevelCompleted(DifficultySetting difficulty)
		{
			switch (difficulty)
			{
			case DifficultySetting.Beginner:
				beginnerCompleted = true;
				break;
			case DifficultySetting.Normal:
				normalCompleted = true;
				break;
			case DifficultySetting.Hard:
				hardCompleted = true;
				break;
			case DifficultySetting.Hell:
				hellCompleted = true;
				break;
			}
			hasNewData = true;
		}

		public void UpdateAchievement(AchievementId stat, AcheivementStatus value)
		{
			switch (value)
			{
			case AcheivementStatus.Bronze:
				switch (stat)
				{
				case AchievementId.SpeedRunner:
					speedBronzeAchieved = true;
					break;
				case AchievementId.ZedSlayer:
					zedBronzeAchieved = true;
					break;
				case AchievementId.MasterDoc:
					docBronzeAchieved = true;
					break;
				}
				break;
			case AcheivementStatus.Silver:
				switch (stat)
				{
				case AchievementId.SpeedRunner:
					speedSilverAchieved = true;
					break;
				case AchievementId.ZedSlayer:
					zedSilverAchieved = true;
					break;
				case AchievementId.MasterDoc:
					docSilverAchieved = true;
					break;
				}
				break;
			case AcheivementStatus.Gold:
				switch (stat)
				{
				case AchievementId.SpeedRunner:
					speedGoldAchieved = true;
					break;
				case AchievementId.ZedSlayer:
					zedGoldAchieved = true;
					break;
				case AchievementId.MasterDoc:
					docGoldAchieved = true;
					break;
				}
				break;
			}
			hasNewData = true;
		}

		public override void DebugPrint()
		{
			Debug.Log("name " + name);
			Debug.Log("beginnerCompleted " + beginnerCompleted);
			Debug.Log("normalCompleted " + normalCompleted);
			Debug.Log("hardCompleted " + hardCompleted);
			Debug.Log("hellCompleted " + hellCompleted);
		}
	}
}
