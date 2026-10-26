using System;
using EffectEvents;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class BeingHitAnimation : MonoBehaviour, IEffectTriggerListener
{
	[Serializable]
	public class BlockAnimation
	{
		public string Animation;

		[HideInInspector]
		public int Hash;

		public int Layer;
	}

	public BlockAnimation[] BlockAnimations;

	public int HitAnimationCount = 3;

	public int DamageThreshold = 15;

	private AnimationController animationController;

	private int[] hitAnimationHashes;

	void IEffectTriggerListener.OnEffectTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.DamageReceived)
		{
			OnDamageReceived(e as DamageReceived);
		}
	}

	private void Awake()
	{
		GlobalHelpers.RegisterEffectEventListener(base.gameObject, this);
		animationController = base.transform.parent.GetComponent<AnimationController>();
		SetupBlockAnimationHashes();
		SetupHitAnimationHashes();
	}

	private void SetupBlockAnimationHashes()
	{
		if (animationController != null)
		{
			BlockAnimation[] blockAnimations = BlockAnimations;
			foreach (BlockAnimation blockAnimation in blockAnimations)
			{
				blockAnimation.Hash = animationController.GetHash(blockAnimation.Animation);
			}
		}
	}

	private void SetupHitAnimationHashes()
	{
		if (animationController != null)
		{
			hitAnimationHashes = new int[HitAnimationCount];
			for (int i = 0; i < HitAnimationCount; i++)
			{
				hitAnimationHashes[i] = animationController.GetHash("Hit" + (i + 1));
			}
		}
	}

	private void OnDamageReceived(DamageReceived damageReceived)
	{
		if (damageReceived.Amount >= DamageThreshold && CanTriggerAnimation())
		{
			TriggerRandomHitAnimation();
		}
	}

	private bool CanTriggerAnimation()
	{
		if (animationController != null)
		{
			BlockAnimation[] blockAnimations = BlockAnimations;
			foreach (BlockAnimation blockAnimation in blockAnimations)
			{
				if (animationController.IsAnimationWithTagActive(blockAnimation.Hash, blockAnimation.Layer))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private void TriggerRandomHitAnimation()
	{
		int hash = hitAnimationHashes[UnityEngine.Random.Range(0, HitAnimationCount)];
		animationController.TriggerAnimation(hash);
	}
}
