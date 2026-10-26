using System;
using System.Collections.Generic;

[Serializable]
public class PerkStatisticsDisplayData
{
	public string DisplayName;

	public string Image;

	public Perk Perk;

	public List<string> AbilityLines;

	public List<string> DescriptionLines;

	public List<string> AdvancedDescriptionLines;
}
