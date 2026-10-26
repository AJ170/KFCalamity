using UnityEngine;

public class GUICustomLocalize : MonoBehaviour
{
	public enum GUILetterStyle
	{
		Default = 0,
		UpperCase = 1,
		LowerCase = 2
	}

	public string Key = string.Empty;

	public LocalizationLanguage Language = LocalizationLanguage.English;

	public GUILetterStyle LetterStyle;

	private bool started;

	private void Localize()
	{
		UIWidget component = GetComponent<UIWidget>();
		UILabel uILabel = component as UILabel;
		UISprite uISprite = component as UISprite;
		if (string.IsNullOrEmpty(Key) && uILabel != null)
		{
			Key = uILabel.text;
		}
		string text = ((!string.IsNullOrEmpty(Key)) ? Localizer.GetString(Key) : string.Empty);
		if (LetterStyle == GUILetterStyle.UpperCase)
		{
			text = text.ToUpper();
		}
		else if (LetterStyle == GUILetterStyle.LowerCase)
		{
			text = text.ToLower();
		}
		if (uILabel != null)
		{
			uILabel.text = text;
			uILabel.MakePositionPerfect();
			uILabel.MakePixelPerfect();
		}
		else if (uISprite != null)
		{
			uISprite.spriteName = text;
			uISprite.MakePixelPerfect();
		}
	}

	private void OnEnable()
	{
		if (started && Localizer.Instance.Initialized)
		{
			Localize();
		}
	}

	private void Start()
	{
		started = true;
		if (Localizer.Instance.Initialized)
		{
			Localize();
		}
	}
}
