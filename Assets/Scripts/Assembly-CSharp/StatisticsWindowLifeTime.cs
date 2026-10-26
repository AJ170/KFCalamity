using System;
using System.Collections.Generic;
using Meta;
using UnityEngine;

public class StatisticsWindowLifeTime : MonoBehaviour
{
	public enum LabelType
	{
		Cash = 0,
		Damage = 1,
		Kills = 2,
		Deaths = 3,
		Syringes = 4,
		Game_Easy = 5,
		Game_Normal = 6,
		Game_Hard = 7,
		Game_Hell = 8,
		Kills_Bloat = 9,
		Kills_Clot = 10,
		Kills_Crawler = 11,
		Kills_Fleshpound = 12,
		Kills_Gorefast = 13,
		Kills_Husk = 14,
		Kills_Patriarch = 15,
		Kills_Scrake = 16,
		Kills_Siren = 17,
		Kills_Stalker = 18
	}

	[Serializable]
	public class StatisticLabel
	{
		public UILabel LabelName;

		public UILabel LabelValue;

		public uint Value;

		public string Name;

		public string DisplayName;

		public LabelType Type;
	}

	public List<StatisticLabel> Labels;

	public List<TweenAlpha> AlphaTweens;

	private void Start()
	{
		MetaManager instance = MetaManager.Instance;
		if (instance != null)
		{
			SetValueByType(LabelType.Cash, instance.LifetimeStat(LifetimeStat.CashEarned));
			SetValueByType(LabelType.Damage, instance.LifetimeStat(LifetimeStat.DamageDone));
			SetValueByType(LabelType.Deaths, instance.LifetimeStat(LifetimeStat.Deaths));
			SetValueByType(LabelType.Game_Easy, instance.LifetimeStat(LifetimeStat.EasyGamesCompleted));
			SetValueByType(LabelType.Game_Hard, instance.LifetimeStat(LifetimeStat.HardGamesCompleted));
			SetValueByType(LabelType.Game_Hell, instance.LifetimeStat(LifetimeStat.HellGamesCompleted));
			SetValueByType(LabelType.Game_Normal, instance.LifetimeStat(LifetimeStat.NormalGamesCompleted));
			SetValueByType(LabelType.Kills, instance.LifetimeStat(LifetimeStat.Kills));
			SetValueByType(LabelType.Kills_Bloat, instance.LifetimeStat(LifetimeStat.BloatsKilled));
			SetValueByType(LabelType.Kills_Clot, instance.LifetimeStat(LifetimeStat.ClotsKilled));
			SetValueByType(LabelType.Kills_Crawler, instance.LifetimeStat(LifetimeStat.CrawlersKilled));
			SetValueByType(LabelType.Kills_Fleshpound, instance.LifetimeStat(LifetimeStat.FleshpoundsKilled));
			SetValueByType(LabelType.Kills_Gorefast, instance.LifetimeStat(LifetimeStat.GorefastsKilled));
			SetValueByType(LabelType.Kills_Husk, instance.LifetimeStat(LifetimeStat.HusksKilled));
			SetValueByType(LabelType.Kills_Patriarch, instance.LifetimeStat(LifetimeStat.PatriarchsKilled));
			SetValueByType(LabelType.Kills_Scrake, instance.LifetimeStat(LifetimeStat.ScrakesKilled));
			SetValueByType(LabelType.Kills_Siren, instance.LifetimeStat(LifetimeStat.SirensKilled));
			SetValueByType(LabelType.Kills_Stalker, instance.LifetimeStat(LifetimeStat.StalkersKilled));
			SetValueByType(LabelType.Syringes, instance.LifetimeStat(LifetimeStat.SyringesUsed));
		}
	}

	private void Update()
	{
	}

	public void SetValueByType(LabelType type, uint value)
	{
		for (int i = 0; i < Labels.Count; i++)
		{
			if (Labels[i] == null || Labels[i].Type != type)
			{
				continue;
			}
			Labels[i].Value = value;
			if (type == LabelType.Damage || type == LabelType.Cash)
			{
				if (value >= 1000)
				{
					value = (uint)Mathf.CeilToInt((float)value / 1000f);
				}
				Labels[i].LabelValue.text = value + "k";
			}
			else
			{
				Labels[i].LabelValue.text = value.ToString();
			}
			Labels[i].LabelName.text = Localizer.GetString(Labels[i].DisplayName);
		}
	}

	public void TweenAlpha(float amount, float duration)
	{
		for (int i = 0; i < AlphaTweens.Count; i++)
		{
			if (AlphaTweens[i] != null)
			{
				AlphaTweens[i].to = amount;
				AlphaTweens[i].from = AlphaTweens[i].alpha;
				AlphaTweens[i].duration = duration;
				AlphaTweens[i].Play(true);
			}
		}
	}

	public void TweenOut()
	{
		DeActivate();
	}

	public void Open()
	{
		base.gameObject.SetActive(true);
	}

	private void DeActivate()
	{
		base.gameObject.SetActive(false);
	}

	private void Kill()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
