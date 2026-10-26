using System.Collections.Generic;
using UnityEngine;

public class AttributeModifier : AttributeBase
{
	private int fixedValue;

	private float multiplier = 1f;

	private int defaultFixed;

	private float defaultMultiplier = 1f;

	private Dictionary<object, int> values = new Dictionary<object, int>();

	private Dictionary<object, float> multipliers = new Dictionary<object, float>();

	private bool recalculate;

	public int Fixed
	{
		get
		{
			return fixedValue;
		}
	}

	public float Multiplier
	{
		get
		{
			return multiplier;
		}
	}

	public AttributeModifier(AttributeName name)
		: base(AttributeType.Modifier, name)
	{
	}

	public AttributeModifier(AttributeName name, int defaultFixed, float defaultMultiplier)
		: base(AttributeType.Modifier, name)
	{
		this.defaultFixed = defaultFixed;
		fixedValue = defaultFixed;
		this.defaultMultiplier = defaultMultiplier;
		multiplier = defaultMultiplier;
	}

	public void Alter(object alterer, float multiplier)
	{
		multiplier = Mathf.Clamp(multiplier, 0f, multiplier);
		if (multipliers.ContainsKey(alterer))
		{
			float num = multipliers[alterer];
			multipliers[alterer] = num * multiplier;
		}
		else
		{
			multipliers.Add(alterer, multiplier);
		}
		recalculate = true;
	}

	public void Alter(object alterer, int value)
	{
		if (values.ContainsKey(alterer))
		{
			int num = values[alterer];
			values[alterer] = num + value;
		}
		else
		{
			values.Add(alterer, value);
		}
		recalculate = true;
	}

	public void Restore(object alterer)
	{
		multipliers.Remove(alterer);
		values.Remove(alterer);
		recalculate = true;
	}

	public void Recalculate()
	{
		if (recalculate)
		{
			RecalculateMultipliers();
			RecalculateValues();
			recalculate = false;
		}
	}

	private void RecalculateMultipliers()
	{
		multiplier = defaultMultiplier;
		foreach (KeyValuePair<object, float> multiplier in multipliers)
		{
			this.multiplier *= multiplier.Value;
		}
	}

	private void RecalculateValues()
	{
		fixedValue = defaultFixed;
		foreach (KeyValuePair<object, int> value in values)
		{
			fixedValue += value.Value;
		}
	}

	public void ForceRecalculate()
	{
		recalculate = true;
		Recalculate();
	}
}
