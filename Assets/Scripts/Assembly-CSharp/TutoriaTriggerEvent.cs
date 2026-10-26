using UnityEngine;

public class TutoriaTriggerEvent : TutorialEventBase
{
	public enum TriggerEventType
	{
		Enter = 0,
		Exit = 1,
		PassThrough = 2,
		Contains = 3
	}

	public LayerMask AllowedObjects;

	public TriggerEventType TriggerType;

	public ObjectWaypoint WaypointTrigger;

	public string MustHaveTag;

	public string ExludedTag;

	public int NumObjects = 1;

	public bool MustStayInTrigger;

	public bool DisableOnComplete;

	public bool RegisterObjectsOnTrigger;

	private int objectsEntered;

	private int objectsExited;

	private bool hasTriggeredCollider;

	private void Start()
	{
	}

	private void Update()
	{
		if (!base.IsComplete)
		{
			bool flag = false;
			if (hasTriggeredCollider)
			{
				flag = !(WaypointTrigger != null) || WaypointTrigger.HasBeenReached;
			}
			if (flag)
			{
				SetComplete();
			}
		}
	}

	private bool IsRightTag(GameObject gameObject)
	{
		if (!string.IsNullOrEmpty(MustHaveTag))
		{
			return gameObject.tag == MustHaveTag;
		}
		if (!string.IsNullOrEmpty(ExludedTag))
		{
			return gameObject.tag != ExludedTag;
		}
		return true;
	}

	private void OnTriggerStay(Collider collider)
	{
		if (IsRightTag(collider.gameObject) && !base.IsComplete && TriggerType == TriggerEventType.Contains)
		{
			SetHasTriggered(collider.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (IsRightTag(collider.gameObject) && !base.IsComplete)
		{
			objectsEntered++;
			if (objectsEntered >= NumObjects && TriggerType == TriggerEventType.Enter)
			{
				SetHasTriggered(collider.gameObject);
			}
		}
	}

	private void SetHasTriggered(GameObject colliderObject)
	{
		if (RegisterObjectsOnTrigger)
		{
			TutorialManager.AddEventObject(colliderObject.name, colliderObject);
		}
		hasTriggeredCollider = true;
	}

	private void SetComplete()
	{
		if (DisableOnComplete)
		{
			base.gameObject.GetComponent<Collider>().enabled = false;
		}
		SetComplete(true);
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!IsRightTag(collider.gameObject) || base.IsComplete)
		{
			return;
		}
		objectsExited++;
		if (objectsExited == NumObjects)
		{
			if (TriggerType == TriggerEventType.Exit)
			{
				SetComplete();
			}
			else if (TriggerType == TriggerEventType.PassThrough && objectsEntered == NumObjects)
			{
				SetHasTriggered(collider.gameObject);
			}
		}
	}

	public override void OnStart()
	{
	}
}
