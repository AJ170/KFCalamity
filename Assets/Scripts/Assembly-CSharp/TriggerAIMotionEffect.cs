using EventHelpers;
using UnityEngine;

public class TriggerAIMotionEffect : EffectBaseListener
{
	public AIMotionType MotionType = AIMotionType.Leap;

	private AIController aiController;

	private void OnMounted(GameObject parent)
	{
		aiController = parent.GetComponent<AIController>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && aiController != null)
		{
			AIMotion aIMotion = null;
			AIMotionType motionType = MotionType;
			if (motionType == AIMotionType.Leap)
			{
				aIMotion = new AILeapMotion();
			}
			if (aIMotion != null)
			{
				aiController.TriggerAIEvent(new MotionAIEvent(aIMotion));
			}
		}
	}
}
