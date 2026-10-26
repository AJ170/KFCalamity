using System.Collections.Generic;
using EffectHelpers;
using UnityEngine;

public class EffectResistance
{
	private BuffClass name;

	private float multiplierValue = 1f;

	private float defaultMultiplierValue = 1f;

	private bool immune;

	private bool defaultImmune;

	private Dictionary<object, float> multipliers = new Dictionary<object, float>();

	private List<object> temporaryImmunity = new List<object>();

	private bool recalculate;

	public BuffClass Name
	{
		get
		{
			return name;
		}
	}

	public float Multiplier
	{
		get
		{
			return multiplierValue;
		}
	}

	public bool IsImmune
	{
		get
		{
			return immune;
		}
	}

	public EffectResistance(BuffClass resistanceName)
	{
		name = resistanceName;
	}

	public EffectResistance(BuffClass resistanceName, bool immunity, float multiplier)
	{
		name = resistanceName;
		defaultImmune = immunity;
		immune = defaultImmune;
		defaultMultiplierValue = multiplier;
		multiplierValue = defaultMultiplierValue;
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

	public void SetTemporaryImmunity(object alterer)
	{
		if (!defaultImmune)
		{
			temporaryImmunity.Add(alterer);
			recalculate = true;
		}
	}

	public void Restore(object alterer)
	{
		multipliers.Remove(alterer);
		temporaryImmunity.Remove(alterer);
		recalculate = true;
	}

	public void Recalculate()
	{
		if (recalculate)
		{
			RecalculateMultipliers();
			RecalculateImmunity();
			recalculate = false;
		}
	}

	private void RecalculateMultipliers()
	{
		multiplierValue = 1f;
		foreach (KeyValuePair<object, float> multiplier in multipliers)
		{
			multiplierValue *= multiplier.Value;
		}
	}

	private void RecalculateImmunity()
	{
		immune = (defaultImmune ? defaultImmune : (temporaryImmunity.Count > 0));
	}
}
