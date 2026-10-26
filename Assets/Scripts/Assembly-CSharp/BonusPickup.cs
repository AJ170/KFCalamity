using System.Collections.Generic;
using UnityEngine;

public class BonusPickup : Pickup
{
	public PickupBonusType BonusType;

	public bool AffectAllPlayers;

	private string BonusEffectComponent
	{
		get
		{
			string result = string.Empty;
			if (BonusType == PickupBonusType.Damage)
			{
				result = "SuperDamageBonusEffect";
			}
			else if (BonusType == PickupBonusType.Shield)
			{
				result = "SuperResistanceBonusEffect";
			}
			return result;
		}
	}

	private void Awake()
	{
		type = PickupType.Bonus;
	}

	protected override void OnRespawn()
	{
		ShowPickupObject();
	}

	protected override void OnCollidePlayer(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		if (!string.IsNullOrEmpty(BonusEffectComponent))
		{
			if (AffectAllPlayers)
			{
				SoldierManager instance = SoldierManager.Instance;
				if (instance != null)
				{
					List<SoldierItem> aliveSoldiers = instance.GetAliveSoldiers();
					foreach (SoldierItem item in aliveSoldiers)
					{
						ApplyToPlayer(item.SoldierObject);
					}
				}
				else
				{
					ApplyToPlayer(gameObject);
				}
			}
			else
			{
				ApplyToPlayer(gameObject);
			}
		}
		PlayPickupSound();
		PlayPickupParticle();
		PlayerGUILink playerGUILink = GUILinkByObject(gameObject);
		if (playerGUILink != null)
		{
			playerGUILink.OnBonusPickup(BonusType);
		}
		if (Respawn())
		{
			HidePickupObject();
		}
		else
		{
			ForceDelete();
		}
	}

	private void ApplyToPlayer(GameObject player)
	{
		EffectController effectController = EffectControllerByObject(player);
		if (effectController != null)
		{
			effectController.CreateEffect(BonusEffectComponent, player, null);
		}
	}

	private EffectController EffectControllerByObject(GameObject go)
	{
		if (go != null)
		{
			return go.GetComponent<EffectController>();
		}
		return null;
	}
}
