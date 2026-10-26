using System;
using System.Collections.Generic;

[Serializable]
public class WeaponStatisticsDisplayData
{
	public string Name;

	public string DisplayName;

	public string Image;

	public string Damage;

	public string HeadshotDamage;

	public int Price;

	public bool CanDoHeadshot;

	public bool unlockedFromStart;

	public bool HiddenIfLocked;

	public Perk Perk;

	public List<string> DescriptionLines;
}
