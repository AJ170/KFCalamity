using System;
using Calamity.Input;
using UnityEngine;

public class ControlOverlay : MonoBehaviour
{
	public string Name;

	public UILabel[] Labels;

	private TweenAlpha alphaTween;

	private bool fading;

	private float startAlpha;

	private float moveDistance;

	private void Awake()
	{
		alphaTween = GetComponent<TweenAlpha>();
		UIPanel component = GetComponent<UIPanel>();
		startAlpha = component.alpha;
	}

	public void ToggleActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	public void FadeIn()
	{
		alphaTween.Play(true);
		alphaTween.onFinished = null;
		fading = true;
	}

	public void FadeOut()
	{
		TweenAlpha tweenAlpha = alphaTween;
		tweenAlpha.onFinished = (UITweener.OnFinished)Delegate.Combine(tweenAlpha.onFinished, new UITweener.OnFinished(OnFadeOutComplete));
		alphaTween.from = startAlpha;
		alphaTween.to = 0f;
		alphaTween.Play(true);
		fading = true;
	}

	private void OnFadeInComplete(UITweener tweener)
	{
		fading = false;
	}

	private void OnFadeOutComplete(UITweener tweener)
	{
		ToggleActive(false);
		fading = false;
	}

	private bool CheckAnyKeyDown()
	{
		if (InputManager.GetBool(InputAction.Movement))
		{
			return true;
		}
		return TouchHandler.instance.GetNumTouches() > 0 || Input.anyKeyDown || moveDistance > 10f || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
	}

	private void Update()
	{
		if (CheckAnyKeyDown() && !fading)
		{
			FadeOut();
		}
	}
}
