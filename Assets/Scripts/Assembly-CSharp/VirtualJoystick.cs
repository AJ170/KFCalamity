using System;
using Calamity.Input;
using UnityEngine;

public class VirtualJoystick : MonoBehaviour
{
	[Serializable]
	public class JoystickZone
	{
		public string Name;

		public UISprite Sprite;

		public float Min;

		public float Max;

		public string OnActivatedSprite;

		public string OnActiveSprite;

		[HideInInspector]
		public bool isActive;

		[HideInInspector]
		public TweenAlpha AlphaTween;
	}

	public float maxDragLength;

	public float DeadZone = 0.5f;

	public UISprite JoystickBase;

	public UISprite Background;

	public Transform BaseParent;

	public JoystickZone[] Zones;

	private Vector3 initialLocalPos;

	private Vector3 direction;

	private Vector3 staticStickOffset;

	private Transform objectTransform;

	private Transform joystickBaseTransform;

	private BoxCollider cachedCollider;

	private bool isDragging;

	private bool initialized;

	private bool lockedSticks;

	private float posMultiplier;

	[HideInInspector]
	public int touchID;

	public Vector3 Direction
	{
		get
		{
			return direction;
		}
	}

	public float HorizontalAxis
	{
		get
		{
			return direction.x;
		}
	}

	public void SetStaticStickOffset(Vector3 offset)
	{
		staticStickOffset = offset;
	}

	public int GetNumberOfZones()
	{
		return (Zones != null) ? Zones.Length : 0;
	}

	public void GetZoneThreshold(string zoneName, ref float min, ref float max)
	{
		if (Zones == null)
		{
			Debug.LogWarning("Tried getting a threshold for the zone: " + zoneName + ", but the joystick has no zones set", this);
			return;
		}
		int num = Array.FindIndex(Zones, (JoystickZone a) => a.Name == zoneName);
		if (num > -1)
		{
			min = Zones[num].Min;
			max = Zones[num].Max;
		}
		else
		{
			Debug.LogWarning("The joystick does not contain a zone named: " + zoneName);
		}
	}

	private void OnPress(bool isDown)
	{
		if (lockedSticks)
		{
			if (isDown && !isDragging)
			{
				StickTouchRecognizer.Instance.OnPress(isDown);
			}
			else if (!isDown)
			{
				StickTouchRecognizer.Instance.OnPress(isDown);
			}
		}
	}

	private void Initialize()
	{
		objectTransform = base.transform;
		joystickBaseTransform = JoystickBase.transform;
		lockedSticks = InputManager.Instance.StickType == TouchStickType.Locked;
		initialized = true;
	}

	private void Awake()
	{
		if (!initialized)
		{
			Initialize();
		}
	}

	private void Start()
	{
		posMultiplier = (float)UIRoot.list[0].activeHeight / (float)Screen.height;
		if (Zones != null)
		{
			JoystickZone[] zones = Zones;
			foreach (JoystickZone joystickZone in zones)
			{
				joystickZone.AlphaTween = joystickZone.Sprite.GetComponent<TweenAlpha>();
				TweenAlpha alphaTween = joystickZone.AlphaTween;
				alphaTween.onFinished = (UITweener.OnFinished)Delegate.Combine(alphaTween.onFinished, new UITweener.OnFinished(OnZoneAlphaTweenComplete));
			}
		}
		initialLocalPos = objectTransform.localPosition;
		if (lockedSticks)
		{
			cachedCollider = GetComponent<BoxCollider>();
			cachedCollider.enabled = true;
		}
	}

	private UICamera.MouseOrTouch GetMouseOrTouch()
	{
		return UICamera.GetTouch(touchID);
	}

