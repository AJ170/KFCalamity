using System.Collections.Generic;
using UnityEngine;

public abstract class LineOfSightComponent : MonoBehaviour
{
	private Transform parentTransform;

	protected List<LineOfSightComponent> visibleTargets = new List<LineOfSightComponent>();

	public Vector3 Position
	{
		get
		{
			return parentTransform.position;
		}
	}

	protected virtual void Start()
	{
		parentTransform = base.transform;
		if (LineOfSightManager.Instance != null)
		{
			Register();
		}
		else
		{
			base.enabled = false;
		}
	}

	protected virtual void OnDestroy()
	{
		if (LineOfSightManager.Instance != null)
		{
			Unregister();
		}
	}

	public bool CanSeeTarget(GameObject target)
	{
		if (base.enabled)
		{
			foreach (LineOfSightComponent visibleTarget in visibleTargets)
			{
				if (visibleTarget.gameObject == target)
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public void FilterUnseenTargets(ref List<GameObject> targets)
	{
		for (int num = targets.Count - 1; num >= 0; num--)
		{
			if (!CanSeeTarget(targets[num]))
			{
				targets.RemoveAt(num);
			}
		}
	}

	protected abstract void Register();

	protected abstract void Unregister();

	public void SetVisible(LineOfSightComponent target, bool sourceVisible)
	{
		if (sourceVisible)
		{
			if (!visibleTargets.Contains(target))
			{
				OnGainVisibility(target);
				visibleTargets.Add(target);
			}
		}
		else if (visibleTargets.Remove(target))
		{
			OnLostVisibility(target);
		}
	}

	protected virtual void OnGainVisibility(LineOfSightComponent target)
	{
	}

	protected virtual void OnLostVisibility(LineOfSightComponent target)
	{
	}
}
