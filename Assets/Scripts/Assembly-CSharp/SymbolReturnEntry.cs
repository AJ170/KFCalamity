public class SymbolReturnEntry
{
	public bool IsSprite;

	public string EntryValue;

	public string InputText;

	public SymbolReturnEntry(string text, string inputText, bool isSprite)
	{
		EntryValue = text;
		InputText = inputText;
		IsSprite = isSprite;
	}
}
