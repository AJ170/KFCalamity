using System.Collections.Generic;
using UnityEngine;

public class LineOfSightManager : MonoBehaviour
{
	private enum InternalState
	{
		Passive = 0,
		PreparingCheck = 1,
		PerformingCheck = 2,
		Delay = 3
	}

	private static LineOfSightManager instance;

	public LayerMask Mask;

	public int MaxChecksPerFrame = 1;

	public float DelayBetweenChecks = 0.25f;

	public Vector3 Offset;

	public bool ForceVisibleWhenNoSource;

	public bool AlwaysForceVisible;

	public bool DebugMode;

	private InternalState state;

	private bool forcingVisible;

	private float delayCounter;

	private List<LineOfSightSource> sources = new List<LineOfSightSource>();

	private List<LineOfSightTarget> targets = new List<LineOfSightTarget>();

	private Queue<LineOfSightTarget> queuedChecks = new Queue<LineOfSightTarget>();

	public static LineOfSightManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		forcingVisible = ShouldForceVisible();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void Update()
	{
		if (state == InternalState.Delay)
		{
			delayCounter -= Time.deltaTime;
			if (delayCounter <= 0f)
			{
				PrepareCheck();
			}
		}
		else if (state == InternalState.PreparingCheck)
		{
			PrepareCheck();
		}
		else if (state == InternalState.PerformingCheck)
		{
			PerformCheck();
		}
	}

	public void RegisterSource(LineOfSightSource source)
	{
		if (!sources.Contains(source))
		{
			sources.Add(source);
			UpdateForceVisibility();
			OnRegistered();
			if (forcingVisible)
			{
				SetAllTargetsVisibleForSource(source, true);
			}
		}
	}

	public void UnregisterSource(LineOfSightSource source)
	{
		if (!sources.Remove(source))
		{
			return;
		}
		UpdateForceVisibility();
		OnUnregistered();
		foreach (LineOfSightTarget target in targets)
		{
			target.SetVisible(source, false);
		}
	}

	public void RegisterTarget(LineOfSightTarget target)
	{
		if (!targets.Contains(target))
		{
			targets.Add(target);
			OnRegistered();
			if (forcingVisible)
			{
				ForceTargetVisible(target, true);
				SetVisibleForAllSources(target, true);
			}
		}
	}

	public void UnregisterTarget(LineOfSightTarget target)
	{
		if (!targets.Remove(target))
		{
			return;
		}
		OnUnregistered();
		RemoveTargetFromQueue(target);
		foreach (LineOfSightSource source in sources)
		{
			source.SetVisible(target, false);
		}
	}

	private void OnRegistered()
	{
		if (state == InternalState.Passive && !forcingVisible && sources.Count > 0 && targets.Count > 0)
		{
			state = InternalState.PreparingCheck;
		}
	}

	private void OnUnregistered()
	{
		if (state != 0 && (sources.Count == 0 || targets.Count == 0))
		{
			Reset();
		}
	}

	private void PrepareCheck()
	{
		QueueTargets();
		state = InternalState.PerformingCheck;
	}

	private void QueueTargets()
	{
		queuedChecks.Clear();
		foreach (LineOfSightTarget target in targets)
		{
			queuedChecks.Enqueue(target);
		}
	}

	private void PerformCheck()
	{
		if (queuedChecks.Count > 0)
		{
			int num = Mathf.Min(MaxChecksPerFrame, queuedChecks.Count);
			for (int i = 0; i < num; i++)
			{
				LineOfSightTarget lineOfSightTarget = queuedChecks.Dequeue();
				if (lineOfSightTarget != null && lineOfSightTarget.enabled)
				{
					CheckTarget(lineOfSightTarget);
				}
			}
		}
		else
		{
			OnCheckFinished();
		}
	}

	private void CheckTarget(LineOfSightTarget target)
	{
		Vector3 target2 = target.Position + Offset;
		foreach (LineOfSightSource source2 in sources)
		{
			Vector3 source = source2.Position + Offset;
			bool sourceVisible = CheckLineOfSight(source, target2);
			source2.SetVisible(target, sourceVisible);
			target.SetVisible(source2, sourceVisible);
		}
	}

	private bool CheckLineOfSight(Vector3 source, Vector3 target)
	{
		return !Physics.Linecast(source, target, Mask.value);
	}

	private void OnCheckFinished()
	{
		if (targets.Count > 0)
		{
			delayCounter = DelayBetweenChecks;
			state = InternalState.Delay;
		}
	}

	private void UpdateForceVisibility()
	{
		bool flag = ShouldForceVisible();
		if (flag != forcingVisible)
		{
			if (forcingVisible)
			{
				ForceAllTargetsVisible(false);
				if (sources.Count > 0 && targets.Count > 0)
				{
					state = InternalState.PreparingCheck;
				}
			}
			else
			{
				Reset();
				ForceAllTargetsVisible(true);
			}
		}
		forcingVisible = flag;
	}

	private void ForceAllTargetsVisible(bool visible)
	{
		foreach (LineOfSightTarget target in targets)
		{
			ForceTargetVisible(target, visible);
		}
	}

	private void ForceTargetVisible(LineOfSightTarget target, bool visible)
	{
		target.SetForcedVisibility(visible);
	}

	private bool ShouldForceVisible()
	{
		return AlwaysForceVisible || (ForceVisibleWhenNoSource && sources.Count == 0);
	}

	private void SetVisibleForAllSources(LineOfSightTarget target, bool visible)
	{
		foreach (LineOfSightSource source in sources)
		{
			source.SetVisible(target, visible);
			target.SetVisible(source, visible);
		}
	}

	private void SetAllTargetsVisibleForSource(LineOfSightSource source, bool visible)
	{
		foreach (LineOfSightTarget target in targets)
		{
			source.SetVisible(target, visible);
			target.SetVisible(source, visible);
		}
	}

	private void Reset()
	{
		queuedChecks.Clear();
		state = InternalState.Passive;
	}

	private void RemoveTargetFromQueue(LineOfSightTarget target)
	{
		if (!queuedChecks.Contains(target))
		{
			return;
		}
		LineOfSightTarget[] array = queuedChecks.ToArray();
		queuedChecks.Clear();
		LineOfSightTarget[] array2 = array;
		foreach (LineOfSightTarget lineOfSightTarget in array2)
		{
			if (lineOfSightTarget != target)
			{
				queuedChecks.Enqueue(lineOfSightTarget);
			}
		}
	}

	public List<LineOfSightSource> GetSources()
	{
		return sources;
	}

	private void DebugDraw(Vector3 source, Vector3 target, bool visible)
	{
		Debug.DrawLine(source, target, (!visible) ? Color.red : Color.green, DelayBetweenChecks);
	}
}
