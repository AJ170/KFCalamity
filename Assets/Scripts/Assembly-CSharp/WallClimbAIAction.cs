using UnityEngine.AI;
using AIActionTransitions;
using UnityEngine;

public class WallClimbAIAction : AIAction
{
	private enum WallClimbState
	{
		Inactive = 0,
		BottomMove = 1,
		Climbing = 2,
		TopMove = 3,
		Done = 4
	}

	private OffMeshLinkData linkData;

	private Vector3 startPosition;

	private Vector3 endPosition;

	private NavMeshAgent agent;

	private AIMovementMotor motor;

	private Transform transform;

	private WallClimbState climbState;

	private Vector3 moveDirection = Vector3.zero;

	private Vector3 lastPosition = Vector3.zero;

	private float moveDistance;

	public WallClimbAIAction(NavMeshAgent agent, OffMeshLinkData linkData)
		: this(agent, linkData, null)
	{
	}

	public WallClimbAIAction(NavMeshAgent agent, OffMeshLinkData linkData, AIWallClimbMotion climbMotion)
	{
		this.agent = agent;
		this.linkData = linkData;
		startPosition = linkData.startPos;
		endPosition = linkData.endPos;
	}

	public override void OnStart()
	{
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			agent.Stop(true);
			motor.StartMotion();
			transform = controller.transform;
			SetupBottomMove();
			lastPosition = transform.position;
			base.OnStart();
		}
		else
		{
			Debug.LogWarning("JumpAIAction failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
	}

	public override Transition Update()
	{
		if (climbState == WallClimbState.Climbing)
		{
			UpdateClimb();
		}
		else if (climbState == WallClimbState.BottomMove || climbState == WallClimbState.TopMove)
		{
			UpdateHorizontalMove();
		}
		return base.Update();
	}

	private void UpdateClimb()
	{
		Vector3 vector = moveDirection * motor.Speed * Time.deltaTime;
		Vector3 forcedPosition = lastPosition + vector;
		if (forcedPosition.y >= endPosition.y)
		{
			forcedPosition.y = endPosition.y;
			SetupTopMove();
		}
		motor.SetForcedPosition(forcedPosition);
		lastPosition = forcedPosition;
	}

	private void UpdateHorizontalMove()
	{
		Vector3 vector = moveDirection * motor.Speed * Time.deltaTime;
		Vector3 forcedPosition = lastPosition + vector;
		moveDistance -= Vector3.Magnitude(vector);
		if (moveDistance > 0f)
		{
			motor.SetForcedPosition(forcedPosition);
			lastPosition = forcedPosition;
		}
		else if (climbState == WallClimbState.BottomMove)
		{
			SetupClimb();
		}
		else
		{
			forcedPosition = endPosition;
			OnCompleted();
		}
	}

	private void SetupBottomMove()
	{
		Vector3 position = transform.position;
		moveDirection = Vector3.Normalize(startPosition - position);
		moveDistance = Vector3.Distance(startPosition, position);
		climbState = WallClimbState.BottomMove;
	}

	private void SetupClimb()
	{
		Vector3 vector = new Vector3(startPosition.x, endPosition.y, startPosition.z);
		moveDirection = Vector3.Normalize(vector - startPosition);
		Vector3 vector2 = Vector3.Normalize(endPosition - linkData.startPos);
		vector2.y = 0f;
		transform.rotation = Quaternion.LookRotation(moveDirection, -vector2.normalized);
		climbState = WallClimbState.Climbing;
	}

	private void SetupTopMove()
	{
		Vector3 vector = new Vector3(startPosition.x, endPosition.y, startPosition.z);
		moveDirection = Vector3.Normalize(endPosition - vector);
		transform.rotation = Quaternion.LookRotation(moveDirection);
		moveDistance = Vector3.Distance(vector, endPosition);
		climbState = WallClimbState.TopMove;
	}

	private void OnCompleted()
	{
		agent.CompleteOffMeshLink();
		agent.Resume();
		motor.EndMotion();
		climbState = WallClimbState.Done;
		status = AIActionStatus.Complete;
	}

	public override void OnSuspended()
	{
		OnCompleted();
	}
}
