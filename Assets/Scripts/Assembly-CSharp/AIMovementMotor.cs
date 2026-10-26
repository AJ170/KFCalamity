using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovementMotor : MonoBehaviour
{
	public float StoppingDistance = 1f;

	public float Speed = 3.5f;

	public float Acceleration = 8f;

	public float AngularSpeed = 360f;

	public float RotationSpeed = 0.3f;

	public bool ZombieLike;

	public float DistanceBeforeZombie = 2f;

	private AIController aiController;

	private ZedAnimationController animationController;

	private NavMeshAgent agent;

	private new Rigidbody rigidbody;

	private bool moving;

	private bool rotating;

	private Vector3 rotationDirection = Vector3.zero;

	private bool inMotion;

	private Vector3 forcedPosition = Vector3.zero;

	private AttributeModifier speedAttribute;

	public NavMeshAgent Agent
	{
		get
		{
			return agent;
		}
	}

	public bool Moving
	{
		get
		{
			return moving;
		}
	}

	private void Awake()
	{
		aiController = GetComponent<AIController>();
		animationController = GetComponent<ZedAnimationController>();
		agent = GetComponent<NavMeshAgent>();
		rigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	private void Start()
	{
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, base.gameObject);
		Speed *= GlobalModifiers.Speed;
		SetupAgentStats();
	}

	private void Update()
	{
		if (moving && !inMotion)
		{
			UpdateSpeed();
			UpdateMoving();
			UpdateMotions();
		}
		else
		{
			UpdateRotation();
		}
	}

	private void FixedUpdate()
	{
		if (inMotion)
		{
			UpdateForcedPosition();
		}
	}

	public void MoveToTargetPosition(Vector3 targetPosition)
	{
		Vector3 position = base.transform.position;
		float num = Vector3.Distance(position, targetPosition);
		if (num > StoppingDistance)
		{
			agent.SetDestination(targetPosition);
			OnStarted();
		}
	}

	public void Resume()
	{
		if (agent.hasPath)
		{
			OnStarted();
			agent.Resume();
		}
	}

	public void Warp(Vector3 targetPosition)
	{
		agent.Warp(targetPosition);
	}

	public void Stop()
	{
		Stop(false);
	}

	public void Stop(bool stopUpdates)
	{
		agent.Stop(stopUpdates);
		OnStopped();
	}

	public void SetRotationDirection(Vector3 direction)
	{
		rotationDirection = direction;
		OnStartRotation();
	}

	public void EndRotation()
	{
		OnStopRotation();
	}

	public void SetForcedPosition(Vector3 position)
	{
		forcedPosition = position;
	}

	public void StartMotion()
	{
		inMotion = true;
	}

	public void EndMotion()
	{
		inMotion = false;
		forcedPosition = Vector3.zero;
	}

	private void SetupAgentStats()
	{
		agent.stoppingDistance = StoppingDistance;
		agent.speed = Speed * speedAttribute.Multiplier;
		agent.acceleration = Acceleration;
		agent.angularSpeed = AngularSpeed;
	}

	private void UpdateSpeed()
	{
		float num = Speed * speedAttribute.Multiplier;
		float acceleration = Acceleration * speedAttribute.Multiplier;
		if (num != agent.speed)
		{
			agent.speed = num;
			agent.acceleration = acceleration;
		}
	}

	private void UpdateMoving()
	{
		if (DestinationReached())
		{
			Stop();
		}
	}

	private void UpdateMotions()
	{
		if ((inMotion ^ agent.isOnOffMeshLink) && !inMotion && aiController != null)
		{
			TriggerMotionEvent();
			if (!inMotion)
			{
				agent.CompleteOffMeshLink();
			}
		}
	}

	private void TriggerMotionEvent()
	{
		OffMeshLinkData currentOffMeshLinkData = agent.currentOffMeshLinkData;
		OffMeshLink offMeshLink = currentOffMeshLinkData.offMeshLink;
		if (offMeshLink != null)
		{
			AIMotion component = offMeshLink.GetComponent<AIMotionComponent>();
			aiController.TriggerAIEvent(new MotionAIEvent(agent, currentOffMeshLinkData, component));
		}
		else
		{
			aiController.TriggerAIEvent(new MotionAIEvent(agent, currentOffMeshLinkData));
		}
	}

	private void UpdateForcedPosition()
	{
		if (forcedPosition != Vector3.zero)
		{
			if (rigidbody != null)
			{
				rigidbody.MovePosition(forcedPosition);
			}
			else
			{
				base.transform.position = forcedPosition;
			}
		}
	}

	public bool DestinationReached()
	{
		if (moving)
		{
			if (!agent.pathPending)
			{
				return agent.remainingDistance < StoppingDistance;
			}
			return false;
		}
		return true;
	}

	private void UpdateRotation()
	{
		if (rotationDirection != Vector3.zero)
		{
			if (!rotating)
			{
				OnStartRotation();
			}
			float num = AngleAroundAxis(base.transform.forward, rotationDirection, Vector3.up);
			if (rigidbody != null)
			{
				rigidbody.angularVelocity = Vector3.up * num * RotationSpeed;
			}
			else
			{
				base.transform.Rotate(Vector3.up, num * RotationSpeed * Time.deltaTime);
			}
		}
	}

	private void OnStarted()
	{
		moving = true;
		OnStopRotation();
		if (animationController != null)
		{
			animationController.SetBool(ZedAnimationTrigger.Idle, false);
		}
	}

	private void OnStopped()
	{
		moving = false;
		if (animationController != null)
		{
			animationController.SetBool(ZedAnimationTrigger.Idle, true);
		}
	}

	private void OnStartRotation()
	{
		if (rigidbody != null)
		{
			rigidbody.isKinematic = false;
			SetConstraints((RigidbodyConstraints)94);
		}
		rotating = true;
	}

	private void OnStopRotation()
	{
		if (rotating)
		{
			if (rigidbody != null)
			{
				rigidbody.isKinematic = true;
				SetConstraints((RigidbodyConstraints)80);
			}
			rotationDirection = Vector3.zero;
			rotating = false;
		}
	}

	public void SetConstraints(RigidbodyConstraints constraints)
	{
		if (rigidbody != null)
		{
			rigidbody.constraints = constraints;
		}
	}

	private float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		float num = Vector3.Angle(dirA, dirB);
		return num * (float)((!(Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f)) ? 1 : (-1));
	}
}
