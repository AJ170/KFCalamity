using UnityEngine;

public class OldFocusCamera : MonoBehaviour
{
	public GameObject Target;

	public Vector3 Offset = Vector3.zero;

	public FollowMultipleCamera followCamera;

	public GameObject Roof;

	private Quaternion startRotation;

	private bool isActive;

	private void Start()
	{
		if (followCamera == null)
		{
			followCamera = GetComponent<FollowMultipleCamera>();
		}
		startRotation = base.transform.rotation;
	}

	private void Update()
	{
		if (isActive)
		{
			SetPosition();
		}
	}

	public void Activate()
	{
		if (followCamera != null)
		{
			followCamera.enabled = false;
		}
		SetPosition();
		if (Roof != null)
		{
			Roof.SetActive(true);
		}
		if (SoldierManager.Instance != null)
		{
			SoldierManager.Instance.SetLockedInput(true, this);
		}
		if (GuiInGameManager.Instance != null)
		{
			GuiInGameManager.Instance.SetUnitBarVisibility(false);
		}
		isActive = true;
	}

	public void Deactivate()
	{
		if (followCamera != null)
		{
			followCamera.enabled = true;
		}
		base.transform.rotation = startRotation;
		if (Roof != null)
		{
			Roof.SetActive(false);
		}
		if (SoldierManager.Instance != null)
		{
			SoldierManager.Instance.SetLockedInput(false, this);
		}
		if (GuiInGameManager.Instance != null)
		{
			GuiInGameManager.Instance.SetUnitBarVisibility(true);
		}
		isActive = false;
	}

	private void SetPosition()
	{
		if (Target != null)
		{
			Transform transform = Target.transform;
			base.transform.position = transform.TransformPoint(Offset);
			base.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(transform.position + Vector3.up * 2f - base.transform.position));
		}
	}
}
