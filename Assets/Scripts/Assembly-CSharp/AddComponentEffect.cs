using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class AddComponentEffect : EffectBaseListenerWithTarget
{
	public string ComponentName;

	protected GameObject parent;

	private GameObject Source
	{
		get
		{
			GameObject gameObject = parent;
			if (gameObject == null)
			{
				gameObject = base.transform.root.gameObject;
			}
			return gameObject;
		}
	}

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	protected override void Execute(TriggerBase e)
	{
		if (NoCustomEvents() || MatchCustomEvent(e.CustomName))
		{
			ApplyComponent(e);
		}
	}

	protected void ApplyComponent(TriggerBase e)
	{
		GameObject source = Source;
		if (!string.IsNullOrEmpty(ComponentName))
		{
			GameObject gameObject = FindTargetObject(e);
			if (gameObject != null)
			{
				EffectController componentInRoot = gameObject.GetComponentInRoot<EffectController>();
				if (componentInRoot != null)
				{
					componentInRoot.CreateEffect(ComponentName, source, effectSystem);
				}
			}
			else
			{
				Debug.LogError("AddComponentEffect:Execute -> Couldn't find target. From " + source.name);
			}
		}
		else
		{
			Debug.LogError("AddComponentEffect:Execute -> Invalid component name. From " + source.name);
		}
	}
}
