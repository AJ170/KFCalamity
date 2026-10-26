using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBarDrag : MonoBehaviour
{
	[Serializable]
	public struct ButtonLayout
	{
		public string Name;

		public Vector2 DodgeButton;

		public Vector2 SyringeButton;

		public Vector2 GrenadeButton;

		public Vector2 AbilityButton;

		public Vector2 ReloadButton;

		public Vector2 Offset;
	}

	public GameObject MovableFeedback;

	public Vector2 Size;

	public Transform DodgeButton;

	public Transform SyringeButton;

	public Transform GrenadeButton;

	public Transform AbilityButton;

	public Transform ReloadButton;

	public ButtonActions FlipButton;

	public UISprite BackgroundFrame;

	public Vector2 PaddingBackgroundFrame;

	public ButtonLayout[] Layouts;

	private UIAnchor offsetAnchor;

	private Vector3 halfSize;

	private Transform cachedTransform;

	private bool isDragging;

	private bool isTweenedOut;

	private OptionManager optionManager;

	private TweenPosition positionTween;

	private UIRoot root;

	private int layoutIndex;

	private TweenAlpha[] alphaTweens;

	private List<Transform> allTransforms;

	private List<AbilityButtonNormal> allAbilityButtons;

	private void Awake()
	{
		optionManager = OptionManager.Instance;
		Transform[] collection = new Transform[5] { DodgeButton, SyringeButton, GrenadeButton, AbilityButton, ReloadButton };
		allTransforms = new List<Transform>(collection);
		allAbilityButtons = new List<AbilityButtonNormal>(allTransforms.ConvertAll((Transform a) => a.GetComponent<AbilityButtonNormal>()));
		positionTween = GetComponent<TweenPosition>();
		TweenPosition tweenPosition = positionTween;
		tweenPosition.onFinished = (UITweener.OnFinished)Delegate.Combine(tweenPosition.onFinished, new UITweener.OnFinished(OnTweeedIn));
		offsetAnchor = GetComponent<UIAnchor>();
		offsetAnchor.relativeOffset = optionManager.ButtonBarPosition;
		if (FlipButton != null)
		{
			FlipButton.SetOnClickCallback(OnFlipButtonClicked);
		}
		if (GuiInGameManager.Instance.IsGamePaused)
		{
			offsetAnchor.enabled = false;
			positionTween.enabled = false;
		}
		alphaTweens = MovableFeedback.GetComponentsInChildren<TweenAlpha>(true);
		bool save = false;
		if (optionManager.ButtonLayout >= Layouts.Length || optionManager.ButtonLayout < 0)
		{
			optionManager.ButtonLayout = 2;
			save = true;
		}
		ApplyLayout(optionManager.ButtonLayout, save);
	}

	public void ToggleButtonsVisible(bool toggle)
	{
		foreach (AbilityButtonNormal allAbilityButton in allAbilityButtons)
		{
			allAbilityButton.Icon.SetActive(toggle);
			allAbilityButton.Background.gameObject.SetActive(toggle);
			allAbilityButton.Charges.SetActive(toggle);
			allAbilityButton.Cooldown.SetActive(toggle);
		}
	}

	private void CalculateFrameSize()
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		float num3 = float.MinValue;
		float num4 = float.MinValue;
		foreach (Transform allTransform in allTransforms)
		{
			Vector3 localPosition = allTransform.localPosition;
			if (localPosition.x < num)
			{
				num = localPosition.x;
			}
			if (localPosition.x > num3)
			{
				num3 = localPosition.x;
			}
			if (localPosition.y < num2)
			{
				num2 = localPosition.y;
			}
			if (localPosition.y > num4)
			{
				num4 = localPosition.y;
			}
		}
		float x = num3 - num + PaddingBackgroundFrame.x * 2f;
		float y = num4 - num2 + PaddingBackgroundFrame.y * 2f;
		BackgroundFrame.transform.localScale = new Vector3(x, y, 1f);
		halfSize = BackgroundFrame.transform.localScale * 0.5f;
	}

	private void ApplyLayout(int layoutIndex, bool save)
	{
		ButtonLayout buttonLayout = Layouts[layoutIndex];
		Vector3 vector = new Vector3(buttonLayout.Offset.x, buttonLayout.Offset.y, 0f);
		DodgeButton.localPosition = new Vector3(buttonLayout.DodgeButton.x, buttonLayout.DodgeButton.y, DodgeButton.localPosition.z) + vector;
		SyringeButton.localPosition = new Vector3(buttonLayout.SyringeButton.x, buttonLayout.SyringeButton.y, SyringeButton.localPosition.z) + vector;
		GrenadeButton.localPosition = new Vector3(buttonLayout.GrenadeButton.x, buttonLayout.GrenadeButton.y, GrenadeButton.localPosition.z) + vector;
		AbilityButton.localPosition = new Vector3(buttonLayout.AbilityButton.x, buttonLayout.AbilityButton.y, AbilityButton.localPosition.z) + vector;
		ReloadButton.localPosition = new Vector3(buttonLayout.ReloadButton.x, buttonLayout.ReloadButton.y, ReloadButton.localPosition.z) + vector;
		CalculateFrameSize();
		optionManager.ButtonLayout = layoutIndex;
		optionManager.SetSaveData(optionManager.Serialize());
		optionManager.Save();
	}

	private void OnFlipButtonClicked(ButtonActions sender)
	{
		layoutIndex = (layoutIndex + 1) % Layouts.Length;
		ApplyLayout(layoutIndex, true);
	}

	private void Start()
	{
		cachedTransform = base.transform;
		root = NGUITools.FindInParents<UIRoot>(base.gameObject);
		halfSize = Size * 0.5f;
	}

	public void TweenOut()
	{
		positionTween.from.y = positionTween.cachedTransform.localPosition.y;
		positionTween.to = positionTween.cachedTransform.localPosition;
		positionTween.delay = 0f;
		positionTween.Play(false);
		offsetAnchor.enabled = false;
		isTweenedOut = true;
	}

	public void TweenIn()
	{
		positionTween.delay = 0f;
		positionTween.Play(true);
		isTweenedOut = false;
	}

	public void OnTweeedIn(UITweener tweener)
	{
		if (!isTweenedOut)
		{
			offsetAnchor.enabled = true;
		}
	}

	public void OnWeaponTrayToggled(bool isOpen)
	{
		MovableFeedback.SetActive(isOpen);
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			offsetAnchor.enabled = false;
			isDragging = true;
			TweenAlpha[] array = alphaTweens;
			foreach (TweenAlpha tweenAlpha in array)
			{
				tweenAlpha.enabled = true;
				tweenAlpha.Play(true);
			}
			return;
		}
		offsetAnchor.enabled = true;
		isDragging = false;
		optionManager.ButtonBarPosition = offsetAnchor.relativeOffset;
		optionManager.SetSaveData(optionManager.Serialize());
		optionManager.Save();
		TweenAlpha[] array2 = alphaTweens;
		foreach (TweenAlpha tweenAlpha2 in array2)
		{
			tweenAlpha2.enabled = true;
			tweenAlpha2.Play(false);
		}
	}

	private void Update()
	{
		if (isDragging)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			float num = root.activeHeight;
			float num2 = (float)Screen.width * (num / (float)Screen.height);
			if (localPosition.x + halfSize.x > num2)
			{
				localPosition.x = num2 - halfSize.x;
			}
			else if (localPosition.x - halfSize.x < 0f)
			{
				localPosition.x = halfSize.x;
			}
			if (localPosition.y + halfSize.y > num)
			{
				localPosition.y = num - halfSize.y;
			}
			else if (localPosition.y - halfSize.y < 0f)
			{
				localPosition.y = halfSize.y;
			}
			offsetAnchor.relativeOffset = new Vector2(localPosition.x / num2, localPosition.y / num);
			offsetAnchor.relativeOffset.x = Mathf.Clamp01(offsetAnchor.relativeOffset.x);
			offsetAnchor.relativeOffset.y = Mathf.Clamp01(offsetAnchor.relativeOffset.y);
		}
	}
}
