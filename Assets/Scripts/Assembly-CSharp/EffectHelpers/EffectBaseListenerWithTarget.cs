using EffectEvents;
using EventHelpers;
using UnityEngine;

namespace EffectHelpers
{
	public class EffectBaseListenerWithTarget : EffectBaseListener
	{
		public TargetId Target;

		protected GameObject FindTargetObject(TriggerBase e)
		{
			GameObject result = null;
			if (Target == TargetId.Self)
			{
				result = base.gameObject;
			}
			else if (Target == TargetId.Other)
			{
				if (e.Id == TriggerId.Hit || e.Id == TriggerId.HitAOE)
				{
					HitBase hitBase = (HitBase)e;
					result = hitBase.HitTransform.gameObject;
				}
				else if (e.Id == TriggerId.FriendlyAim)
				{
					FriendlyAim friendlyAim = (FriendlyAim)e;
					result = friendlyAim.Target;
				}
				else if (e.Id == TriggerId.Affect)
				{
					Affect affect = (Affect)e;
					result = affect.AffectTransform.gameObject;
				}
			}
			else if (Target == TargetId.Parent)
			{
				result = base.transform.root.gameObject;
			}
			return result;
		}
	}
}
