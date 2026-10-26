using System.Collections.Generic;
using UnityEngine;

public class StatisticsPagePerk : GuiListButtonLevel
{
	public UILabel LabelTitle;

	public UILabel LabelTitleAbility;

	public UILabel LabelTitleBonus;

	public UILabel LabelTitleBonusAdvanced;

	public UILabel LabelDescAbility;

	public UILabel LabelDescBonus;

	public UILabel LabelDescBonusAdvanced;

	public UILabel LabelXp;

	public UISprite SpritePerk;

	public List<TweenAlpha> AlphaTweens;

	public UISlider XpSlider;

	public GameObject TouchPressObject;

	private void Start()
	{
		alphaTween = base.gameObject.GetComponent<TweenAlpha>();
		positionTween = base.gameObject.GetComponent<TweenPosition>();
		scaleTween = base.gameObject.GetComponent<TweenScale>();
	}

	public void Initialize()
	{
		startSize = new Vector3(0.6f, 0.6f, 0f);
		maxSize = new Vector3(1f, 1f, 0f);
	}

	private void Update()
	{
	}

	public void TweenAlpha(float amount, float duration)
	{
		for (int i = 0; i < AlphaTweens.Count; i++)
		{
			if (AlphaTweens[i] != null)
			{
				AlphaTweens[i].to = amount;
				AlphaTweens[i].from = AlphaTweens[i].alpha;
				AlphaTweens[i].duration = duration;
				AlphaTweens[i].Play(true);
			}
		}
	}

	public override void SetAlpha(float amount)
	{
		for (int i = 0; i < AlphaTweens.Count; i++)
		{
			if (AlphaTweens[i] != null)
			{
				AlphaTweens[i].to = amount;
				AlphaTweens[i].from = amount;
				AlphaTweens[i].duration = 0.1f;
				AlphaTweens[i].Play(true);
			}
		}
	}

	public void SetXP(float value)
	{
		if (XpSlider != null)
		{
			XpSlider.sliderValue = value;
		}
	}

	public override void OnSelect()
	{
		base.OnSelect();
	}

	public override void OnDeselect()
	{
		base.OnDeselect();
	}

	public override void SetButtonList(GuiList buttonList)
	{
		base.SetButtonList(buttonList);
	}

	public override void SetDepth(int depthIndex)
	{
		base.SetDepth(depthIndex);
	}

	public override void SetImage(string image)
	{
		SpritePerk.spriteName = image;
		SpritePerk.MakePixelPerfect();
	}

	public void SetXpText(int current, int total)
	{
		if (current == 0 && total == 0)
		{
			LabelXp.text = "Max level";
		}
		else
		{
			LabelXp.text = current + " / " + total + " XP";
		}
	}

	public void SetAbilityTitle(string text)
	{
		LabelTitleAbility.text = text;
	}

	public void SetBonusTitle(string text)
	{
		LabelTitleBonus.text = text;
	}

	public void SetAdvancedBonusTitle(string text)
	{
		LabelTitleBonusAdvanced.text = text;
	}

	public void SetAbilityDescription(string text)
	{
		LabelDescAbility.text = text;
	}

	public void SetBonusDescription(string text)
	{
		LabelDescBonus.text = text;
	}

	public void SetAdvancedBonusDescription(string text)
	{
		LabelDescBonusAdvanced.text = text;
	}

	public override void SetPosition(Vector3 position)
	{
		base.SetPosition(position);
	}

	public override void SetText(string text)
	{
		LabelTitle.text = text;
	}

	public override void OnAlphaZero()
	{
		positionTween.Reset();
		for (int i = 0; i < AlphaTweens.Count; i++)
		{
			if (AlphaTweens[i] != null)
			{
				AlphaTweens[i].Reset();
			}
		}
		hasMoved = true;
	}

	public override void Tween(Vector3 newPosition, TweenDirection direction, int amount, float alphaAmount, float tweenSpeed)
	{
		if (sprite == null)
		{
			sprite = base.gameObject.GetComponent<UISprite>();
		}
		if (positionTween == null)
		{
			positionTween = base.gameObject.GetComponent<TweenPosition>();
		}
		if (scaleTween == null)
		{
			scaleTween = base.gameObject.GetComponent<TweenScale>();
		}
		if (alphaTween == null)
		{
			alphaTween = base.gameObject.GetComponent<TweenAlpha>();
		}
		hasMoved = false;
		Vector3 localPosition = base.transform.localPosition;
		float num = 0f;
		for (int i = 0; i < AlphaTweens.Count; i++)
		{
			if (AlphaTweens[i] != null)
			{
				num = AlphaTweens[i].alpha;
				AlphaTweens[i].eventReceiver = null;
				AlphaTweens[i].Reset2();
			}
		}
		if (alphaAmount == 1f)
		{
			scaleTween.Reset2();
			scaleTween.duration = tweenSpeed;
			scaleTween.to = maxSize;
			scaleTween.from = startSize;
			scaleTween.Play(true);
			if (alphaTween.eventReceiver == null)
			{
				alphaTween.eventReceiver = buttonList.gameObject;
			}
			for (int j = 0; j < AlphaTweens.Count; j++)
			{
				if (AlphaTweens[j] != null)
				{
				}
			}
		}
		else if (num == 1f)
		{
			scaleTween.Reset2();
			scaleTween.duration = tweenSpeed;
			scaleTween.to = startSize;
			scaleTween.from = maxSize;
			scaleTween.Play(true);
		}
		positionTween.Reset();
		positionTween.duration = tweenSpeed;
		positionTween.to = newPosition;
		base.transform.localPosition = localPosition;
		positionTween.from = localPosition;
		for (int k = 0; k < AlphaTweens.Count; k++)
		{
			if (AlphaTweens[k] != null)
			{
				AlphaTweens[k].from = num;
				AlphaTweens[k].duration = tweenSpeed;
				AlphaTweens[k].to = alphaAmount;
				AlphaTweens[k].Play(true);
			}
		}
		positionTween.Play(true);
	}
}
