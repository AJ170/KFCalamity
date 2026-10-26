using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public abstract class AimState
{
	public enum AimStateType
	{
		Free = 0,
		Locked = 1,
		Friendly = 2
	}

	protected const float spineRotationLerpSpeed = 5f;

	protected const float spineRotationOffset = 3.85f;

	private float heightForwardCheckOffset = 5f;

	private float heightUpCheckOffset = 5f;

	private float heightDifferenceLimit = 2.5f;

	protected GameObject soldier;

	protected Transform transform;

	protected SoldierInputController inputController;

	protected MovementMotor motor;

	protected Transform spine;

	protected float spineRotation = 3.85f;

	protected float spineRotationTarget;

	protected bool inputLocked;

	public AimStateType Type { get; protected set; }

	protected AimState(AimStateType type, GameObject soldier, Transform spine)
	{
		Type = type;
		this.soldier = soldier;
		transform = soldier.transform;
		this.spine = spine;
		inputController = soldier.GetComponent<SoldierInputController>();
		motor = soldier.GetComponent<MovementMotor>();
	}

	public virtual void Update()
	{
	}

	public virtual void LateUpdate()
	{
	}

	public virtual void OnActivated()
	{
		spineRotation = 3.85f;
		spineRotationTarget = 0f;
	}

	public virtual void OnDeactivated()
	{
	}

	public virtual void Shutdown()
	{
	}

	public void LockInput(bool lockInput)
	{
		inputLocked = lockInput;
	}

	protected GameObject[] FindTargetsInDirection(Vector3 position, Vector3 direction, float angle, float range, int layer)
	{
		GameObject[] targets;
		TargetsInRange(out targets, position, range, layer);
		TargetsInArc(ref targets, position, direction, angle);
		return targets;
	}

	protected GameObject[] FindTargetsInRange(Vector3 position, float range, int layer)
	{
		GameObject[] targets;
		TargetsInRange(out targets, position, range, layer);
		return targets;
	}

	protected void TargetsInRange(out GameObject[] targets, Vector3 position, float range, int layer)
	{
		Collider[] array = Physics.OverlapSphere(position, range, layer);
		targets = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject != soldier)
			{
				targets[i] = array[i].gameObject;
			}
		}
	}

	protected void TargetsInArc(ref GameObject[] targets, Vector3 position, Vector3 direction, float angle)
	{
		float cone = Mathf.Cos(angle * ((float)Math.PI / 180f));
		for (int num = targets.Length - 1; num >= 0; num--)
		{
			GameObject gameObject = targets[num];
			if (gameObject != null)
			{
				Vector3 position2 = gameObject.transform.position;
				if (!PositionIsInArc(position, position2, direction, cone))
				{
					targets[num] = null;
				}
			}
		}
	}

	protected bool PositionIsInArc(Vector3 source, Vector3 target, Vector3 direction, float cone)
	{
		bool result = false;
		Vector3 rhs = Vector3.Normalize(target - source);
		float num = Vector3.Dot(direction, rhs);
		if (num > cone)
		{
			result = true;
		}
		return result;
	}

	protected bool PositionIsInRange(Vector3 source, Vector3 target, float range)
	{
		bool result = false;
		float num = Vector3.Distance(source, target);
		if (num <= range)
		{
			result = true;
		}
		return result;
	}

	protected GameObject ClosestTarget(IEnumerable<GameObject> targets)
	{
		Vector3 position = transform.position;
		GameObject result = null;
		float num = float.MaxValue;
		foreach (GameObject target in targets)
		{
			if (target != null)
			{
				float num2 = Vector3.Distance(position, target.transform.position);
				if (num2 < num)
				{
					result = target;
					num = num2;
				}
			}
		}
		return result;
	}

	protected GameObject TargetWithSmallestAngle(ref GameObject[] targets, Vector3 position, Vector3 direction, float angle)
	{
		GameObject result = null;
		if (targets.Length > 0)
		{
			float num = Mathf.Cos(angle * ((float)Math.PI / 180f));
			float num2 = num;
			for (int i = 0; i < targets.Length; i++)
			{
				GameObject gameObject = targets[i];
				if (gameObject != null && gameObject != soldier)
				{
					float num3 = Vector3.Dot(direction, (gameObject.transform.position - position).normalized);
					if (num3 > num2)
					{
						result = gameObject;
						num2 = num3;
					}
				}
			}
		}
		return result;
	}

	protected GameObject BestTargetInIncrementingAngles(ref GameObject[] targets, Vector3 position, Vector3 direction, float angle)
	{
		GameObject gameObject = null;
		int num = 7;
		float num2 = angle / (float)num;
		for (int i = 1; i <= num; i++)
		{
			float angle2 = (float)i * num2;
			List<GameObject> list = TargetsInArc2(ref targets, position, direction, angle2);
			if (list != null && list.Count > 0)
			{
				gameObject = ClosestTarget(list);
			}
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	private List<GameObject> TargetsInArc2(ref GameObject[] targets, Vector3 position, Vector3 direction, float angle)
	{
		List<GameObject> list = null;
		float cone = Mathf.Cos(angle * ((float)Math.PI / 180f));
		for (int num = targets.Length - 1; num >= 0; num--)
		{
			GameObject gameObject = targets[num];
			if (gameObject != null)
			{
				Vector3 position2 = gameObject.transform.position;
				if (PositionIsInArc(position, position2, direction, cone))
				{
					if (list == null)
					{
						list = new List<GameObject>();
					}
					list.Add(gameObject);
				}
			}
		}
		return list;
	}

	private void DrawCone(Vector3 position, Vector3 direction, float angle, bool found)
	{
		float num = angle * ((float)Math.PI / 180f);
		Vector3 vector = new Vector3(direction.x, direction.y, direction.z).RotateY(0f - num);
		Vector3 vector2 = new Vector3(direction.x, direction.y, direction.z).RotateY(num);
		Color color = ((!found) ? Color.blue : Color.red);
		Debug.DrawLine(position, position + vector * 10f, color, 1f);
		Debug.DrawLine(position, position + vector2 * 10f, color, 1f);
		Debug.DrawLine(position, position + direction * 10f, Color.yellow, 1f);
	}

	protected float GetGroundAimRotation()
	{
		float result = 0f;
		int layerMask = 1 << LayerMask.NameToLayer("Ground");
		Vector3 position = transform.position;
		Vector3 origin = position + heightForwardCheckOffset * transform.forward + heightUpCheckOffset * transform.up;
		RaycastHit hitInfo;
		Physics.Raycast(origin, -transform.up, out hitInfo, 2f * heightUpCheckOffset, layerMask);
		if (hitInfo.transform != null)
		{
			Vector3 targetPosition = hitInfo.point;
			if (Mathf.Abs(targetPosition.y - position.y) > heightDifferenceLimit)
			{
				targetPosition = position + transform.forward;
			}
			else
			{
				targetPosition.y -= 0.2f;
			}
			result = BoneRotationFromTarget(targetPosition);
		}
		return result;
	}

	protected float BoneRotationFromTarget(Vector3 targetPosition)
	{
		Vector3 forward = Vector3.Normalize(transform.position - targetPosition);
		float num = Quaternion.LookRotation(forward).eulerAngles.x;
		if (num > 180f)
		{
			num -= 360f;
		}
		return num;
	}
}
