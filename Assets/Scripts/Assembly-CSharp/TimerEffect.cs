using System.Collections;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TimerEffect : EffectBaseListener
{
	public string Name;

	public float Interval = 1f;

	public int Repeats = 1;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			StartCoroutine(StartTimer());
		}
	}

	private IEnumerator StartTimer()
	{
		if (Repeats > 0 && Interval > 0f)
		{
			effectSystem.TriggerInternal(new TimerStart(Name));
			for (int i = 0; i < Repeats; i++)
			{
				yield return new WaitForSeconds(Interval);
				effectSystem.TriggerInternal(new TimerTick(Name));
			}
			effectSystem.TriggerInternal(new TimerEnd(Name));
		}
	}
}
