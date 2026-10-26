using System.Collections.Generic;
using EffectEvents;
using UnityEngine;

public class Ability : EffectSystem
{
	public enum MoveAndUseMode
	{
		NoRestriction = 0,
		RotateFreely = 1,
		FaceTarget = 2,
		Freeze = 3,
		FaceThenFreeze = 4
	}

	public string Name;

	public string Description;

	public AbilityType Type;

	public string Image;

	public float ChargeUp;

	public float WindDown;

	public float Cooldown;

	public MoveAndUseMode MoveAndUse;

	public bool CancelReload = true;

	public bool UseCharges;

	public int MaxCharges = 1;

	public int StartCharges = 1;

	protected AbilityController abilityController;

	protected float activeCooldown;

	protected bool activeToggle;

	protected int chargesLeft;

	private List<IAbilityCriterion> criterions = new List<IAbilityCriterion>();

	public bool IsToggleActive
	{
		get
		{
			return activeToggle;
		}
	}

	protected override void Awake()
	{
		systemType = EffectSystemType.Ability;
		SetupCharges();
	}

	protected override void Start()
	{
		base.Start();
		abilityController = AbilityController();
	}

	private void Update()
	{
		UpdateCooldown();
	}

	private void SetupCharges()
	{
		if (UseCharges)
		{
			chargesLeft = StartCharges;
		}
	}

	public void SetMaxCharges(int maxCharges)
	{
		MaxCharges = maxCharges;
		if (chargesLeft > MaxCharges)
		{
			SetCharges(MaxCharges);
		}
	}

	public void SetCharges(int charges)
	{
		if (UseCharges)
		{
			chargesLeft = charges;
			if (abilityController != null)
			{
				abilityController.OnChargeChange(this);
			}
		}
	}

	public void ConsumeCharge()
	{
		if (UseCharges && chargesLeft > 0)
		{
			chargesLeft--;
			if (abilityController != null)
			{
				abilityController.OnChargeChange(this);
			}
		}
	}

	public void RefillCharges(int charges)
	{
		if (UseCharges)
		{
			chargesLeft = Mathf.Min(chargesLeft + charges, MaxCharges);
			if (abilityController != null)
			{
				abilityController.OnChargeChange(this);
			}
		}
	}

	public int ChargesLeft()
	{
		return UseCharges ? chargesLeft : 0;
	}

	public bool AnyChargesLeft()
	{
		return (UseCharges && chargesLeft > 0) || !UseCharges;
	}

	public bool AllChargesLeft()
	{
		return UseCharges && chargesLeft == MaxCharges;
	}

	public void RegisterCriterion(IAbilityCriterion criterion)
	{
		criterions.Add(criterion);
	}

	private bool CheckCriteria()
	{
		if (criterions != null)
		{
			foreach (IAbilityCriterion criterion in criterions)
			{
				if (!criterion.Evaluate())
				{
					return false;
				}
			}
		}
		return true;
	}

	public virtual bool CanCast()
	{
		return CanCast(false);
	}

	public virtual bool CanCast(bool ignoreCriterion)
	{
		if (base.enabled && IsCooldownReady() && AnyChargesLeft() && (ignoreCriterion || CheckCriteria()))
		{
			return true;
		}
		return false;
	}

	public void OnChargeUp()
	{
		TriggerInternal(new ChargeUp());
	}

	public void Cast()
	{
		if (Type == AbilityType.Activated)
		{
			ConsumeCharge();
			TriggerInternal(new Cast(Name));
			TriggerCooldown();
		}
		else if (Type == AbilityType.Toggled)
		{
			Toggle();
		}
	}

	private void Toggle()
	{
		if (Type == AbilityType.Toggled)
		{
			if (activeToggle)
			{
				DeleteToggleEffects();
				TriggerInternal(new ToggledOff(Name));
				TriggerCooldown();
				activeToggle = false;
			}
			else
			{
				TriggerInternal(new Cast(Name));
				TriggerInternal(new ToggledOn(Name));
				activeToggle = true;
			}
		}
	}

	private void DeleteToggleEffects()
	{
		EffectController componentInRoot = base.gameObject.GetComponentInRoot<EffectController>();
		if (componentInRoot != null)
		{
			componentInRoot.DeleteEffectsByCreator(this);
		}
	}

	private void UpdateCooldown()
	{
		if (activeCooldown > 0f)
		{
			activeCooldown = Mathf.Max(activeCooldown - Time.deltaTime, 0f);
			if (IsCooldownReady() && abilityController != null)
			{
				abilityController.OnCooldownOver(this);
			}
		}
	}

	public virtual void TriggerCooldown()
	{
		if (Cooldown > 0f)
		{
			activeCooldown = Cooldown;
			if (abilityController != null)
			{
				abilityController.OnTriggerCooldown(this);
			}
		}
	}

	public virtual void ResetCooldown()
	{
		if (activeCooldown > 0f)
		{
			activeCooldown = 0f;
			if (abilityController != null)
			{
				abilityController.OnCooldownOver(this);
			}
		}
	}

	public bool IsCooldownReady()
	{
		return activeCooldown == 0f;
	}

	public AbilityController AbilityController()
	{
		if (abilityController == null)
		{
			GameObject gameObject = base.transform.root.gameObject;
			abilityController = gameObject.GetComponent<AbilityController>();
		}
		return abilityController;
	}
}
