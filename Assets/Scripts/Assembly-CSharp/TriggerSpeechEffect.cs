using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TriggerSpeechEffect : EffectBaseListener
{
	public enum SpeechTriggerTarget
	{
		Parent = 0,
		Target = 1,
		Random = 2
	}

	public SpeechManager.SpeechLine Line = SpeechManager.SpeechLine.Reload;

	public SpeechTriggerTarget Target;

	public float TriggerChance = 1f;

	private GameObject parent;

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((!NoCustomEvents() && !flag) || !ShouldPlay(e))
		{
			return;
		}
		if (Target == SpeechTriggerTarget.Parent)
		{
			if (parent != null)
			{
				SpeechManager.PlaySpeech(Line, parent);
			}
		}
		else if (Target == SpeechTriggerTarget.Random)
		{
			SpeechManager.PlaySpeech(Line);
		}
		else if (Target == SpeechTriggerTarget.Target)
		{
			GameObject target = GetTarget(e);
			if (target != null)
			{
				SpeechManager.PlaySpeech(Line, target);
			}
		}
	}

	protected virtual bool ShouldPlay(TriggerBase e)
	{
		bool flag = true;
		if (flag && SpeechManager.CanPlay)
		{
			flag = TriggerChance > 0f && Random.value < TriggerChance;
		}
		return flag;
	}

	private GameObject GetTarget(TriggerBase e)
	{
		GameObject result = null;
		if (e.Id == TriggerId.Hit)
		{
			Hit hit = (Hit)e;
			result = hit.HitTransform.gameObject;
		}
		else if (e.Id == TriggerId.FriendlyAim)
		{
			FriendlyAim friendlyAim = (FriendlyAim)e;
			result = friendlyAim.Target;
		}
		else if (e.Id == TriggerId.BeingKilled)
		{
			BeingKilled beingKilled = (BeingKilled)e;
			result = beingKilled.Attacker;
		}
		else if (e.Id == TriggerId.Detected)
		{
			Detected detected = (Detected)e;
			result = detected.Target;
		}
		return result;
	}
}
