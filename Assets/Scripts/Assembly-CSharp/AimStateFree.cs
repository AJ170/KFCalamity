using Calamity.Input;
using UnityEngine;

public class AimStateFree : AimState
{
	private float heightUpdateFrequency = 4f;

	private float heightUpdateCounter = 1f;

	private bool lerping;

	public AimStateFree(GameObject soldier, Transform spine)
		: base(AimStateType.Free, soldier, spine)
	{
	}

	public override void Update()
	{
		UpdateHorizontalRotation();
		UpdateHeight();
	}

	public override void LateUpdate()
	{
		UpdateVerticalRotation();
	}

	private void UpdateHorizontalRotation()
	{
		if (!inputLocked && inputController.Control != null)
		{
			Vector3 vector = inputController.Control.GetVector(InputAction.Rotation);
			motor.SetFacingDirection(vector);
		}
	}

	private void UpdateVerticalRotation()
	{
		if (spine != null)
		{
			if (lerping)
			{
				spineRotation = Mathf.Lerp(spineRotation, spineRotationTarget + 3.85f, 5f * Time.deltaTime);
			}
			Vector3 localEulerAngles = spine.localEulerAngles;
			spine.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, spineRotation);
			if (lerping && Mathf.Abs(spineRotationTarget + 3.85f - spineRotation) < 0.05f)
			{
				lerping = false;
			}
		}
	}

	private void UpdateHeight()
	{
		heightUpdateCounter -= Time.deltaTime;
		if (heightUpdateCounter <= 0f)
		{
			spineRotationTarget = GetGroundAimRotation();
			lerping = true;
			heightUpdateCounter = 1f / heightUpdateFrequency;
		}
	}
}
