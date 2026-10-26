using EventHelpers;
using UnityEngine;

public class DebugPrintEffect : EffectBaseListener
{
	public string Text = string.Empty;

	public bool PrintTime;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(Text))
			{
				text = Text;
			}
			if (PrintTime)
			{
				text = text + " " + Time.time;
			}
			if (!string.IsNullOrEmpty(text))
			{
				MonoBehaviour.print(text);
			}
		}
	}
}
