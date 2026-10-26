using Calamity.Input;
using UnityEngine;

namespace PlayerInputHelpers
{
	public abstract class DualInputActionBase
	{
		private float inputCounter;

		private float inputThreshold = 0.5f;

		private SoldierInputController playerInputController;

		public DualInputActionBase(float inputThreshold, SoldierInputController playerInputController)
		{
			this.inputThreshold = inputThreshold;
			this.playerInputController = playerInputController;
		}

		public void ProcessInput(bool input)
		{
			if (input)
			{
				if (inputCounter == DualInputStatus.THRESHOLD_DONE || inputCounter == DualInputStatus.THRESHOLD_FAILED)
				{
					return;
				}
				if (ThresholdEvaluator())
				{
					if (inputCounter == 0f)
					{
						ThresholdInputStart(inputThreshold);
					}
					inputCounter += Time.deltaTime;
					if (inputCounter > inputThreshold)
					{
						if (!ThredholdAction())
						{
							PlayErrorSound();
						}
						inputCounter = DualInputStatus.THRESHOLD_DONE;
					}
				}
				else
				{
					PlayErrorSound();
					if (inputCounter > 0f)
					{
						ThresholdInputRelease();
					}
					inputCounter = DualInputStatus.THRESHOLD_FAILED;
				}
				return;
			}
			if (inputCounter == DualInputStatus.THRESHOLD_DONE || inputCounter > 0f)
			{
				ThresholdInputRelease();
			}
			if (inputCounter > 0f)
			{
				if (NormalEvaluator())
				{
					if (!NormalAction())
					{
						PlayErrorSound();
					}
				}
				else
				{
					PlayErrorSound();
				}
			}
			inputCounter = 0f;
		}

		protected virtual void ThresholdInputStart(float threshold)
		{
		}

		protected virtual void ThresholdInputRelease()
		{
		}

		protected virtual bool ThresholdEvaluator()
		{
			return true;
		}

		protected virtual bool ThredholdAction()
		{
			return false;
		}

		protected virtual bool NormalEvaluator()
		{
			return true;
		}

		protected virtual bool NormalAction()
		{
			return false;
		}

		private void PlayErrorSound()
		{
			if (playerInputController != null)
			{
				playerInputController.PlayErrorSound();
			}
		}
	}
}
