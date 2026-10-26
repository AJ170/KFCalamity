using EventHelpers;
using UnityEngine;

public class TriggerWeaponAnimationEffect : TriggerAnimationEffect
{
	private GameObject parent;

	protected override void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	protected override void Execute(TriggerBase e)
	{
		if (animationController == null && parent != null)
		{
			FindAnimationController();
			if (animationController != null)
			{
				GetHash();
			}
		}
		base.Execute(e);
	}

	private void FindAnimationController()
	{
		WeaponController component = parent.GetComponent<WeaponController>();
		if (!(component != null))
		{
			return;
		}
		GameObject activeWeaponGameObject = component.ActiveWeaponGameObject;
		if (activeWeaponGameObject != null)
		{
			animationController = activeWeaponGameObject.GetComponent<AnimationController>();
			if (animationController == null)
			{
				Debug.LogError(string.Format("TriggerWeaponAnimation failed to locate Animator in parent {0}.", parent.name));
			}
		}
		else
		{
			Debug.LogError(string.Format("TriggerWeaponAnimation failed to locate active weapon in parent {0}.", parent.name));
		}
	}
}
