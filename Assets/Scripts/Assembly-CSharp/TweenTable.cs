using System;
using System.Collections.Generic;
using UnityEngine;

public class TweenTable : MonoBehaviour
{
	[Serializable]
	public class ElementSize
	{
		public float Width;

		public float Height;
	}

	public ElementSize GridSize;

	public float Spacing;

	public TweenPosition PositionTween;

	public TweenPosition Row1Tween;

	public TweenPosition Row2Tween;

	private List<GameObject> elements = new List<GameObject>();

	private List<GameObject> elementsRow2 = new List<GameObject>();

	private bool moving = true;

	private int maxRowLength = 9;

	private int maxRowElements = 8;

	private int moveCount;

	public void HandleHorizontalInput(int axis)
	{
		if (axis == 0 || moving)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		if ((axis > 0 && moveCount > 0) || (axis < 0 && moveCount < elements.Count - maxRowElements))
		{
			PositionTween.method = UITweener.Method.EaseOut;
			PositionTween.eventReceiver = base.gameObject;
			PositionTween.callWhenFinished = "OnMovementEnded";
			if (axis > 0)
			{
				Vector3 to = new Vector3(localPosition.x + GridSize.Width, localPosition.y, localPosition.z);
				PositionTween.from = localPosition;
				PositionTween.to = to;
				moveCount--;
			}
			else if (axis < 0)
			{
				Vector3 to = new Vector3(localPosition.x - GridSize.Width, localPosition.y, localPosition.z);
				PositionTween.from = localPosition;
				PositionTween.to = to;
				moveCount++;
			}
			moving = true;
			PositionTween.Reset2();
			PositionTween.enabled = true;
		}
	}

	public void OnMovementEnded()
	{
		moving = false;
	}

	public GameObject AddInstantly(ref GameObject AchievementPrefab)
	{
		bool flag = false;
		if (elements.Count >= maxRowElements)
		{
			if (elementsRow2.Count < maxRowElements)
			{
				flag = true;
			}
			else if (elements.Count > elementsRow2.Count)
			{
				flag = true;
			}
		}
		GameObject gameObject;
		if (flag)
		{
			gameObject = NGUITools.AddChild(Row2Tween.gameObject, AchievementPrefab);
			Vector3 localPosition = Row2Tween.gameObject.transform.localPosition;
			for (int i = 0; i < elementsRow2.Count; i++)
			{
				Vector3 localPosition2 = elementsRow2[i].transform.localPosition;
				elementsRow2[i].transform.localPosition = new Vector3(localPosition2.x + GridSize.Width, localPosition2.y, localPosition2.z);
			}
			gameObject.transform.localPosition = localPosition;
			elementsRow2.Add(gameObject);
		}
		else
		{
			gameObject = NGUITools.AddChild(Row1Tween.gameObject, AchievementPrefab);
			Vector3 localPosition = Row1Tween.gameObject.transform.localPosition;
			for (int j = 0; j < elements.Count; j++)
			{
				Vector3 localPosition3 = elements[j].transform.localPosition;
				elements[j].transform.localPosition = new Vector3(localPosition3.x + GridSize.Width, localPosition3.y, localPosition3.z);
			}
			gameObject.transform.localPosition = localPosition;
			elements.Add(gameObject);
		}
		return gameObject;
	}

	public GameObject Add(ref GameObject AchievementPrefab)
	{
		bool flag = false;
		if (elements.Count >= maxRowElements)
		{
			if (elementsRow2.Count < maxRowElements)
			{
				flag = true;
			}
			else if (elements.Count > elementsRow2.Count)
			{
				flag = true;
			}
		}
		GameObject gameObject;
		if (flag)
		{
			gameObject = NGUITools.AddChild(Row2Tween.gameObject, AchievementPrefab);
			Vector3 localPosition = Row2Tween.gameObject.transform.localPosition;
			Vector3 vector = new Vector3(localPosition.x - GridSize.Width, localPosition.y, localPosition.z);
			Row2Tween.transform.localPosition = vector;
			Row2Tween.enabled = true;
			Row2Tween.Reset2();
			Row2Tween.from = vector;
			Row2Tween.to = localPosition;
			for (int i = 0; i < elementsRow2.Count; i++)
			{
				Vector3 localPosition2 = elementsRow2[i].transform.localPosition;
				elementsRow2[i].transform.localPosition = new Vector3(localPosition2.x + GridSize.Width, localPosition2.y, localPosition2.z);
			}
			gameObject.transform.localPosition = localPosition;
			elementsRow2.Add(gameObject);
			if (maxRowLength >= elementsRow2.Count)
			{
				moving = false;
			}
		}
		else
		{
			gameObject = NGUITools.AddChild(Row1Tween.gameObject, AchievementPrefab);
			Vector3 localPosition = Row1Tween.gameObject.transform.localPosition;
			Vector3 vector = new Vector3(localPosition.x - GridSize.Width, localPosition.y, localPosition.z);
			Row1Tween.transform.localPosition = vector;
			Row1Tween.enabled = true;
			Row1Tween.Reset2();
			Row1Tween.from = vector;
			Row1Tween.to = localPosition;
			for (int j = 0; j < elements.Count; j++)
			{
				Vector3 localPosition3 = elements[j].transform.localPosition;
				elements[j].transform.localPosition = new Vector3(localPosition3.x + GridSize.Width, localPosition3.y, localPosition3.z);
			}
			gameObject.transform.localPosition = localPosition;
			elements.Add(gameObject);
			if (maxRowLength >= elements.Count)
			{
				moving = false;
			}
		}
		return gameObject;
	}
}
