using System.Collections.Generic;
using UnityEngine;

public class KillStreak : MonoBehaviour
{
	public enum BackgroundState
	{
		none = 0,
		normal = 1,
		medium = 2,
		high = 3
	}

	public Transform Background;

	public Transform TextAmount;

	public Transform TextMoney;

	public TweenColor ColorTween;

	private TweenScale labelAmountScale;

	private TweenScale spriteBackgroundScale;

	private SortedList<BackgroundState, Color> colors = new SortedList<BackgroundState, Color>(4);

	private UILabel labelAmount;

	private UILabel labelMoney;

	private int currentKillStreak;

	private float currentMoneyMultiplier;

	private BackgroundState backgroundState;

	private Color activeColor = Color.white;

	public BackgroundState ActiveState
	{
		get
		{
			return backgroundState;
		}
		set
		{
			backgroundState = value;
		}
	}

	public Color ActiveColor
	{
		get
		{
			return activeColor;
		}
		set
		{
			activeColor = value;
		}
	}

	private void Start()
	{
		colors.Add(BackgroundState.normal, new Color(1f, 1f, 0.352f));
		colors.Add(BackgroundState.medium, new Color(1f, 0.784f, 0.196f));
		colors.Add(BackgroundState.high, new Color(1f, 0.49f, 0.039f));
		backgroundState = BackgroundState.none;
		if (TextAmount != null)
		{
			labelAmount = TextAmount.GetComponent<UILabel>();
			labelAmountScale = TextAmount.GetComponent<TweenScale>();
		}
		else
		{
			MonoBehaviour.print("<KillStreak> Could not find 'TextAmount' reference.");
		}
		if (Background != null)
		{
			spriteBackgroundScale = Background.GetComponent<TweenScale>();
			Background.gameObject.SetActive(false);
		}
		else
		{
			MonoBehaviour.print("<KillStreak> Could not find 'Background' reference.");
		}
		if (TextMoney != null)
		{
			labelMoney = TextMoney.GetComponent<UILabel>();
			labelMoney.text = Localizer.GetString("gui_hud_killstreak");
			labelMoney.enabled = false;
		}
		else
		{
			MonoBehaviour.print("<KillStreak> Could not find 'TextMoney' reference.");
		}
		if (currentKillStreak != 0)
		{
			SetKillStreak(currentKillStreak, currentMoneyMultiplier);
		}
	}

	public string GetCurrentHexCodeColor()
	{
		switch (ActiveState)
		{
		case BackgroundState.normal:
			return "[FFFF5A]";
		case BackgroundState.medium:
			return "[FFC832]";
		case BackgroundState.high:
			return "[FF7D0A]";
		default:
			return string.Empty;
		}
	}

	public void OnColorChange()
	{
		Color to = ColorTween.to;
		ColorTween.Reset2();
		ColorTween.from = to;
		ColorTween.to = new Color(Random.value, Random.value, Random.value, 1f);
		ColorTween.Play(true);
	}

	private void ChangeBackgroundState(BackgroundState state, float moneyMultiplier)
	{
		if (state != backgroundState)
		{
			spriteBackgroundScale.Reset();
			switch (state)
			{
			case BackgroundState.normal:
				Background.gameObject.SetActive(true);
				break;
			case BackgroundState.medium:
				Background.gameObject.SetActive(true);
				spriteBackgroundScale.Play(true);
				break;
			case BackgroundState.high:
				Background.gameObject.SetActive(true);
				spriteBackgroundScale.Play(true);
				break;
			}
			backgroundState = state;
		}
	}

	public void SetKillStreak(int killCount, float moneyMultiplier)
	{
		if (killCount >= 10)
		{
			labelAmount.text = killCount.ToString();
			labelMoney.enabled = true;
			base.gameObject.SetActive(true);
		}
		else
		{
			labelAmount.text = string.Empty;
			labelMoney.enabled = false;
			base.gameObject.SetActive(false);
		}
		currentKillStreak = killCount;
		currentMoneyMultiplier = moneyMultiplier;
		if (killCount >= 100)
		{
			ColorTween.from = labelAmount.color;
			ColorTween.enabled = true;
			ColorTween.Play(true);
		}
		else
		{
			ColorTween.Reset();
			ColorTween.from = labelAmount.color;
			ColorTween.enabled = false;
		}
		labelAmountScale.Reset();
		labelAmountScale.Play(true);
		if (killCount >= 10 && currentKillStreak < 20)
		{
			SetColor(BackgroundState.normal);
			activeColor = labelAmount.color;
			ChangeBackgroundState(BackgroundState.normal, moneyMultiplier);
		}
		if (currentKillStreak >= 20 && currentKillStreak < 30)
		{
			SetColor(BackgroundState.medium);
			ChangeBackgroundState(BackgroundState.medium, moneyMultiplier);
		}
		if (currentKillStreak >= 30)
		{
			SetColor(BackgroundState.high);
			ChangeBackgroundState(BackgroundState.high, moneyMultiplier);
		}
	}

	private void SetColor(BackgroundState state)
	{
		if (state != 0)
		{
			labelAmount.color = colors[state];
			ActiveColor = colors[state];
		}
	}

	public void Reset()
	{
		labelAmount.color = Color.white;
		if (ColorTween != null)
		{
			ColorTween.Reset();
			ColorTween.from = Color.white;
			ColorTween.enabled = false;
		}
		backgroundState = BackgroundState.none;
		currentKillStreak = 0;
		currentMoneyMultiplier = 0f;
		if (Background != null)
		{
			Background.gameObject.SetActive(false);
		}
		labelAmountScale.Reset();
		spriteBackgroundScale.Reset();
		labelAmount.text = string.Empty;
		if (labelMoney != null)
		{
			labelMoney.enabled = false;
		}
		if ((bool)this)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SyncGui(KillStreak killStreak)
	{
		currentKillStreak = killStreak.currentKillStreak;
		currentMoneyMultiplier = killStreak.currentMoneyMultiplier;
	}
}
