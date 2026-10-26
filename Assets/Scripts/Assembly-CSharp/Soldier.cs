using EffectHelpers;
using EventHelpers;
using GameEvents;
using UnityEngine;

public class Soldier : MonoBehaviour
{
	public Perk Perk = Perk.None;

	public int MoneyToDrop = 100;

	public int MoneyToGiveAwayInShop = 100;

	private EventSystem eventSystem;

	private PerkBonuses perkBonuses;

	private int playerID;

	private int playerInputID;

	private PlayerStats stats;

	private static float armorCost = 1.5f;

	public int PlayerID
	{
		get
		{
			return playerID;
		}
	}

	public int PlayerInputID
	{
		get
		{
			return playerInputID;
		}
	}

	public PlayerStats Stats
	{
		get
		{
			return stats;
		}
	}

	private void Awake()
	{
		eventSystem = EventSystem.Instance;
		perkBonuses = PerkBonuses.Instance;
		if (eventSystem != null)
		{
			eventSystem.Subscribe(EventId.PerkLevelUp, OnPerkLevelUp, this);
		}
		ObscuredUnitsManager.RegisterUnit(base.gameObject);
	}

	public void Setup(int playerID, int playerInputID)
	{
		this.playerID = playerID;
		this.playerInputID = playerInputID;
		PlayerStatsManager instance = PlayerStatsManager.Instance;
		if (instance != null)
		{
			stats = instance.StatsByPlayerId(playerID);
		}
	}

	private void Start()
	{
		PerkAttributes();
		if (Perk != Perk.None)
		{
			SetStartingArmor();
		}
		ZedTimeManager.RegisterSoldierDeathReceiver(base.gameObject);
	}

	private void OnDestroy()
	{
		if (eventSystem != null)
		{
			eventSystem.Unsubscribe(this);
		}
		ObscuredUnitsManager.UnregisterUnit(base.gameObject);
	}

	private void OnPerkLevelUp(EventBase e)
	{
		if (Perk != Perk.None)
		{
			PerkLevelUp perkLevelUp = e as PerkLevelUp;
			if (perkLevelUp.Perk == Perk)
			{
				PerkAttributes();
			}
		}
	}

	private void OnZedTimeStart(EventBase e)
	{
		ParticleManager instance = ParticleManager.Instance;
		if (instance != null)
		{
			GameObject gameObject = instance.CreateParticle("FxZedTime", Vector3.zero, Quaternion.identity, ParticleDestroyMethod.Duration);
			if (gameObject != null)
			{
				Transform transform = gameObject.transform;
				transform.parent = base.transform;
				transform.localPosition = new Vector3(0f, 0.1f, 0f);
			}
		}
	}

	private void PerkAttributes()
	{
		if (Perk != Perk.None && perkBonuses != null)
		{
			UpdateHealingDealt();
			UpdateConstraintReduction();
			UpdateStealthDetection();
			UpdateDamageReduction();
		}
	}

	public void DropMoney()
	{
		PickupManager instance = PickupManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		MoneyPickup moneyPickup = instance.CreateMoneyPickup(base.transform.position);
		if (moneyPickup != null)
		{
			int money = (moneyPickup.Wealth = Mathf.Min(stats.Money, MoneyToDrop));
			moneyPickup.Rotate(base.transform.rotation);
			stats.RemoveMoney(money);
			PlayerGUILink component = GetComponent<PlayerGUILink>();
			if (component != null)
			{
				component.OnMoneyChange();
			}
			SpeechManager.PlaySpeech(SpeechManager.SpeechLine.MoneyDropped, base.gameObject);
		}
	}

	public bool CanDropMoney()
	{
		if (stats != null)
		{
			return stats.Money > 0;
		}
		return false;
	}

