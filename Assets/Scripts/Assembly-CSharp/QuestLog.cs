using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour
{
	public GameObject QuestPrefab;

	public UITable questTable;

	private List<GameObject> questsEntries = new List<GameObject>();

	private void Start()
	{
	}

	public GameObject NewEntry(string text, string image, int maxCount)
	{
		GameObject gameObject = NewEntry(text, image);
		QuestEntry questEntry = NGUITools.FindInParents<QuestEntry>(gameObject);
		questEntry.SetMaxCounter(maxCount);
		return gameObject;
	}

	private string GetQuestTextForCurrentInput(string originalKey)
	{
		string empty = string.Empty;
		if (Localizer.HasKey(originalKey))
		{
			return Localizer.GetString(originalKey, true);
		}
		return Localizer.GetString(originalKey);
	}

	public GameObject NewEntry(string text, string symbolLibraryKey)
	{
		GameObject gameObject = NGUITools.AddChild(questTable.gameObject, QuestPrefab);
		QuestEntry questEntry = NGUITools.FindInParents<QuestEntry>(gameObject);
		questEntry.SetText(GetQuestTextForCurrentInput(text));
		SymbolReturnEntry symbolForCurrentInputMode = SymbolsLibrary.Instance.GetSymbolForCurrentInputMode(symbolLibraryKey);
		questEntry.SetSymbol(symbolForCurrentInputMode);
		questsEntries.Add(gameObject);
		questTable.Reposition();
		return gameObject;
	}

	private string GetImageNameBasedOnControls(string image)
	{
		switch (image)
		{
		case "q_left_bumper":
			image = "q_left_trigger";
			break;
		case "q_left_trigger":
			image = "q_left_bumper";
			break;
		case "q_right_bumper":
			image = "q_right_trigger";
			break;
		case "q_right_trigger":
			image = "q_right_bumper";
			break;
		}
		return image;
	}

	public void SetEntrySymbol(GameObject entry, SymbolReturnEntry symbolEntry)
	{
		QuestEntry questEntry = NGUITools.FindInParents<QuestEntry>(entry);
		if (questEntry != null)
		{
			questEntry.SetSymbol(symbolEntry);
		}
	}

	public void SetEntryTimer(GameObject entry, int seconds)
	{
		QuestEntry questEntry = NGUITools.FindInParents<QuestEntry>(entry);
		if (questEntry != null)
		{
			questEntry.SetTimer(seconds);
		}
	}

	public void SetEntryCounter(GameObject entry, int amount)
	{
		QuestEntry questEntry = NGUITools.FindInParents<QuestEntry>(entry);
		if (questEntry != null)
		{
			questEntry.SetCounter(amount);
		}
	}

	public void SetEntryMaxCounter(GameObject entry, int amount)
	{
		QuestEntry questEntry = NGUITools.FindInParents<QuestEntry>(entry);
		if (questEntry != null)
		{
			questEntry.SetMaxCounter(amount);
		}
	}

	public void SetEntryDone(GameObject entry)
	{
		QuestEntry questEntry = NGUITools.FindInParents<QuestEntry>(entry);
		if (questEntry != null)
		{
			questEntry.SetCheckbox(true);
		}
	}

	public void RemoveEntry(GameObject entry)
	{
		questsEntries.Remove(entry);
		NGUITools.Destroy(entry);
	}

	public void ClearLog()
	{
		foreach (GameObject questsEntry in questsEntries)
		{
			NGUITools.Destroy(questsEntry);
		}
		questsEntries.Clear();
	}
}
