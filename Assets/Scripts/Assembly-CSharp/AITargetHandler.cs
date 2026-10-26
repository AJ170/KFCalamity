using System;
using UnityEngine;

[RequireComponent(typeof(AIController))]
public class AITargetHandler : MonoBehaviour
{
	public string[] TargetTags;

	public float SearchRadius = 1000f;

	public bool RequireLineOfSight;

	public bool RequireClearPath;

	public bool EnforceMaxRange;

	public bool SearchInArc;

	public bool EnforceArc;

	public float Arc = 135f;

	public bool FixateTargets;

	public float CurrentTargetSearchModifier = 0.65f;

	public bool CanAbandonTargets;

	public float AbandonTimeThreshold = 7f;

	public float AbandonAttemptFrequency = 0.5f;

	public float AbandonSearchMultiplier = 1.5f;

	public float AbandonChance = 0.25f;

	private new Transform transform;

	private AIController controller;

	private LineOfSightComponent lineOfSightComponent;

	private bool hasTarget;

	private GameObject currentTarget;

	private Health currentTargetHealth;

	private float timeOnTarget;

	private float abandonCooldown;

	private float cone;

	private static LayerMask clearPathLayerMask = -1;

	public GameObject Target
	{
		get
		{
			return currentTarget;
		}
	}

	public bool HasTarget
	{
		get
		{
			return currentTarget != null;
		}
	}

	public bool TargetInLineOfSight
	{
		get
		{
			return currentTarget != null && CanSeeTarget(currentTarget);
		}
	}

	public void Start()
	{
		controller = GetComponent<AIController>();
		lineOfSightComponent = GetComponent<LineOfSightComponent>();
		transform = base.transform;
		if (TargetTags.Length == 0)
		{
			TargetTags = new string[1] { "Soldier" };
		}
		CalculateCone();
		if ((int)clearPathLayerMask == -1)
		{
			clearPathLayerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes semi-solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid (no Line of sight)"));
		}
	}

	private void Update()
	{
		UpdateTarget();
	}

	public GameObject SearchForTarget()
	{
		int num = TargetTags.Length - 1;
		if (currentTarget != null)
		{
			num = GetTagIndex(currentTarget.tag);
			if (FixateTargets)
			{
				num--;
			}
		}
		GameObject foundTarget = null;
		for (int i = 0; i <= num; i++)
		{
			string text = TargetTags[i];
			if (FindTarget(out foundTarget, text))
			{
				break;
			}
		}
		if (foundTarget != null && foundTarget != currentTarget)
		{
			SetTarget(foundTarget);
		}
		return foundTarget;
	}

