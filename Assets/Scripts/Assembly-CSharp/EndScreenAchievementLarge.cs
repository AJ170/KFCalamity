using System.Collections.Generic;
using Acheivements;
using UnityEngine;

public class EndScreenAchievementLarge : MonoBehaviour
{
	public class ProgressLevel
	{
		public int Level;

		public float XpFractionFrom;

		public float XpFractionTo;

		public float XpFrom;

		public float XpTo;
	}

	private int level;

	private int displayLevel;

	private string displayName;

	private Queue<ProgressLevel> levelProgress;

	private bool displayProgress;

	private bool spawnCompleted;

	private bool calculatingLevels;

	private float progressEndFraction = 1f;

	private float progressStartFraction;

	private float currentFraction;

	private float progressDuration = 0.5f;

	private float progressTimer;

	private TimePrint timePrinter;

	private ProgressLevel currentLevel;

	private EndScreen callbackReciever;

	public UISlider ProgressBar;

	public UISprite Background;

	public UISprite Weapon;

	public UISprite Stripe;

	public UISprite Badge;

	public UISprite GlowRing;

	public UISprite GlowCenter;

	public UISprite ProgressBarFrame;

	public UILabel Label;

	public UILabel CountLabel;

	public List<TweenAlpha> Alphas;

	public List<TweenAlpha> ProgressBarAlphas;

	public TweenAlpha LevelUpFlashAlpha;

	public string Name;

	public Perk Perk = Perk.None;

	public bool IsUnlock;

	public EndScreen.AchievementType Type;

	private void Update()
	{
		if (spawnCompleted && displayProgress && calculatingLevels)
		{
			OnProgressLevelTick();
		}
	}

	public void SetStartValues(EndScreen.AchievementType type, Queue<ProgressLevel> levels, Perk perk, string perkName, string name, AcheivementStatus status)
	{
		AddLevelProgress(levels);
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
		string text = string.Empty;
		Type = type;
		string text2 = string.Empty;
		string text3 = string.Empty;
		switch (status)
		{
		case AcheivementStatus.Bronze:
			text2 = "Bronze";
			text3 = Localizer.GetString("gui_end_screen_achievement_rank_bronze");
			break;
		case AcheivementStatus.Silver:
			text2 = "Silver";
			text3 = Localizer.GetString("gui_end_screen_achievement_rank_silver");
			break;
		case AcheivementStatus.Gold:
			text2 = "Gold";
			text3 = Localizer.GetString("gui_end_screen_achievement_rank_gold");
			break;
		case AcheivementStatus.Uncompleted:
			text2 = string.Empty;
			break;
		}
		switch (Type)
		{
		case EndScreen.AchievementType.PerkLevelUp:
		{
			ProgressLevel progressLevel = levelProgress.Peek();
			if (progressLevel != null)
			{
				text = progressLevel.Level.ToString();
			}
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			displayName = Localizer.GetString(perkName) + '\n' + Localizer.GetString("gui_end_screen_achievement_level") + " ";
			SetTitleString(displayName + text);
			SetBadgeImage(perkName + "_medal");
			break;
		}
		case EndScreen.AchievementType.Unlock:
			displayName = Localizer.GetString(name) + '\n' + Localizer.GetString("gui_end_screen_achievement_unlocked");
			SetTitleString(displayName);
			SetWeaponImage(name);
			SetStripeImage(perkName);
			break;
		case EndScreen.AchievementType.WeaponLevelUp:
		{
			ProgressLevel progressLevel = levelProgress.Peek();
			if (progressLevel != null)
			{
				text = progressLevel.Level.ToString();
			}
			displayName = Localizer.GetString(name) + '\n' + Localizer.GetString("gui_end_screen_achievement_level") + " ";
			SetTitleString(displayName + text);
			SetWeaponImage(name);
			SetStripeImage(perkName);
			break;
		}
		case EndScreen.AchievementType.ZedSlayer:
			Label.transform.localPosition = new Vector3(0f, -140f, 0f);
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			displayName = Localizer.GetString("gui_end_screen_achievement_zed_slayer");
			SetTitleString(displayName + '\n' + text3);
			SetBadgeImage("SAZedSlayer" + text2);
			break;
		case EndScreen.AchievementType.MasterDoc:
			Label.transform.localPosition = new Vector3(0f, -140f, 0f);
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			displayName = Localizer.GetString("gui_end_screen_achievement_master_doc");
			SetTitleString(displayName + '\n' + text3);
			SetBadgeImage("SAMasterDoc" + text2);
			break;
		case EndScreen.AchievementType.SpeedRunner:
			Label.transform.localPosition = new Vector3(0f, -140f, 0f);
			Weapon.gameObject.SetActive(false);
			Background.gameObject.SetActive(false);
			Stripe.gameObject.SetActive(false);
			Badge.gameObject.SetActive(true);
			displayName = Localizer.GetString("gui_end_screen_achievement_speed_runner");
			SetTitleString(displayName + '\n' + text3);
			SetBadgeImage("SASpeedRunner" + text2);
			break;
		}
	}

	private void OnProgressLevelTick()
	{
		if (timePrinter != null)
		{
			timePrinter.Update();
			CountLabel.text = (currentLevel.XpFrom + (float)timePrinter.GetCurrentCount()).ToString() + " / " + currentLevel.XpTo + " XP";
		}
		progressTimer += Time.deltaTime;
		currentFraction = progressTimer / progressDuration * progressEndFraction;
		if (progressTimer >= progressDuration)
		{
			if (GetNextLevelProgression())
			{
				LevelUpFlashAlpha.enabled = true;
				LevelUpFlashAlpha.Reset();
				LevelUpFlashAlpha.Play(true);
				Label.text = displayName + currentLevel.Level;
				ProgressBar.sliderValue = 0f;
				currentFraction = 0f;
				progressTimer = 0f;
				timePrinter = new TimePrint();
				timePrinter.AddMessage((int)(currentLevel.XpTo - currentLevel.XpFrom), progressDuration, false);
				if (callbackReciever != null && callbackReciever.Manager != null)
				{
					callbackReciever.Manager.PlaySound(GuiInGameManager.Sound.ProgressBar);
				}
			}
			else
			{
				ProgressBar.sliderValue = progressEndFraction;
				calculatingLevels = false;
				Invoke("CallbackCall", 0.5f);
			}
		}
		else
		{
			ProgressBar.sliderValue = currentFraction;
		}
	}

	private bool GetNextLevelProgression()
	{
		if (levelProgress != null && levelProgress.Count > 0)
		{
			if (Type == EndScreen.AchievementType.PerkLevelUp || Type == EndScreen.AchievementType.WeaponLevelUp)
			{
				ProgressBar.gameObject.SetActive(true);
				ProgressBarFrame.gameObject.SetActive(true);
				currentLevel = levelProgress.Dequeue();
				progressStartFraction = currentLevel.XpFractionFrom;
				progressEndFraction = currentLevel.XpFractionTo;
				ProgressBar.sliderValue = progressStartFraction;
				calculatingLevels = true;
				timePrinter = new TimePrint();
				timePrinter.AddMessage((int)(currentLevel.XpTo - currentLevel.XpFrom), progressDuration, false);
				return true;
			}
			return false;
		}
		return false;
	}

	public void AddLevelProgress(Queue<ProgressLevel> levels)
	{
		levelProgress = levels;
		displayProgress = true;
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
		spawnCompleted = true;
		if (displayProgress)
		{
			if (!GetNextLevelProgression())
			{
				Invoke("CallbackCall", 0.5f);
			}
		}
		else
		{
			Invoke("CallbackCall", 0.5f);
		}
	}
}
