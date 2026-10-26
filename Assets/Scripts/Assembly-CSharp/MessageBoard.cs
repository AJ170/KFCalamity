using UnityEngine;

public class MessageBoard : MonoBehaviour
{
	public enum FontSize
	{
		Normal = 0,
		Small = 1,
		Large = 2
	}

	public UILabel Label;

	public TweenAlpha AlphaTween;

	public UIFont Font24;

	public UIFont Font36;

	public UIFont Font48;

	private float timeLeft;

	private string currentMessage = string.Empty;

	private bool calulateTime;

	private bool displayTime;

	private Vector3 baseScale = Vector3.zero;

	private GuiInGameManager manager;

	private void Start()
	{
		baseScale = Label.transform.localScale;
	}

	public void SetManagerReference(GuiInGameManager gManager)
	{
		manager = gManager;
	}

	private void Update()
	{
		if (calulateTime)
		{
			OnTick();
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		AlphaTween.enabled = true;
		AlphaTween.callWhenFinished = string.Empty;
		AlphaTween.Play(true);
	}

	public void Hide()
	{
		AlphaTween.callWhenFinished = "Disable";
		AlphaTween.Play(false);
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	public void OnAlphaTweenComplete()
	{
		calulateTime = true;
	}

	public void StartDisplayText(string text, float time, FontSize size)
	{
		if (manager != null)
		{
			manager.PlaySound(GuiInGameManager.Sound.MessageSound);
		}
		UIFont fontByEnum = GetFontByEnum(size);
		if (fontByEnum != null)
		{
			Label.font = fontByEnum;
			Label.MakePixelPerfect();
		}
		else
		{
			Label.transform.localScale = GetFontScaleByEnum(size);
		}
		timeLeft = time;
		Label.text = text;
		currentMessage = text;
		displayTime = false;
		Show();
		AlphaTween.callWhenFinished = "OnAlphaTweenComplete";
	}

	public void StartDisplayText(string text, float time)
	{
		StartDisplayText(text, time, FontSize.Normal);
	}

	public void StartDisplayTextWithTime(string text, float time, FontSize size)
	{
		if (manager != null)
		{
			manager.PlaySound(GuiInGameManager.Sound.MessageSound);
		}
		UIFont fontByEnum = GetFontByEnum(size);
		if (fontByEnum != null)
		{
			Label.font = fontByEnum;
			Label.MakePixelPerfect();
		}
		else
		{
			Label.transform.localScale = GetFontScaleByEnum(size);
		}
		timeLeft = time;
		Label.text = text + " " + time;
		currentMessage = text;
		displayTime = true;
		Show();
		AlphaTween.callWhenFinished = "OnAlphaTweenComplete";
	}

	public void StartDisplayTextWithTime(string text, float time)
	{
		StartDisplayTextWithTime(text, time, FontSize.Normal);
	}

	private UIFont GetFontByEnum(FontSize size)
	{
		UIFont result = null;
		switch (size)
		{
		case FontSize.Small:
			result = Font24;
			break;
		case FontSize.Normal:
			result = Font36;
			break;
		case FontSize.Large:
			result = Font48;
			break;
		}
		return result;
	}

	private Vector3 GetFontScaleByEnum(FontSize size)
	{
		Vector3 result = baseScale;
		switch (size)
		{
		case FontSize.Small:
			result = new Vector3(24f, 24f, 1f);
			break;
		case FontSize.Normal:
			result = new Vector3(36f, 36f, 1f);
			break;
		case FontSize.Large:
			result = new Vector3(48f, 48f, 1f);
			break;
		}
		return result;
	}

	public void StopDisplayText()
	{
		if (Font24 != null)
		{
			Label.font = Font24;
			Label.MakePixelPerfect();
		}
		else
		{
			Label.transform.localScale = baseScale;
		}
		calulateTime = false;
		displayTime = false;
		Label.text = string.Empty;
		Hide();
	}

	private void OnTick()
	{
		float num = Mathf.Ceil(timeLeft);
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0f)
		{
			StopDisplayText();
		}
		else if (displayTime)
		{
			if (Mathf.Ceil(timeLeft) < num && manager != null)
			{
				manager.PlaySound(GuiInGameManager.Sound.MessageSound);
			}
			Label.text = currentMessage + " " + Mathf.Ceil(timeLeft);
		}
	}
}
