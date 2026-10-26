using System.Collections.Generic;
using EffectHelpers;
using UnityEngine;

public class EffectController : EffectSystem
{
	private List<EffectBaseTarget> activeEffects = new List<EffectBaseTarget>();

	private EffectResistanceController resistanceController;

	protected override void Awake()
	{
		systemType = EffectSystemType.General;
	}

	protected override void Start()
	{
		base.Start();
		resistanceController = EffectResistanceController.ControllerByGameObject(base.gameObject);
	}

	private void Update()
	{
		DeleteEffectsByImmunity();
		DestroyDeletedEffects();
		UpdateActiveEffects();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		DestroyAllEffects();
		activeEffects.Clear();
		resistanceController = null;
	}

	public EffectBaseTarget CreateEffect(string effectName, GameObject source, EffectSystem creator)
	{
		EffectBaseTarget effectBaseTarget = TargetEffectFactory.CreateEffect(effectName);
		if (effectBaseTarget != null)
		{
			effectBaseTarget.Owner = this;
			effectBaseTarget.GameObject = base.gameObject;
			effectBaseTarget.Creator = creator;
			effectBaseTarget.Source = FindCorrectSource(source);
			InitEffect(effectBaseTarget);
			bool addEffect = false;
			bool immune = false;
			if (effectBaseTarget.EffectType != 0)
			{
				if (effectBaseTarget.Evaluator())
				{
					ProcessResistance(effectBaseTarget, out immune);
					if (!immune)
					{
						ProcessEffect(effectBaseTarget, out addEffect);
					}
				}
			}
			else
			{
				Debug.LogWarning("EffectController:CreateEffect() Ignore effect " + effectName + ", has EffectType.None");
			}
			if (addEffect)
			{
				activeEffects.Add(effectBaseTarget);
			}
			return (!addEffect) ? null : effectBaseTarget;
		}
		Debug.LogError("EffectController:CreateEffect() Failed to create effect " + effectName);
		return null;
	}

	private void InitEffect(EffectBaseTarget effect)
	{
		if (effect != null)
		{
			if (effect != null)
			{
				((IFactoryItem)effect).Init();
			}
			else
			{
				Debug.LogError("EffectController:InitEffect() Failed to init effect " + effect.GetType().Name);
			}
		}
	}

	private void DestroyEffect(EffectBaseTarget effect)
	{
		if (effect != null)
		{
			if (effect.ShouldShutdown())
			{
				effect.Shutdown();
			}
			activeEffects.Remove(effect);
		}
	}

	public void DeleteEffectsByCreator(EffectSystem creator)
	{
		if (!(creator != null))
		{
			return;
		}
		foreach (EffectBaseTarget activeEffect in activeEffects)
		{
			if (creator == activeEffect.Creator)
			{
				activeEffect.ForceDelete();
			}
		}
	}

	private void DestroyDeletedEffects()
	{
		EffectBaseTarget effectBaseTarget = null;
		int count = activeEffects.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			effectBaseTarget = activeEffects[num];
			if (effectBaseTarget.IsDeleted)
			{
				DestroyEffect(effectBaseTarget);
			}
		}
	}

	private void DestroyAllEffects()
	{
		EffectBaseTarget effectBaseTarget = null;
		int count = activeEffects.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			effectBaseTarget = activeEffects[num];
			DestroyEffect(effectBaseTarget);
		}
	}

	private void UpdateActiveEffects()
	{
		foreach (EffectBaseTarget activeEffect in activeEffects)
		{
			activeEffect.Update();
		}
	}

	private void ProcessEffect(EffectBaseTarget effect, out bool addEffect)
	{
		addEffect = true;
		EffectType effectType = effect.EffectType;
		if (effectType != EffectType.Continuous && effectType != EffectType.Infinite)
		{
			return;
		}
		if (effect.IsOverridable)
		{
			bool consumed;
			bool refreshed;
			RefreshEffect(effect, out consumed, out refreshed);
			if (consumed)
			{
				if (refreshed)
				{
				}
				addEffect = false;
			}
		}
		if (!effect.IsStackable)
		{
			return;
		}
		bool consumed2;
		bool stacked;
		StackEffect(effect, out consumed2, out stacked);
		if (consumed2)
		{
			if (stacked)
			{
			}
			addEffect = false;
		}
	}

	private void ProcessResistance(EffectBaseTarget effect, out bool immune)
	{
		immune = false;
		if (resistanceController != null)
		{
			EffectResistance resistance = resistanceController.ResistanceByName(effect.BuffClass);
			effect.Resistance(resistance, out immune);
		}
	}

	private void DeleteEffectsByImmunity()
	{
		if (!(resistanceController != null))
		{
			return;
		}
		List<EffectResistance> list = resistanceController.Resistances();
		foreach (EffectResistance item in list)
		{
			if (item.IsImmune)
			{
				DeleteEffectsByBuffClass(item.Name);
			}
		}
	}

	private void DeleteEffectsByBuffClass(BuffClass immuneTo)
	{
		if (immuneTo == BuffClass.None)
		{
			return;
		}
		foreach (EffectBaseTarget activeEffect in activeEffects)
		{
			if (immuneTo == activeEffect.BuffClass)
			{
				activeEffect.ForceDelete();
			}
		}
	}

	private void RefreshEffect(EffectBaseTarget effect, out bool consumed, out bool refreshed)
	{
		consumed = false;
		refreshed = false;
		foreach (EffectBaseTarget activeEffect in activeEffects)
		{
			if (effect.GetType() == activeEffect.GetType())
			{
				consumed = true;
				refreshed = activeEffect.Refresh(effect);
				break;
			}
		}
	}

	private void StackEffect(EffectBaseTarget effect, out bool consumed, out bool stacked)
	{
		consumed = false;
		stacked = false;
		foreach (EffectBaseTarget activeEffect in activeEffects)
		{
			if (effect.GetType() == activeEffect.GetType())
			{
				consumed = true;
				stacked = activeEffect.Stack();
				break;
			}
		}
	}

	private GameObject FindCorrectSource(GameObject source)
	{
		return StandaloneEntity.Source(source);
	}

	public bool HasEffect(string effectName)
	{
		return base.gameObject.GetComponent(effectName) != null;
	}
}
