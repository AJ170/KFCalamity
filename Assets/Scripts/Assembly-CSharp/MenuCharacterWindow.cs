using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class MenuCharacterWindow : MonoBehaviour
{
	public TweenAlpha AlphaReady;

	public TweenPosition TopPositionTween;

	public TweenPosition BottomPositionTween;

	public UILabel LabelTitle;

	public UILabel LabelPerk;

	public UILabel LabelCharacter;

	public UILabel LabelDescription;

	public UILabel LabelInfo;

	public UILabel LabelReady;

	public UILabel Labellevel;

	public UILabel LabelXp;

	public UISprite Lock;

	public MenuCharacterTitleBar PerkBar;

	public MenuCharacterTitleBar CharacterBar;

	public UISlider LevelBar;

	private List<MenuCharacterTitleBar> TitleBars = new List<MenuCharacterTitleBar>();

	private int id;

	private int listIndex;

	private int perkIndex;

	private int characterIndex;

	private bool handleInput;

	private bool handleAxisInput = true;

	private bool isReady;

	private bool isDemo = true;

	private int playerID;

	private MenuCharacterSelect manager;

	private List<PerkData> perks;

	private List<CharacterDisplayData> characters;

	private List<CharacterDisplayData> sortedCharacters = new List<CharacterDisplayData>();

	private Player player;

	private AxisTouchButtonsWrapper[] touchButtons;

	private void Start()
	{
		TitleBars.Add(PerkBar);
		TitleBars.Add(CharacterBar);
		TitleBars[listIndex].OnFocusGained();
		LabelDescription.lineWidth = 270;
		perks = manager.Perks();
		characters = manager.Characters();
		PerkBar.SetText(Localizer.GetString(perks[perkIndex].PerkDisplayName));
		LabelReady.text = string.Empty;
		LabelInfo.text = Localizer.GetString("gui_tap_to_start");
		LabelTitle.text = Localizer.GetString("gui_player") + " " + (id + 1);
		LabelPerk.text = Localizer.GetString("gui_perks");
		LabelCharacter.text = Localizer.GetString("gui_characters");
		SortCharactersByPerk(perks[perkIndex].Perk);
		touchButtons = new AxisTouchButtonsWrapper[2];
		touchButtons[0] = new AxisTouchButtonsWrapper("perk_button_left", "perk_button_right");
		touchButtons[1] = new AxisTouchButtonsWrapper("character_button_left", "character_button_right");
		for (int i = 0; i < touchButtons.Length; i++)
		{
			touchButtons[i].InitializeIngame();
		}
	}

	private void Update()
	{
		for (int i = 0; i < touchButtons.Length; i++)
		{
			touchButtons[i].Update();
		}
		HandleInput();
	}

	public void TweenOut()
	{
		handleInput = false;
		AlphaReady.enabled = true;
		AlphaReady.Play(false);
		BottomPositionTween.Play(false);
		if (!isReady)
		{
			TopPositionTween.callWhenFinished = "OnTweenOutComplete";
		}
		TopPositionTween.Play(false);
	}

	public void OnTweenOutComplete()
	{
		handleInput = true;
		manager.RemovePlayer(id);
	}

	public void SetManagerReference(MenuCharacterSelect manager)
	{
		this.manager = manager;
	}

	public void SetPlayerReference(Player player)
	{
		this.player = player;
	}

	public void OnReady()
	{
		isReady = true;
		LabelReady.text = Localizer.GetString("gui_ready");
		AlphaReady.enabled = true;
		AlphaReady.Play(true);
		TitleBars[listIndex].OnFocusLost();
		LabelInfo.text = Localizer.GetString("gui_tap_to_unready");
		player.SetSelectedSoldier(perks[perkIndex].Perk, sortedCharacters[characterIndex].DisplayName, sortedCharacters[characterIndex].Name);
		manager.SetPerkPickedStatus(perks[perkIndex].Perk, true);
		manager.PlaySound(GuiMenuManager.Sound.Confirm);
		manager.SetCharacterReadyStatus(id, true);
	}

	public void OnUnReady()
	{
		isReady = false;
		AlphaReady.enabled = true;
		AlphaReady.Play(false);
		TitleBars[listIndex].OnFocusGained();
		LabelInfo.text = Localizer.GetString("gui_tap_to_start");
		manager.SetCharacterReadyStatus(id, false);
		manager.SetPerkPickedStatus(perks[perkIndex].Perk, false);
	}

	public void UpdateMaterialForCharacterByPerk(Perk perk, bool status)
	{
		if (perks[perkIndex] != null && perk == perks[perkIndex].Perk && !isReady)
		{
			manager.SetMaterialForCharacterByStatusAndID(id, status);
			if (status)
			{
				AlphaReady.enabled = true;
				AlphaReady.Play(true);
				LabelReady.text = Localizer.GetString("gui_menu_character_perk_picked");
			}
			else
			{
				AlphaReady.enabled = true;
				AlphaReady.Play(false);
			}
		}
	}

	public void SetXpText(int current, int total)
	{
		if (current == 0 && total == 0)
		{
			LabelXp.text = string.Empty;
		}
		else
		{
			LabelXp.text = current + " / " + total + " XP";
		}
	}

	public void SetPlayerId(int id)
	{
		this.id = id;
		handleInput = true;
	}

	public int GetId()
	{
		return id;
	}

	public void SetDemoLock(bool demo)
	{
		isDemo = demo;
	}

	private int HandleTouchButtons()
	{
		int num = 0;
		for (int i = 0; i < touchButtons.Length; i++)
		{
			num = touchButtons[i].GetInt();
			if (num != 0)
			{
				SetSelected(i);
				break;
			}
		}
		return num;
	}

	private void HandleInput()
	{
		if (handleInput)
		{
			bool @bool = InputManager.GetBool(InputAction.MenuForward, id);
			bool bool2 = InputManager.GetBool(InputAction.MenuBackward, id);
			int num = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate, id);
			int @int = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate, id);
			if (num == 0)
			{
				num = HandleTouchButtons();
			}
			if (@bool)
			{
				HandleButtonForwardPressed();
			}
			if (bool2)
			{
				HandleButtonBackwardPressed();
			}
			HandleHorizontalAxisInput(num);
			HandleVerticalAxisInput(@int);
		}
	}

	public void HandleButtonForwardPressed()
	{
		CharacterDisplayData characterDisplayData = sortedCharacters[characterIndex];
		PerkData perkData = perks[perkIndex];
		if (characterDisplayData != null && perkData != null)
		{
			if (!isReady)
			{
				if (!perkData.IsAlreadyPicked)
				{
					if (!isDemo || (isDemo && !characterDisplayData.DemoLocked))
					{
						if (!isReady)
						{
							OnReady();
						}
					}
					else
					{
						manager.PlaySound(GuiMenuManager.Sound.UnConfim);
					}
				}
				else
				{
					manager.PlaySound(GuiMenuManager.Sound.UnConfim);
				}
			}
			else
			{
				OnUnReady();
			}
		}
		else
		{
			manager.PlaySound(GuiMenuManager.Sound.UnConfim);
		}
	}

	public void HandleButtonBackwardPressed()
	{
		if (isReady)
		{
			manager.PlaySound(GuiMenuManager.Sound.UnConfim);
			OnUnReady();
		}
		else
		{
			TweenOut();
		}
	}

	public bool CanReady()
	{
		if (isReady)
		{
			return true;
		}
		CharacterDisplayData characterDisplayData = sortedCharacters[characterIndex];
		PerkData perkData = perks[perkIndex];
		if (characterDisplayData != null && perkData != null && !perkData.IsAlreadyPicked && (!isDemo || (isDemo && !characterDisplayData.DemoLocked)))
		{
			return true;
		}
		return false;
	}

	private void SetSelected(int index)
	{
		manager.PlaySound(GuiMenuManager.Sound.MenuButton);
		TitleBars[listIndex].OnFocusLost();
		listIndex = index;
		TitleBars[listIndex].OnFocusGained();
	}

	public void HandleVerticalAxisInput(int axis)
	{
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		if (axis < 1)
		{
			if (listIndex + 1 <= TitleBars.Count - 1 && !isReady)
			{
				manager.PlaySound(GuiMenuManager.Sound.MenuButton);
				TitleBars[listIndex].OnFocusLost();
				listIndex++;
				TitleBars[listIndex].OnFocusGained();
			}
		}
		else if (listIndex - 1 >= 0 && !isReady)
		{
			manager.PlaySound(GuiMenuManager.Sound.MenuButton);
			TitleBars[listIndex].OnFocusLost();
			listIndex--;
			TitleBars[listIndex].OnFocusGained();
		}
	}

	public void HandleHorizontalAxisInput(int axis)
	{
		if (isReady)
		{
			return;
		}
		TitleBars[listIndex].OnHorizontalInput(axis);
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		manager.PlaySound(GuiMenuManager.Sound.CharButton);
		if (listIndex == 0)
		{
			switch (axis)
			{
			case -1:
				perkIndex--;
				if (perkIndex < 0)
				{
					perkIndex = perks.Count - 1;
				}
				break;
			case 1:
				perkIndex++;
				if (perkIndex > perks.Count - 1)
				{
					perkIndex = 0;
				}
				break;
			}
			SortCharactersByPerk(perks[perkIndex].Perk);
			TitleBars[listIndex].SetText(Localizer.GetString(perks[perkIndex].PerkDisplayName));
		}
		else
		{
			if (listIndex != 1)
			{
				return;
			}
			switch (axis)
			{
			case -1:
				characterIndex--;
				if (characterIndex < 0)
				{
					characterIndex = sortedCharacters.Count - 1;
				}
				break;
			case 1:
				characterIndex++;
				if (characterIndex > sortedCharacters.Count - 1)
				{
					characterIndex = 0;
				}
				break;
			}
			TitleBars[listIndex].SetText(Localizer.GetString(sortedCharacters[characterIndex].DisplayName));
			SetCharacterDescription(sortedCharacters[characterIndex].Description);
			PerkData perkData = perks[perkIndex];
			bool isLocked = ((sortedCharacters[characterIndex].DemoLocked || perkData.IsAlreadyPicked) ? true : false);
			if (isDemo)
			{
				SetCharacterLockState(sortedCharacters[characterIndex].DemoLocked);
				manager.SpawnSoldierPrefab(id, sortedCharacters[characterIndex].Name, isLocked);
			}
			else
			{
				SetCharacterLockState(false);
				manager.SpawnSoldierPrefab(id, sortedCharacters[characterIndex].Name, perkData.IsAlreadyPicked);
			}
		}
	}

	private void SortCharactersByPerk(Perk perk)
	{
		characterIndex = 0;
		sortedCharacters.Clear();
		for (int i = 0; i < characters.Count; i++)
		{
			if (characters[i].Perk == perk)
			{
				sortedCharacters.Add(characters[i]);
			}
		}
		CharacterBar.SetText(Localizer.GetString(sortedCharacters[characterIndex].DisplayName));
		SetCharacterPerk(sortedCharacters[characterIndex].Perk);
		SetCharacterDescription(sortedCharacters[characterIndex].Description);
		PerkData perkData = perks[perkIndex];
		bool isLocked = ((sortedCharacters[characterIndex].DemoLocked || perkData.IsAlreadyPicked) ? true : false);
		if (perkData.IsAlreadyPicked && !sortedCharacters[characterIndex].DemoLocked)
		{
			LabelReady.text = Localizer.GetString("gui_menu_character_perk_picked");
		}
		else
		{
			LabelReady.text = string.Empty;
		}
		if (isDemo)
		{
			SetCharacterLockState(sortedCharacters[characterIndex].DemoLocked);
			manager.SpawnSoldierPrefab(id, sortedCharacters[characterIndex].Name, isLocked);
		}
		else
		{
			SetCharacterLockState(false);
			manager.SpawnSoldierPrefab(id, sortedCharacters[characterIndex].Name, perkData.IsAlreadyPicked);
		}
	}

	private void SetCharacterDescription(string key)
	{
		LabelDescription.text = Localizer.GetString(key);
	}

	private void SetCharacterLockState(bool isLocked)
	{
		if (isLocked)
		{
			LabelDescription.color = Color.gray;
		}
		else
		{
			LabelDescription.color = Color.white;
		}
		Lock.gameObject.SetActive(isLocked);
	}

	private void SetCharacterPerk(Perk perk)
	{
		float num = 1f;
		int num2 = MetaManager.Instance.PerkLevel(perk);
		int perkMaxLevel = MetaManager.Instance.PerkMaxLevel;
		if (num2 < perkMaxLevel)
		{
			float num3 = MetaManager.Instance.PerkTotalExperience(perk);
			float num4 = MetaManager.Instance.PerkExperienceByLevel(num2 + 1);
			float num5 = MetaManager.Instance.PerkExperienceByLevel(num2);
			num = (num3 - num5) / (num4 - num5);
			SetXpText((int)(num3 - num5), (int)(num4 - num5));
			SetLevelBarFraction(num);
		}
		else
		{
			SetLevelBarFraction(1f);
			SetXpText(0, 0);
		}
		Labellevel.text = Localizer.GetString("gui_level") + " " + num2;
	}

	private void SetLevelBarFraction(float fraction)
	{
		LevelBar.gameObject.SetActive(true);
		LevelBar.sliderValue = fraction;
	}

	private void ResetDelay()
	{
		handleAxisInput = true;
	}
}
