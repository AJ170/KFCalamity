using UnityEngine;

public class LocalizeInputModeIcon : MonoBehaviour
{
	public string SymbolEntryName;

	public UISprite SpriteToLocalize;

	public UILabel Label1ToLocalize;

	public UILabel Label2ToLocalize;

	private void Awake()
	{
		if (string.IsNullOrEmpty(SymbolEntryName))
		{
			return;
		}
		SymbolReturnEntry symbolForCurrentInputMode = SymbolsLibrary.Instance.GetSymbolForCurrentInputMode(SymbolEntryName);
		if (symbolForCurrentInputMode != null)
		{
			if (symbolForCurrentInputMode.IsSprite)
			{
				if (SpriteToLocalize != null)
				{
					SpriteToLocalize.enabled = true;
					SpriteToLocalize.spriteName = symbolForCurrentInputMode.EntryValue;
					if (!string.IsNullOrEmpty(symbolForCurrentInputMode.InputText) && Label1ToLocalize != null)
					{
						Label1ToLocalize.enabled = true;
						Label1ToLocalize.text = symbolForCurrentInputMode.InputText;
					}
				}
				else
				{
					Debug.LogError("LocalizeInputModeIcon SymbolsLibrary returned a Sprite, but no Sprite Reference has been set!", base.gameObject);
				}
				return;
			}
			if (SpriteToLocalize != null)
			{
				SpriteToLocalize.enabled = false;
			}
			if (Label1ToLocalize != null)
			{
				Label1ToLocalize.enabled = true;
				Label1ToLocalize.text = symbolForCurrentInputMode.EntryValue;
			}
			else
			{
				Debug.LogError("LocalizeInputModeIcon SymbolsLibrary returned a string, but not Label Reference has been set!", base.gameObject);
			}
			if (!string.IsNullOrEmpty(symbolForCurrentInputMode.InputText) && Label2ToLocalize != null)
			{
				Label2ToLocalize.enabled = true;
				Label2ToLocalize.text = symbolForCurrentInputMode.InputText;
			}
		}
		else
		{
			Debug.LogError("LocalizeInputModeIcon could not find Symbol entry with the name: " + SymbolEntryName, base.gameObject);
		}
	}
}
