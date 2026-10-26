using Calamity.Camera;
using EventHelpers;
using UnityEngine;

public class FocusCameraEffect : EffectBaseListener
{
	public bool TargetParent = true;

	public float Duration = 5f;

	private GameObject parent;

	private CameraController cameraController;

	private float counter;

	protected override void Start()
	{
		base.Start();
		GameObject gameObject = Camera.main.gameObject;
		if (gameObject != null)
		{
			cameraController = gameObject.GetComponent<CameraController>();
		}
	}

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	private void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				DeactivateCamera();
			}
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			ActivateCamera();
		}
	}

	private void ActivateCamera()
	{
		if (cameraController != null && TargetParent)
		{
			cameraController.SetFocusTarget(parent);
			cameraController.SetActiveState(CameraStateType.Focus);
			counter = Duration;
		}
	}

	private void DeactivateCamera()
	{
		if (cameraController != null)
		{
			cameraController.SetActiveState(CameraStateType.Follow);
		}
	}
}