	private void Update()
	{
		if (isDragging)
		{
			UICamera.MouseOrTouch mouseOrTouch = GetMouseOrTouch();
			objectTransform.localPosition = mouseOrTouch.pos * posMultiplier;
			if (lockedSticks)
			{
				objectTransform.localPosition += staticStickOffset;
			}
			direction = objectTransform.localPosition - initialLocalPos;
			float num = direction.magnitude - DeadZone;
			direction = direction.normalized * Mathf.Min(num / maxDragLength, 1f);
			Vector3 vector = direction;
			if (direction.magnitude < DeadZone)
			{
				direction = Vector3.zero;
			}
			if (num > maxDragLength)
			{
				objectTransform.localPosition = initialLocalPos + vector * maxDragLength;
			}
			joystickBaseTransform.localPosition = objectTransform.localPosition;
			JoystickBase.alpha = direction.magnitude;
			float num2 = Mathf.Atan(direction.y / direction.x) * 57.29578f;
			if (direction.x > 0f)
			{
				num2 += 180f;
			}
			if (!float.IsNaN(num2))
			{
				joystickBaseTransform.localEulerAngles = new Vector3(0f, 0f, num2 + 45f);
			}
			UpdateZones();
		}
	}

	private void OnZoneAlphaTweenComplete(UITweener tweener)
	{
		JoystickZone joystickZone = Array.Find(Zones, (JoystickZone a) => a.AlphaTween == tweener);
		if (tweener.tweenFactor == 0f)
		{
			joystickZone.Sprite.enabled = false;
		}
		else if (tweener.tweenFactor == 1f)
		{
			joystickZone.Sprite.enabled = true;
		}
		if (Background.enabled)
		{
			Background.enabled = false;
		}
	}

	private void UpdateZones()
	{
		if (Zones == null)
		{
			return;
		}
		JoystickZone[] zones = Zones;
		foreach (JoystickZone joystickZone in zones)
		{
			float magnitude = direction.magnitude;
			if (magnitude >= joystickZone.Min && magnitude < joystickZone.Max)
			{
				if (!joystickZone.isActive)
				{
					joystickZone.AlphaTween.enabled = true;
					joystickZone.AlphaTween.Play(true);
					joystickZone.Sprite.enabled = true;
					joystickZone.isActive = true;
				}
			}
			else if (joystickZone.isActive)
			{
				joystickZone.AlphaTween.Play(false);
				joystickZone.isActive = false;
			}
		}
	}

	public void EnableStick()
	{
		isDragging = true;
		if (!lockedSticks)
		{
			initialLocalPos = objectTransform.localPosition;
			joystickBaseTransform.localPosition = initialLocalPos;
			if (!JoystickBase.enabled)
			{
				JoystickBase.enabled = true;
			}
			if (BaseParent != null && !BaseParent.gameObject.activeSelf)
			{
				BaseParent.gameObject.SetActive(true);
			}
			if (BaseParent != null)
			{
				BaseParent.localPosition = objectTransform.localPosition;
			}
		}
	}

	public void DisableStick()
	{
		if (!isDragging)
		{
			return;
		}
		if (Background != null)
		{
			Background.enabled = true;
		}
		direction = Vector3.zero;
		isDragging = false;
		if (base.gameObject.activeSelf)
		{
			objectTransform.localPosition = initialLocalPos;
		}
		joystickBaseTransform.localPosition = objectTransform.localPosition;
		JoystickBase.alpha = 0f;
		if (Zones != null)
		{
			JoystickZone[] zones = Zones;
			foreach (JoystickZone joystickZone in zones)
			{
				joystickZone.Sprite.enabled = false;
				joystickZone.AlphaTween.enabled = false;
				joystickZone.isActive = false;
			}
		}
	}

	public void HideJoystick()
	{
		base.gameObject.SetActive(false);
		JoystickBase.enabled = false;
		if (BaseParent != null)
		{
			BaseParent.gameObject.SetActive(false);
		}
	}

	public void ShowJoystick()
	{
		base.gameObject.SetActive(true);
		if (BaseParent != null)
		{
			BaseParent.gameObject.SetActive(true);
		}
	}

	public void SetStickPosition(Vector3 newPosition)
	{
		objectTransform.localPosition = newPosition;
		if (BaseParent != null)
		{
			BaseParent.localPosition = newPosition;
		}
	}
}
