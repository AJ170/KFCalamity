using System;
using UnityEngine;

public class QuestEntry : MonoBehaviour
{
	[Serializable]
	public class SpriteObject
	{
		public string Name;

		public string spriteName;

		public string InputText = string.Empty;
	}

	public enum EntryType
	{
		Counter = 0,
		Timer = 1
	}

	public UICheckbox Checkbox;

	public UILabel Label;

	public UILabel LabelInput;

	public UISprite SymbolSprite;

	public UILabel SymbolLabel;

	private int current;

	private int total;

	private EntryType entryType;

	public string SymbolName
	{
		get
		{
			return SymbolSprite.spriteName;
		}
		set
		{
			SymbolSprite.spriteName = value;
		}
	}

	private void Start()
	{
	}

	public void SetTimer(int value)
	{
		current = value;
		SetCountText(EntryType.Timer);
	}

	public void SetCounter(int value)
	{
		current = value;
		SetCountText(EntryType.Counter);
	}

	public void SetMaxCounter(int max)
	{
		total = max;
		SetCountText(EntryType.Counter);
	}

	private void SetCountText(EntryType entryType)
	{
		UILabel componentInChildren = Checkbox.GetComponentInChildren<UILabel>();
		if (componentInChildren != null)
		{
			switch (entryType)
			{
			case EntryType.Counter:
				componentInChildren.text = current + " / " + total;
				break;
			case EntryType.Timer:
				componentInChildren.text = string.Format("{0:00}:{1:00}", current / 60, current);
				break;
			}
		}
	}

	public void SetSymbol(SymbolReturnEntry entry)
	{
		if (entry.IsSprite)
		{
			SymbolSprite.enabled = true;
			SymbolSprite.spriteName = entry.EntryValue;
			SymbolSprite.MakePixelPerfect();
		}
		else
		{
			SymbolLabel.enabled = true;
			SymbolLabel.text = entry.EntryValue;
		}
		if (!string.IsNullOrEmpty(entry.InputText))
		{
			LabelInput.text = entry.InputText;
		}
	}

	public void SetText(string text)
	{
		Label.text = text;
	}

	public void SetCheckbox(bool isDone)
	{
		if (Checkbox != null)
		{
			TweenScale component = Checkbox.checkSprite.GetComponent<TweenScale>();
			if (component != null)
			{
				component.gameObject.SetActive(true);
				component.Reset();
				component.Play(true);
			}
			Checkbox.isChecked = isDone;
		}
	}
}
