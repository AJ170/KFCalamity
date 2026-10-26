using System;

public class CheatDirectionEntry : CheatEntryBase
{
	private int correctDirection;

	private CheatDirection[] directions;

	public CheatDirectionEntry(CheatDirection direction)
	{
		directions = (CheatDirection[])Enum.GetValues(typeof(CheatDirection));
		correctDirection = Array.FindIndex(directions, (CheatDirection a) => a == direction);
	}

	private bool CheckDirection(CheatDirection direction, int horAxis, int vertAxis)
	{
		switch (direction)
		{
		case CheatDirection.Up:
			return horAxis == 0 && vertAxis > 0;
		case CheatDirection.Right:
			return horAxis > 0 && vertAxis == 0;
		case CheatDirection.Down:
			return horAxis == 0 && vertAxis < 0;
		case CheatDirection.Left:
			return horAxis < 0 && vertAxis == 0;
		default:
			return false;
		}
	}

	public override CheatEntryReturnStatus Check(int horAxis, int vertAxis)
	{
		CheatEntryReturnStatus result = CheatEntryReturnStatus.NoInput;
		if (CheckDirection(directions[correctDirection], horAxis, vertAxis))
		{
			result = CheatEntryReturnStatus.Success;
		}
		for (int i = 0; i < directions.Length; i++)
		{
			if (i != correctDirection && CheckDirection(directions[i], horAxis, vertAxis))
			{
				result = CheatEntryReturnStatus.WrongInput;
			}
		}
		return result;
	}
}
