using System.Collections.Generic;
using Acheivements;
using UnityEngine;

public class EndScreenAchievementSmall : MonoBehaviour
{
	private int level;

	private string displayName;

	private EndScreen callbackReciever;

	public UISprite Background;

	public UISprite Weapon;

	public UISprite Stripe;

	public UISprite Badge;

	public UILabel Label;

	public List<TweenAlpha> Alphas;

	public string Name;

	public Perk Perk = Perk.None;

	public bool IsUnlock;

	public EndScreen.AchievementType Type;

	public void SetStartValues(EndScreen.AchievementType type, Perk perk, string perkName, string name, AcheivementStatus status, int level)
	{
		this.level = level;
		Perk = perk;
		Type = type;
		Name = name;
		SetType(type, name, perkName, status);
	}

	private void SetTitleString(string text)
	{
		if (Label != null)
		{
			Label.text = text;
		}
	}

	private void SetWeaponImage(string weaponName)
	{
		if (Weapon != null)
		{
			Weapon.spriteName = weaponName;
			Weapon.MakePixelPerfect();
		}
	}

	private void SetStripeImage(string stripeName)
	{
		if (Stripe != null)
		{
			Stripe.spriteName = stripeName + "_stripe";
			Stripe.MakePixelPerfect();
		}
	}

	private void SetBadgeImage(string ImageName)
	{
		if (Badge != null)
		{
			Badge.spriteName = ImageName;
			Badge.MakePixelPerfect();
		}
	}

	private void SetType(EndScreen.AchievementType type, string name, string perkName, AcheivementStatus status)
	{
		Type = type;
		string text = string.Empty;
		string text2 = string.Empty;
		switch (status)
		{
		case AcheivementStatus.Bronze:
			text = "Bronze";
			text2 = Localizer.GetString("gui_end_screen_achievement_rank_bronze");
			break;
		case AcheivementStatus.Silver:
			text = "Silver";
			text2 = Localizer.GetString("gui_end_screen_achievement_rank_silver");
			break;
		case AcheivementStatus.Gold:
			text = "Gold";
			text2 = Localizer.GetString("gui_end_screen_achievement_rank_gold");
			break;
		case AcheivementStatus.Uncompleted:
			text = string.Empty;
			break;
		}
		switch (Type)
		{
		case EndScreen.AchievementType.PerkLevelUp:
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			SetTitleString(Localizer.GetString(perkName) + '\n' + Localizer.GetString("gui_end_screen_achievement_level") + " " + level);
			SetBadgeImage(perkName + "_medal");
			break;
		case EndScreen.AchievementType.Unlock:
			SetTitleString(Localizer.GetString(name) + '\n' + Localizer.GetString("gui_end_screen_achievement_unlocked"));
			SetWeaponImage(name);
			SetStripeImage(perkName);
			IsUnlock = true;
			break;
		case EndScreen.AchievementType.WeaponLevelUp:
			SetTitleString(Localizer.GetString(name) + '\n' + Localizer.GetString("gui_end_screen_achievement_level") + " " + level);
			SetWeaponImage(name);
			SetStripeImage(perkName);
			break;
		case EndScreen.AchievementType.ZedSlayer:
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			SetTitleString(Localizer.GetString("gui_end_screen_achievement_zed_slayer") + '\n' + text2);
			SetBadgeImage("SAZedSlayer" + text);
			break;
		case EndScreen.AchievementType.MasterDoc:
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			SetTitleString(Localizer.GetString("gui_end_screen_achievement_master_doc") + '\n' + text2);
			SetBadgeImage("SAMasterDoc" + text);
			break;
		case EndScreen.AchievementType.SpeedRunner:
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			SetTitleString(Localizer.GetString("gui_end_screen_achievement_speed_runner") + '\n' + text2);
			SetBadgeImage("SASpeedRunner" + text);
			break;
		}
	}

	public void SetOnCompleteCallback(EndScreen callbackReciever)
	{
		this.callbackReciever = callbackReciever;
	}

	private void CallbackCall()
	{
		callbackReciever.DestroyActiveLargeAchievement();
	}

	public void OnMedelTweenComplete()
	{
	}
}
