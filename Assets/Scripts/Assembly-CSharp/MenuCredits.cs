using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class MenuCredits : MenuBase
{
	public enum Positions
	{
		Lead_Programmer = 0,
		Lead_Artist = 1,
		Lead_Designer = 2,
		Producer = 3,
		Programmer = 4,
		Designer = 5,
		Artist = 6
	}

	[Serializable]
	public class CreditPosition
	{
		public bool UseTitle;

		public bool UsePosition;

		public string Position;

		public string[] Names;
	}

	public GameObject EntryPrefab;

	public TweenAlpha Alpha;

	public List<CreditPosition> CreditList;

	public List<string> Titles;

	public UIFont TitleFont;

	private List<GameObject> entries = new List<GameObject>();

	private int entryGroupIndex;

	private int titleIndex;

	private int destroyedEntries;

	private int entryNameIndex;

	private int spawns;

	private Vector3 spawnPoint = new Vector3(0f, -800f, 0f);

	private void Start()
	{
		HandleTextSpawn();
	}

	private void Update()
	{
		HandleInput();
	}

	private void HandleTextSpawn()
	{
		CreditPosition creditPosition = CreditList[entryGroupIndex];
		if (creditPosition.UseTitle)
		{
			Invoke("SpawnTextTitle", 1.5f);
		}
		else if (creditPosition.UsePosition)
		{
			Invoke("SpawnPositionText", 0.6f);
		}
		else if (CreditList[entryGroupIndex].Names.Length > 0)
		{
			Invoke("SpawnTextLine", 0.4f);
		}
	}

	private void SpawnPositionText()
	{
		CreditPosition creditPosition = CreditList[entryGroupIndex];
		GameObject gameObject = NGUITools.AddChild(base.gameObject, EntryPrefab);
		gameObject.gameObject.transform.position = spawnPoint;
		UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
		TweenPosition component = gameObject.GetComponent<TweenPosition>();
		component.eventReceiver = base.gameObject;
		componentInChildren.text = creditPosition.Position;
		componentInChildren.color = Color.yellow;
		component.enabled = true;
		entries.Add(gameObject);
		spawns++;
		if (CreditList[entryGroupIndex].Names.Length > 0)
		{
			Invoke("SpawnTextLine", 0.4f);
		}
	}

	private void SpawnTextTitle()
	{
		CreditPosition creditPosition = CreditList[entryGroupIndex];
		GameObject gameObject = NGUITools.AddChild(base.gameObject, EntryPrefab);
		gameObject.gameObject.transform.position = spawnPoint;
		UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
		TweenPosition component = gameObject.GetComponent<TweenPosition>();
		component.eventReceiver = base.gameObject;
		componentInChildren.text = Titles[titleIndex];
		componentInChildren.color = Color.yellow;
		componentInChildren.font = TitleFont;
		componentInChildren.MakePixelPerfect();
		component.enabled = true;
		entries.Add(gameObject);
		titleIndex++;
		spawns++;
		if (creditPosition.UsePosition)
		{
			Invoke("SpawnPositionText", 0.6f);
		}
		else if (CreditList[entryGroupIndex].Names.Length > 0)
		{
			Invoke("SpawnTextLine", 0.6f);
		}
		else
		{
			SpawnNext();
		}
	}

	private void TweenOut()
	{
		Alpha.callWhenFinished = "LoadState";
		Alpha.Play(false);
	}

	private void SpawnTextLine()
	{
		GameObject gameObject = NGUITools.AddChild(base.gameObject, EntryPrefab);
		gameObject.gameObject.transform.position = spawnPoint;
		gameObject.GetComponent<TweenPosition>().eventReceiver = base.gameObject;
		gameObject.GetComponentInChildren<UILabel>().text = CreditList[entryGroupIndex].Names[entryNameIndex].ToString();
		gameObject.GetComponent<TweenPosition>().enabled = true;
		entries.Add(gameObject);
		spawns++;
		SpawnNext();
	}

	private void SpawnNext()
	{
		if (entryNameIndex < CreditList[entryGroupIndex].Names.Length - 1)
		{
			entryNameIndex++;
		}
		else
		{
			entryGroupIndex++;
			entryNameIndex = 0;
		}
		if (entryGroupIndex <= CreditList.Count - 1)
		{
			if (entryNameIndex != 0)
			{
				Invoke("SpawnTextLine", 0.4f);
			}
			else
			{
				HandleTextSpawn();
			}
		}
	}

	public void OnEntryComplete(UITweener tweener)
	{
		UnityEngine.Object.Destroy(tweener.gameObject);
		destroyedEntries++;
		if (destroyedEntries == spawns)
		{
			TweenOut();
		}
	}

	private void HandleInput()
	{
		if (InputManager.GetBool(InputAction.MenuForward) || InputManager.GetBool(InputAction.MenuBackward) || InputManager.GetBool(InputAction.SkipSplashScreen))
		{
			TweenOut();
		}
	}

	public void LoadState()
	{
		if (manager != null)
		{
			manager.ChangeGui(MenuStates.Main);
		}
	}

	public override void OnTweenComplete()
	{
		base.OnTweenComplete();
	}

	public override void SetManagerReference(GuiMenuManager manager)
	{
		base.SetManagerReference(manager);
	}
}
