using System.Collections.Generic;
using UnityEngine;

public class GuiList : MonoBehaviour
{
	public class ListButton
	{
		public GameObject Object;

		public bool Selected;

		public bool Visible;

		public int position;
	}

	public class TweenValue
	{
		public Vector3 position;

		public Vector3 scale;

		public float Alpha;
	}

	public GameObject List;

	public GameObject ButtonPrefab;

	public int TweenDistance = 200;

	private List<ListButton> buttons = new List<ListButton>();

	private bool isButtonPressed;

	private ListButton currentSelected;

	public float TweenSpeed = 0.3f;

	private int visibleButtons;

	private int buttonCount;

	private int middlePosition;

	private MenuBase manager;

	private List<TweenValue> TweenValues = new List<TweenValue>();

	private float blendMultiplier = 0.25f;

	private void Start()
	{
	}

	public void SetManagerReference(MenuBase manager)
	{
		this.manager = manager;
	}

	public List<TweenValue> GetTweenValues()
	{
		return TweenValues;
	}

	public List<ListButton> Buttons()
	{
		return buttons;
	}

	public ListButton CurrentSelected()
	{
		return currentSelected;
	}

	public List<ListButton> Create(int visibleButtons, int buttonCount, float blendValue)
	{
		blendMultiplier = blendValue;
		this.visibleButtons = visibleButtons;
		this.buttonCount = buttonCount + 1;
		for (int i = 0; i < this.buttonCount; i++)
		{
			AddButton();
		}
		return buttons;
	}

	public void SetSelection(int axis)
	{
		if (isButtonPressed)
		{
			return;
		}
		isButtonPressed = true;
		for (int i = 0; i < buttonCount; i++)
		{
			if (buttons[i].Selected)
			{
				OnDeselect(buttons[i]);
				if (i + axis < 0)
				{
					OnSelect(buttons[buttons.Count + (i + axis)]);
				}
				else if (i + axis > buttons.Count - 1)
				{
					OnSelect(buttons[(buttons.Count + axis) % buttons.Count]);
				}
				else
				{
					OnSelect(buttons[i + axis]);
				}
				break;
			}
		}
		if (axis > 0)
		{
			MoveList(TweenDirection.Left, Mathf.Abs(axis));
		}
		else if (axis < 0)
		{
			MoveList(TweenDirection.Right, Mathf.Abs(axis));
		}
	}

	public int GetButtonCount()
	{
		if (buttons != null)
		{
			return buttons.Count;
		}
		return 0;
	}

	public void AddButton()
	{
		ListButton listButton = new ListButton();
		listButton.Object = NGUITools.AddChild(List.gameObject, ButtonPrefab);
		listButton.Object.transform.localPosition = Vector3.zero;
		buttons.Add(listButton);
	}

	public void AdjustButtonPositions()
	{
		middlePosition = buttonCount / 2;
		int num = -(buttonCount / 2 * TweenDistance);
		float alpha = 0f;
		for (int i = 0; i < buttonCount; i++)
		{
			TweenValue tweenValue = new TweenValue();
			tweenValue.position = new Vector3(num, 0f, 0f);
			if (i < middlePosition)
			{
				alpha = ((i >= middlePosition - visibleButtons) ? (1f - (float)(middlePosition - i) * blendMultiplier) : 0f);
			}
			else if (i > middlePosition)
			{
				alpha = ((i <= middlePosition + visibleButtons) ? (1f - (float)(i - middlePosition) * blendMultiplier) : 0f);
			}
			if (i == middlePosition)
			{
				alpha = 1f;
			}
			tweenValue.Alpha = alpha;
			TweenValues.Add(tweenValue);
			buttons[i].position = i;
			buttons[i].Visible = true;
			if (num == 0)
			{
				OnSelect(buttons[i]);
			}
			num += TweenDistance;
			buttons[i].Object.SetActive(true);
		}
	}

	public void MoveList(TweenDirection direction, int steps = 1)
	{
		switch (direction)
		{
		case TweenDirection.Left:
		{
			for (int k = 0; k < buttonCount; k++)
			{
				if (currentSelected == buttons[k])
				{
					OnDeselect(buttons[k]);
					OnSelect(buttons[(k + steps) % buttonCount]);
					break;
				}
			}
			for (int l = 0; l < buttonCount; l++)
			{
				buttons[l].position -= steps;
				if (buttons[l].position < 0)
				{
					buttons[l].position = buttonCount + buttons[l].position;
				}
			}
			break;
		}
		case TweenDirection.Right:
		{
			for (int i = 0; i < buttonCount; i++)
			{
				if (currentSelected == buttons[i])
				{
					OnDeselect(buttons[i]);
					if (i - steps < 0)
					{
						OnSelect(buttons[buttonCount + (i - steps)]);
					}
					else
					{
						OnSelect(buttons[i - steps]);
					}
					break;
				}
			}
			for (int j = 0; j < buttonCount; j++)
			{
				buttons[j].position = (buttons[j].position + steps) % buttonCount;
			}
			break;
		}
		}
		for (int m = 0; m < buttonCount; m++)
		{
			manager.TweenButton(buttons[m].Object, TweenValues[buttons[m].position].position, direction, TweenDistance * steps, TweenValues[buttons[m].position].Alpha, TweenSpeed);
		}
	}

	public void OnTweenComplete()
	{
		isButtonPressed = false;
		manager.OnTweenComplete();
	}

	public void OnSelect(ListButton selected)
	{
		currentSelected = selected;
	}

	public void OnDeselect(ListButton selected)
	{
	}
}
