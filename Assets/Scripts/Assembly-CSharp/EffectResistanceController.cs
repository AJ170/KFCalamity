using System.Collections.Generic;
using EffectHelpers;
using UnityEngine;

public class EffectResistanceController : MonoBehaviour
{
	public List<ResistanceEditorData> DefaultResistances = new List<ResistanceEditorData>();

	private List<EffectResistance> resistances = new List<EffectResistance>();

	private void Awake()
	{
		SetupDefaultResistances();
	}

	private void Update()
	{
		RecalculateResistances();
	}

	private void SetupDefaultResistances()
	{
		foreach (ResistanceEditorData defaultResistance in DefaultResistances)
		{
			if (!IsResistanceRegistered(defaultResistance.Name))
			{
				RegisterResistance(new EffectResistance(defaultResistance.Name, defaultResistance.Immune, defaultResistance.Multiplier));
			}
			else
			{
				Debug.LogError(string.Format("EffectResistanceController:SetupDefaultResistances() -> {0} already registered. Maybe duplicated?", defaultResistance.Name.ToString()));
			}
		}
	}

	public static EffectResistance CreateResistance(BuffClass name, GameObject go)
	{
		EffectResistanceController effectResistanceController = ControllerByGameObject(go);
		EffectResistance effectResistance = null;
		bool flag = true;
		if (effectResistanceController != null && effectResistanceController.IsResistanceRegistered(name))
		{
			effectResistance = effectResistanceController.ResistanceByName(name);
			flag = false;
		}
		if (flag)
		{
			effectResistance = new EffectResistance(name);
			if (effectResistanceController != null)
			{
				effectResistanceController.RegisterResistance(effectResistance);
			}
		}
		return (!(go == null)) ? effectResistance : null;
	}

	public void RegisterResistance(EffectResistance resistance)
	{
		if (resistance != null && !IsResistanceRegistered(resistance.Name))
		{
			if (resistance.Name != 0)
			{
				resistances.Add(resistance);
			}
			else
			{
				Debug.LogError(string.Format("EffectResistanceController:RegisterResistance() -> Tried to register '{0}' for {1}", resistance.Name.ToString(), base.transform.root.name));
			}
		}
	}

	private void RecalculateResistances()
	{
		foreach (EffectResistance resistance in resistances)
		{
			resistance.Recalculate();
		}
	}

	public EffectResistance ResistanceByName(BuffClass name)
	{
		foreach (EffectResistance resistance in resistances)
		{
			if (name == resistance.Name)
			{
				return resistance;
			}
		}
		return null;
	}

	public List<EffectResistance> Resistances()
	{
		return resistances;
	}

	private bool IsResistanceRegistered(BuffClass name)
	{
		foreach (EffectResistance resistance in resistances)
		{
			if (name == resistance.Name)
			{
				return true;
			}
		}
		return false;
	}

	public static EffectResistanceController ControllerByGameObject(GameObject go)
	{
		if (go != null)
		{
			EffectResistanceController componentInRoot = go.GetComponentInRoot<EffectResistanceController>();
			if (componentInRoot != null)
			{
				return componentInRoot;
			}
		}
		else
		{
			Debug.LogError("EffectResistanceController:ControllerByGameObject() Missing gameObject!");
		}
		return null;
	}
}
