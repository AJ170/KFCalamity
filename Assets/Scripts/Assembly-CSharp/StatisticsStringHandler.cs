public class StatisticsStringHandler
{
	public void Update()
	{
	}

	public void DeserializeString(string text)
	{
	}

	private static void FormatString(ref string value)
	{
		if (value.Contains("<br>"))
		{
			value = value.Replace("<br>", "\n");
		}
	}
}