	private bool FindTarget(out GameObject foundTarget, string tag)
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
		int count = targets.Length;
		if ((count > 0 && RequireLineOfSight) || RequireClearPath || SearchInArc)
		{
			FilterSearchTargets(ref targets, ref count);
		}
		if (count > 0)
		{
			foundTarget = ClosestTarget(ref targets);
		}
		else
		{
			foundTarget = null;
		}
		return foundTarget != null;
	}

	private void FilterSearchTargets(ref GameObject[] targets, ref int count)
	{
		for (int i = 0; i < targets.Length; i++)
		{
			GameObject target = targets[i];
			bool flag = false;
			if (RequireLineOfSight)
			{
				flag = !CanSeeTarget(target);
			}
			if (RequireClearPath)
			{
				flag = !ClearPathToTarget(target);
			}
			if (!flag && SearchInArc)
			{
				flag = !TargetInArc(target);
			}
			if (flag)
			{
				targets[i] = null;
				count--;
			}
		}
	}

	private GameObject ClosestTarget(ref GameObject[] targets)
	{
		Vector3 position = transform.position;
		GameObject result = null;
		float num = SearchRadius * SearchRadius;
		GameObject[] array = targets;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			Vector3 vector = gameObject.transform.position - position;
			if (currentTarget != null && gameObject == currentTarget)
			{
				vector *= GetCurrentTargetSearchModifier();
			}
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < num)
			{
				Health component = gameObject.GetComponent<Health>();
				if (component == null || !component.Dead)
				{
					result = gameObject;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	private int GetTagIndex(string tag)
	{
		for (int i = 0; i < TargetTags.Length; i++)
		{
			if (TargetTags[i] == tag)
			{
				return i;
			}
		}
		return -1;
	}

	private float GetCurrentTargetSearchModifier()
	{
		if (CanAbandonTargets && AbandonTimeThreshold < timeOnTarget && abandonCooldown <= 0f)
		{
			abandonCooldown = 1f / AbandonAttemptFrequency;
			if (UnityEngine.Random.value < AbandonChance)
			{
				return AbandonSearchMultiplier;
			}
		}
		return CurrentTargetSearchModifier;
	}

	public void SetTarget(GameObject target)
	{
		if (target != null)
		{
			GameObject gameObject = currentTarget;
			currentTarget = target;
			currentTargetHealth = target.GetComponent<Health>();
			hasTarget = true;
			if (gameObject != null)
			{
				controller.TriggerAIEvent(new TargetChangedAIEvent(target, gameObject));
			}
			else
			{
				controller.TriggerAIEvent(new TargetGainedAIEvent(target));
			}
		}
		else
		{
			ClearTarget();
			controller.TriggerAIEvent(new TargetLostAIEvent());
		}
		timeOnTarget = 0f;
	}

	public void ClearTarget()
	{
		currentTarget = null;
		currentTargetHealth = null;
		hasTarget = false;
		timeOnTarget = 0f;
	}

	private void UpdateTarget()
	{
		if (hasTarget)
		{
			timeOnTarget += Time.deltaTime;
			if (CanAbandonTargets && abandonCooldown > 0f)
			{
				abandonCooldown -= Time.deltaTime;
			}
			if (currentTarget == null)
			{
				ClearTarget();
				controller.TriggerAIEvent(new TargetLostAIEvent());
			}
			else if (currentTargetHealth != null && currentTargetHealth.Dead)
			{
				GameObject target = currentTarget;
				currentTarget = null;
				controller.TriggerAIEvent(new TargetDiedAIEvent(target));
			}
			else if (EnforceLOS() || EnforceRange() || EnforceSearchArc())
			{
				ClearTarget();
				controller.TriggerAIEvent(new TargetLostAIEvent());
			}
		}
	}

	private bool EnforceLOS()
	{
		if (RequireLineOfSight)
		{
			return !CanSeeTarget(currentTarget);
		}
		return false;
	}

	private bool EnforceRange()
	{
		if (EnforceMaxRange)
		{
			float num = Vector3.Distance(transform.position, currentTarget.transform.position);
			return num > SearchRadius;
		}
		return false;
	}

	private bool EnforceSearchArc()
	{
		if (EnforceArc)
		{
			return !TargetInArc(currentTarget);
		}
		return false;
	}

	private bool CanSeeTarget(GameObject target)
	{
		return lineOfSightComponent != null && lineOfSightComponent.CanSeeTarget(target);
	}

	private bool ClearPathToTarget(GameObject target)
	{
		return !Physics.Linecast(transform.position + Vector3.up * 1.5f, target.transform.position + Vector3.up * 1.5f, clearPathLayerMask);
	}

	private bool TargetInArc(GameObject target)
	{
		Vector3 position = target.transform.position;
		Vector3 rhs = Vector3.Normalize(position - transform.position);
		float num = Vector3.Dot(transform.forward, rhs);
		return num > cone;
	}

	private void CalculateCone()
	{
		cone = Mathf.Cos(Arc / 2f * ((float)Math.PI / 180f));
	}

	public void SetArc(float arc)
	{
		Arc = arc;
		CalculateCone();
	}
}
