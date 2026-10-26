using Calamity.Camera;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementMotor : MonoBehaviour
{
	public float MovementSpeed = 5f;

	public float Acceleration = 50f;

	public float RotationSpeed = 0.3f;

	public float Gravity = -20f;

	public bool AllowAutoRotation;

	private Vector3 movementDirection = Vector3.zero;

	private Vector3 facingDirection = Vector3.zero;

	private Vector3 forcedVelocity = Vector3.zero;

	private Vector3 forcedGravity = Vector3.zero;

	public bool BlockRotation;

	private AttributeModifier speedAttribute;

	private AttributeModifier rotationAttribute;

	private FollowCamera followCamera;

	public float MovingBackwardsMultiplier = 0.75f;

	public float MovingBackwardsAngle = 120f;

	private bool isWalkingBackwards;

	private Transform soldierTransform;

	private Rigidbody soldierBody;

	public Vector3 MovementDirection
	{
		get
		{
			return movementDirection;
		}
	}

	public Vector3 FacingDirection
	{
		get
		{
			return facingDirection;
		}
	}

	private void Awake()
	{
		soldierTransform = base.transform;
		soldierBody = base.GetComponent<Rigidbody>();
	}

	private void Start()
	{
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, base.gameObject);
		rotationAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.RotationSpeed, base.gameObject);
		Camera main = Camera.main;
		if (main != null)
		{
			CameraController component = main.GetComponent<CameraController>();
			if (component != null)
			{
				followCamera = component.FollowCamera;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!RestrictMovement())
		{
			UpdateMovingBackwards();
			UpdateMovement();
		}
		UpdateRotation();
	}

	public void SetMovementDirection(Vector3 direction)
	{
		movementDirection = direction;
	}

	public void SetFacingDirection(Vector3 direction)
	{
		facingDirection = direction;
	}

	public void SetForcedVelocity(Vector3 velocity)
	{
		forcedVelocity = velocity;
	}

	public void SetForcedGravity(float gravity)
	{
		if (gravity == 0f)
		{
			forcedGravity = Vector3.zero;
		}
		else
		{
			forcedGravity = Vector3.up * gravity;
		}
	}

	private void UpdateMovement()
	{
		Vector3 zero = Vector3.zero;
		Vector3 vector = ((!(forcedVelocity != zero)) ? (movementDirection * MovementSpeed * speedAttribute.Multiplier) : forcedVelocity);
		if (forcedGravity != zero)
		{
			Vector3 vector2 = forcedGravity / (1f / Time.fixedDeltaTime);
			vector += vector2;
		}
		else if (soldierBody.useGravity)
		{
			vector.y = 0f;
		}
		Vector3 velocity = soldierBody.velocity;
		float y = velocity.y;
		velocity.y = 0f;
		Vector3 vector3 = vector - velocity;
		if (vector3 != zero)
		{
			if (vector == zero && y > 0f)
			{
				vector3.y = 0f - y;
			}
			soldierBody.AddForce(vector3 * Acceleration, ForceMode.Acceleration);
		}
	}

	private bool RestrictMovement()
	{
		if (followCamera != null)
		{
			bool pushed;
			Vector3 position = followCamera.PushPositionToBounds(soldierTransform.position, out pushed);
			if (pushed)
			{
				soldierBody.velocity = Vector3.zero;
				soldierBody.MovePosition(position);
				return true;
			}
		}
		return false;
	}

	private void UpdateRotation()
	{
		if (BlockRotation)
		{
			return;
		}
		Vector3 vector = facingDirection;
		if (AllowAutoRotation && vector == Vector3.zero)
		{
			vector = movementDirection;
		}
		if (!(vector != Vector3.zero))
		{
			return;
		}
		if (RotationSpeed != -1f)
		{
			float num = AngleAroundAxis(soldierTransform.forward, vector, Vector3.up);
			if (Mathf.Abs(num) > 0.1f)
			{
				num *= RotationSpeed * rotationAttribute.Multiplier;
			}
			if (num != 0f)
			{
				soldierTransform.Rotate(Vector3.up, num);
			}
		}
		else
		{
			soldierTransform.rotation = Quaternion.LookRotation(vector);
		}
	}

	private void UpdateMovingBackwards()
	{
		if (IsMoving())
		{
			Vector3 forward = soldierTransform.forward;
			float num = Vector3.Angle(forward, movementDirection);
			bool flag = num > MovingBackwardsAngle;
			if (flag && !isWalkingBackwards)
			{
				speedAttribute.Alter(this, MovingBackwardsMultiplier);
				isWalkingBackwards = true;
			}
			else if (!flag && isWalkingBackwards)
			{
				speedAttribute.Restore(this);
				isWalkingBackwards = false;
			}
		}
		else if (isWalkingBackwards)
		{
			speedAttribute.Restore(this);
			isWalkingBackwards = false;
		}
	}

	private float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		float num = Vector3.Angle(dirA, dirB);
		return num * (float)((!(Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f)) ? 1 : (-1));
	}

	public bool IsMoving()
	{
		return movementDirection != Vector3.zero;
	}

	public float CurrentMovementSpeed()
	{
		if (IsMoving())
		{
			return MovementSpeed * speedAttribute.Multiplier;
		}
		return 0f;
	}
}