	private void UpdateHealingDealt()
	{
		float num = perkBonuses.SyringeHealMultiplier(Perk);
		if (num != 1f)
		{
			AttributeModifier attributeModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.HealingDealt, base.gameObject);
			if (attributeModifier != null)
			{
				attributeModifier.Restore(this);
				attributeModifier.Alter(this, num);
			}
		}
	}

	private void UpdateConstraintReduction()
	{
		float num = perkBonuses.ConstraintReductionMultiplier(Perk);
		if (num != 1f)
		{
			EffectResistance effectResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, base.gameObject);
			if (effectResistance != null)
			{
				effectResistance.Restore(this);
				effectResistance.Alter(this, num);
			}
		}
	}

	private void UpdateStealthDetection()
	{
		StealthDetection component = base.gameObject.GetComponent<StealthDetection>();
		if (component != null)
		{
			float num = perkBonuses.StealthDetectionMultiplier(Perk);
			component.DetectionRange = component.DefaultDetectionRange * num;
		}
	}

	private void UpdateDamageReduction()
	{
		float num = perkBonuses.DamageReduction(Perk);
		if (num != 1f)
		{
			AttributeModifier attributeModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageReceived, base.gameObject);
			if (attributeModifier != null)
			{
				attributeModifier.Restore(this);
				attributeModifier.Alter(this, num);
			}
		}
	}

	public void SetStartingWeapon(PlayerWeaponController weaponController, InventoryWeaponSlot slot)
	{
		if (!(weaponController != null) || perkBonuses == null)
		{
			return;
		}
		WeaponFactory instance = WeaponFactory.Instance;
		if (!(instance != null))
		{
			return;
		}
		switch (slot)
		{
		case InventoryWeaponSlot.Melee:
		{
			string value2 = perkBonuses.StartingMeleeWeapon(Perk);
			if (!string.IsNullOrEmpty(value2))
			{
				weaponController.StartingMelee = instance.WeaponPrefabByName(value2);
			}
			break;
		}
		case InventoryWeaponSlot.Pistol:
		{
			string value = perkBonuses.StartingPistol(Perk);
			if (!string.IsNullOrEmpty(value))
			{
				weaponController.StartingPistol = instance.WeaponPrefabByName(value);
			}
			break;
		}
		}
	}

	public void SetStartingArmor()
	{
		Health component = GetComponent<Health>();
		if (component != null && perkBonuses != null)
		{
			int num = perkBonuses.ExtraArmor(Perk);
			if (num > 0)
			{
				component.RefillArmor(num);
			}
		}
	}

	public void SetStartingCharges(Ability ability, AbilitySlot slot)
	{
		if (ability != null && ability.UseCharges && perkBonuses != null)
		{
			int num = 0;
			if (slot == AbilitySlot.SyringeSelf)
			{
				num = ScalingByGameSettings.SyringeModifierByDifficulty();
			}
			if (num != 0)
			{
				ability.SetMaxCharges(ability.MaxCharges + num);
			}
			int num2 = 0;
			switch (slot)
			{
			case AbilitySlot.Grenade:
				num2 = perkBonuses.ExtraGrenades(Perk);
				break;
			case AbilitySlot.SyringeSelf:
				num2 = perkBonuses.ExtraSyringes(Perk);
				break;
			}
			if (num2 > 0)
			{
				ability.SetMaxCharges(ability.MaxCharges + num2);
				ability.SetCharges(ability.ChargesLeft() + num2);
			}
		}
	}

	public float PerkWeaponDiscountMultiplier()
	{
		if (perkBonuses != null)
		{
			return perkBonuses.PerkWeaponDiscountMultiplier(Perk);
		}
		return 1f;
	}

	public float AmmoDiscountMultiplier()
	{
		if (perkBonuses != null)
		{
			return perkBonuses.AmmoDiscountMultiplier(Perk);
		}
		return 1f;
	}

	public float GrenadeDiscountMultiplier()
	{
		if (perkBonuses != null)
		{
			return perkBonuses.GrenadeDiscountMultiplier(Perk);
		}
		return 1f;
	}

	public float ArmorDiscountMultiplier()
	{
		if (perkBonuses != null)
		{
			return perkBonuses.ArmorDiscountMultiplier(Perk);
		}
		return 1f;
	}

	public static int CalculateGrenadeCost(GameObject parentSoldier)
	{
		int num = 40;
		if (parentSoldier != null)
		{
			Soldier component = parentSoldier.GetComponent<Soldier>();
			if (component != null)
			{
				return Mathf.RoundToInt((float)num * component.GrenadeDiscountMultiplier());
			}
		}
		return num;
	}

	public static int CalculateArmorCost(GameObject parentSoldier)
	{
		if (parentSoldier != null)
		{
			Health component = parentSoldier.GetComponent<Health>();
			Soldier component2 = parentSoldier.GetComponent<Soldier>();
			if (component != null && component2 != null)
			{
				return (int)((float)(component.MaxArmor - component.CurrentArmor) * (armorCost * component2.ArmorDiscountMultiplier()));
			}
		}
		return 0;
	}

	public static int CalculateArmorDeficitByMoney(GameObject parentSoldier, int money)
	{
		if (parentSoldier != null)
		{
			Health component = parentSoldier.GetComponent<Health>();
			Soldier component2 = parentSoldier.GetComponent<Soldier>();
			if (component != null && component2 != null)
			{
				int num = (int)((float)component.MaxArmor * (armorCost * component2.ArmorDiscountMultiplier()));
				if (money < num)
				{
					return (int)((float)money / (armorCost * component2.ArmorDiscountMultiplier()));
				}
				return component.MaxArmor;
			}
		}
		return 0;
	}
}
