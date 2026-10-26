using UnityEngine;

public class EndScreenPlayerStats : MonoBehaviour
{
	public enum LabelState
	{
		None = 0,
		Name = 1,
		Kills = 2,
		Deaths = 3,
		Damage = 4,
		KillStreak = 5
	}

	public GameObject ScoreStar;

	public UILabel LabelName;

	public UILabel LabelKills;

	public UILabel LabelDeaths;

	public UILabel LabelDamage;

	public UILabel LabelKillStreak;

	public UILabel LabelKillsCount;

	public UILabel LabelDeathsCount;

	public UILabel LabelDamageCount;

	public UILabel LabelKillStreakCount;

	public EndScreen Manager;

	private TimePrint timePrint = new TimePrint();

	private LabelState state;

	private int totalKills;

	private int totalKillStreak;

	private int totalDeaths;

	private int totalDamage;

	private bool starKills;

	private bool starKillStreak;

	private bool starDeaths;

	private bool starDamage;

	private bool addInstant;

	private void Start()
	{
		LabelKills.text = Localizer.GetString("gui_endscreen_kills");
		LabelDeaths.text = Localizer.GetString("gui_endscreen_deaths");
		LabelDamage.text = Localizer.GetString("gui_endscreen_damage");
		LabelKillStreak.text = Localizer.GetString("gui_endscreen_kill_streak");
	}

	private void Update()
	{
		if (addInstant)
		{
			return;
		}
		timePrint.Update();
		float num = timePrint.GetCurrentCount();
		if (state == LabelState.Damage)
		{
			LabelDamageCount.text = num.ToString();
			if (num == (float)totalDamage)
			{
				state = LabelState.None;
				Manager.OnCounterComplete();
			}
		}
		else if (state == LabelState.Deaths)
		{
			LabelDeathsCount.text = num.ToString();
			if (num == (float)totalDeaths)
			{
				state = LabelState.None;
				Manager.OnCounterComplete();
			}
		}
		else if (state == LabelState.Kills)
		{
			LabelKillsCount.text = num.ToString();
			if (num == (float)totalKills)
			{
				state = LabelState.None;
				Manager.OnCounterComplete();
			}
		}
		else if (state == LabelState.KillStreak)
		{
			LabelKillStreakCount.text = num.ToString();
			if (num == (float)totalKillStreak)
			{
				state = LabelState.None;
				Manager.OnCounterComplete();
			}
		}
	}

	public void SetName(string name)
	{
		LabelName.text = name;
	}

	public void SetStatsInstantly()
	{
		LabelKillsCount.text = totalKills.ToString();
		LabelDamageCount.text = totalDamage.ToString();
		LabelDeathsCount.text = totalDeaths.ToString();
		LabelKillStreakCount.text = totalKillStreak.ToString();
		if (starKills)
		{
			AddStarKills();
		}
		if (starDamage)
		{
			AddStarDamage();
		}
		if (starDeaths)
		{
			AddStarDeaths();
		}
		if (starKillStreak)
		{
			AddStarKillStreak();
		}
		addInstant = true;
	}

	public void SetKills(int kills, bool giveStar)
	{
		totalKills = kills;
		starKills = giveStar;
		state = LabelState.Kills;
		timePrint.AddMessage(kills, 1.5f, false);
		if (giveStar)
		{
			Invoke("AddStarKills", 1.6f);
		}
	}

	public void SetDamage(int damage, bool giveStar)
	{
		totalDamage = damage;
		starDamage = giveStar;
		state = LabelState.Damage;
		timePrint.AddMessage(damage, 1.5f, false);
		if (giveStar)
		{
			Invoke("AddStarDamage", 1.6f);
		}
	}

	public void SetDeaths(int deaths, bool giveStar)
	{
		totalDeaths = deaths;
		starDeaths = giveStar;
		state = LabelState.Deaths;
		timePrint.AddMessage(deaths, 1.5f, false);
		if (giveStar)
		{
			Invoke("AddStarDeaths", 1.6f);
		}
	}

	public void SetKillStreak(int streak, bool giveStar)
	{
		totalKillStreak = streak;
		starKillStreak = giveStar;
		state = LabelState.KillStreak;
		timePrint.AddMessage(streak, 1.5f, false);
		if (giveStar)
		{
			Invoke("AddStarKillStreak", 1.6f);
		}
	}

	private void AddStarKills()
	{
		if (!addInstant)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, ScoreStar);
			gameObject.transform.localPosition = LabelKillsCount.transform.localPosition;
		}
	}

	private void AddStarKillStreak()
	{
		if (!addInstant)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, ScoreStar);
			gameObject.transform.localPosition = LabelKillStreakCount.transform.localPosition;
		}
	}

	private void AddStarDamage()
	{
		if (!addInstant)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, ScoreStar);
			gameObject.transform.localPosition = LabelDamageCount.transform.localPosition;
		}
	}

	private void AddStarDeaths()
	{
		if (!addInstant)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, ScoreStar);
			gameObject.transform.localPosition = LabelDeathsCount.transform.localPosition;
		}
	}
}
