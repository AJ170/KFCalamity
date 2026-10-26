using System.Collections.Generic;
using EffectEvents;
using UnityEngine;

public class PlayerAbilityController : AbilityController
{
	public GameObject Grenade;

	public GameObject Syringe;

	public GameObject TargetSyringe;

	public GameObject Perk;

	public GameObject Dodge;

	public bool DisableAll;

	private Dictionary<AbilitySlot, Ability> slotAbilities = new Dictionary<AbilitySlot, Ability>();

	private PlayerActionController actionController;

	private PlayerGUILink GUILink;

	private AttributeModifier dodgeCount;

	private AttributeModifier perkCount;

	protected override void Start()
	{
		base.Start();
		dodgeCount = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DodgeCount, base.gameObject);
		perkCount = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.AbilityCount, base.gameObject);
		actionController = GetComponent<PlayerActionController>();
		GUILink = GetComponent<PlayerGUILink>();
		SetupSlotAbility(AbilitySlot.Grenade);
		SetupSlotAbility(AbilitySlot.SyringeSelf);
		SetupSlotAbility(AbilitySlot.SyringeTarget);
		SetupSlotAbility(AbilitySlot.Perk);
		SetupSlotAbility(AbilitySlot.Dodge);
		if (!DisableAll)
		{
			return;
		}
		foreach (Ability value in slotAbilities.Values)
		{
			value.enabled = false;
		}
	}

	private void SetupSlotAbility(AbilitySlot slot)
	{
		Ability ability = SetupAbility(PrefabBySlot(slot));
		if (ability != null)
		{
			AddSlotAbility(slot, ability);
		}
	}

	private void AddSlotAbility(AbilitySlot slot, Ability ability)
	{
		if (ability != null)
		{
			slotAbilities.Add(slot, ability);
			Soldier component = base.gameObject.GetComponent<Soldier>();
			if (component != null)
			{
				component.SetStartingCharges(ability, slot);
			}
			if (GUILink != null && InformGUI(slot))
			{
				GUILink.AddAbility(ability, slot);
			}
		}
	}

	public override bool CanCast(int abilityIndex)
	{
		return CanUse(base.CanCast(abilityIndex));
	}

	public bool CanCast(AbilitySlot slot)
	{
		return CanCast(slot, false);
	}

	public bool CanCast(AbilitySlot slot, bool ignoreCriterion)
	{
		return CanUse(base.CanCast(AbilityBySlot(slot), ignoreCriterion));
	}

	private bool CanUse(bool canCast)
	{
		if (canCast && actionController != null)
		{
			canCast = actionController.CanUseAbility();
		}
		return canCast;
	}

	public override void Cast(int abilityIndex)
	{
		Cast(AbilityByIndex(abilityIndex));
	}

	public void Cast(AbilitySlot slot)
	{
		switch (slot)
		{
		case AbilitySlot.Dodge:
			dodgeCount.Alter(this, 1);
			break;
		case AbilitySlot.Perk:
			perkCount.Alter(this, 1);
			break;
		}
		Cast(AbilityBySlot(slot));
	}

	private void Cast(Ability ability)
	{
		if (ability != null)
		{
			if (actionController != null && ShouldUseAction(ability))
			{
				actionController.OnUseAbility(ability, SlotByAbility(ability));
				UseAbilityAction action = new UseAbilityAction(ability);
				actionController.AddAction(action);
			}
			else
			{
				ability.Cast();
				OnCastCompleted(ability);
			}
		}
	}

	public bool HasChargesLeft(AbilitySlot slot)
	{
		Ability ability = AbilityBySlot(slot);
		if (ability != null)
		{
			return ability.UseCharges && ability.ChargesLeft() > 0;
		}
		return false;
	}

	public void ResetPerkCooldown()
	{
		Ability ability = AbilityBySlot(AbilitySlot.Perk);
		if (ability != null)
		{
			ability.ResetCooldown();
		}
	}

	public override void OnCastCompleted(Ability ability)
	{
		AbilitySlot abilitySlot = SlotByAbility(ability);
		if (abilitySlot == AbilitySlot.SyringeTarget)
		{
			Ability ability2 = AbilityBySlot(AbilitySlot.SyringeSelf);
			if (ability2 != null)
			{
				ability2.TriggerCooldown();
				ability2.ConsumeCharge();
			}
		}
		if (abilitySlot == AbilitySlot.SyringeSelf || abilitySlot == AbilitySlot.SyringeTarget)
		{
			LifetimeStatsHandler.AddSyringeUsed();
		}
	}

	public override void OnChargeChange(Ability ability)
	{
		AbilitySlot slot = SlotByAbility(ability);
		if (InformGUI(slot) && GUILink != null)
		{
			GUILink.SetAbilityCharges(ability, slot);
		}
	}

	public override void OnTriggerCooldown(Ability ability)
	{
		AbilitySlot slot = SlotByAbility(ability);
		if (InformGUI(slot) && GUILink != null)
		{
			GUILink.SetAbilityCooldown(ability, slot);
		}
	}

	public override void OnCooldownOver(Ability ability)
	{
		AbilitySlot abilitySlot = SlotByAbility(ability);
		if (InformGUI(abilitySlot) && GUILink != null)
		{
			GUILink.SetAbilityCooldownOver(abilitySlot);
		}
	}

	public void OnInputChargeUpStart(AbilitySlot slot, float chargeUpTime)
	{
		if (slot == AbilitySlot.SyringeTarget)
		{
			Ability ability = AbilityBySlot(slot);
			if (ability.enabled)
			{
				ability.EventSystem.Trigger(new ChargeUpStart(chargeUpTime));
			}
		}
	}

	public void OnInputChargeUpRelease(AbilitySlot slot)
	{
		if (slot == AbilitySlot.SyringeTarget)
		{
			Ability ability = AbilityBySlot(slot);
			ability.EventSystem.Trigger(new ChargeUpRelease());
		}
	}

	public void OnInputChargeUpMax(AbilitySlot slot, bool castSuccess)
	{
		if (slot == AbilitySlot.SyringeTarget)
		{
			Ability ability = AbilityBySlot(slot);
			if (ability.enabled)
			{
				ability.EventSystem.Trigger(new ChargeUpMax(castSuccess));
			}
		}
	}

	public Ability AbilityBySlot(AbilitySlot slot)
	{
		Ability result = null;
		if (slot != 0 && slotAbilities.ContainsKey(slot))
		{
			result = slotAbilities[slot];
		}
		return result;
	}

	public AbilitySlot SlotByAbility(Ability ability)
	{
		if (ability != null)
		{
			foreach (KeyValuePair<AbilitySlot, Ability> slotAbility in slotAbilities)
			{
				if (ability == slotAbility.Value)
				{
					return slotAbility.Key;
				}
			}
		}
		return AbilitySlot.None;
	}

	private GameObject PrefabBySlot(AbilitySlot slot)
	{
		GameObject result = null;
		switch (slot)
		{
		case AbilitySlot.Grenade:
			result = Grenade;
			break;
		case AbilitySlot.SyringeSelf:
			result = Syringe;
			break;
		case AbilitySlot.SyringeTarget:
			result = TargetSyringe;
			break;
		case AbilitySlot.Perk:
			result = Perk;
			break;
		case AbilitySlot.Dodge:
			result = Dodge;
			break;
		}
		return result;
	}

	private bool ShouldUseAction(Ability ability)
	{
		bool result = false;
		if (ability.ChargeUp > 0f || ability.WindDown > 0f)
		{
			result = true;
		}
		return result;
	}

	private bool InformGUI(AbilitySlot slot)
	{
		return slot != 0 && slot != AbilitySlot.SyringeTarget;
	}
}
