using System.Collections.Generic;
using UnityEngine;

public class WaveBreakGamePhase : GamePhaseBase
{
	protected GameModeBase gameMode;

	protected WaveManager waveManager;

	protected GuiInGameManager guiManager;

	protected CommandQueue commandQueue;

	public WaveBreakGamePhase(GameModeBase gameMode)
		: base(gameMode.GamePhaseManagerRef())
	{
		this.gameMode = gameMode;
		waveManager = gameMode.WaveManagerRef();
		guiManager = gameMode.GameStateRef().Context.GUIManager;
		commandQueue = new CommandQueue();
	}

	public override void OnEnter()
	{
		int num = waveManager.CurrentWaveIndex();
		if (num >= 0)
		{
			if (guiManager != null)
			{
				guiManager.OnWaveComplete();
			}
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.SetTemporaryVolume(0.5f);
			}
			WaveClearedMessage();
			commandQueue.Add(RewardAliveSoldiers, 1f);
			commandQueue.Add(null, 2f);
			commandQueue.Add(MakeSoldierInvulnerable);
			commandQueue.Add(RestoreSoldierHealth);
			commandQueue.Add(RefillSyringes);
			commandQueue.Add(TryRespawn);
			commandQueue.Add(ShopMessage, 1f);
			commandQueue.Add(OpenShop, 3f);
			commandQueue.Add(ResetSoldierAbilityCooldown);
			commandQueue.Add(DestroyPickups);
		}
		else
		{
			commandQueue.Add(GameStartMessage, 5f);
			commandQueue.Add(EndPhase, 6f);
		}
	}

	public override void Update()
	{
		commandQueue.Update();
		if (guiManager == null || (guiManager != null && guiManager.IsShopOpen() && guiManager.IsShopPhaseDone()))
		{
			if (guiManager != null)
			{
				guiManager.CloseShop();
			}
			ClearMemory();
			SetSoldierInvulnerability(false);
			commandQueue.Add(UnlockPlayerInput, 0.2f);
			commandQueue.Add(NextWaveMessage, 3f);
			commandQueue.Add(EndPhase, 3f);
		}
	}

	private void RespawnMessage()
	{
		if (guiManager != null)
		{
			guiManager.DisplayMessage(Localizer.GetString("gui_message_respawning"), 3f);
		}
	}

	private void WaveClearedMessage()
	{
		if (guiManager != null)
		{
			guiManager.DisplayMessage(Localizer.GetString("gui_message_wave_cleared"), 3f);
		}
	}

	protected void GameStartMessage()
	{
		if (guiManager != null)
		{
			guiManager.DisplayTimerMessage(Localizer.GetString("gui_message_game_start"), 5f);
		}
	}

	private void NextWaveMessage()
	{
		if (guiManager != null)
		{
			guiManager.DisplayTimerMessage(Localizer.GetString("gui_message_next_wave"), 3f);
		}
	}

	private void ShopMessage()
	{
		if (guiManager != null)
		{
			guiManager.DisplayTimerMessage(Localizer.GetString("gui_message_open_shop"), 3f);
		}
	}

	protected void EndPhase()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.SetTemporaryVolume(1f);
		}
		if (waveManager.IsBossWave(waveManager.NextWaveIndex()))
		{
			GotoBossPhase();
		}
		else
		{
			GotoWavePhase();
		}
	}

	protected virtual void GotoBossPhase()
	{
		Debug.LogWarning("WaveBreakBaseGamePhase:GotoBossPhase() Implement this method");
	}

	protected virtual void GotoWavePhase()
	{
		Debug.LogWarning("WaveBreakBaseGamePhase:GotoWavePhase() Implement this method");
	}

	private void TryRespawn()
	{
		if (gameMode.NeedRespawn())
		{
			commandQueue.Insert(RespawnMessage, 1f, 1);
			commandQueue.Insert(gameMode.RespawnPlayers, 3f, 2);
		}
	}

	protected virtual void OpenShop()
	{
		if (guiManager != null)
		{
			guiManager.OpenShop();
		}
		SetPlayerInputLock(true);
		SpeechManager.PlaySpeech(SpeechManager.SpeechLine.OpenShop);
	}

	private void ClearMemory()
	{
		Resources.UnloadUnusedAssets();
	}

	private void RewardAliveSoldiers()
	{
		SoldierManager instance = SoldierManager.Instance;
		PlayerStatsManager instance2 = PlayerStatsManager.Instance;
		if (instance == null || instance2 == null)
		{
			return;
		}
		int num = 250;
		List<SoldierItem> aliveSoldiers = instance.GetAliveSoldiers();
		foreach (SoldierItem item in aliveSoldiers)
		{
			PlayerStats playerStats = instance2.StatsByPlayer(item.Player);
			if (playerStats != null)
			{
				playerStats.AddMoney(num);
				PlayerGUILink component = item.SoldierObject.GetComponent<PlayerGUILink>();
				if (component != null)
				{
					component.OnMoneyGained(num);
				}
			}
		}
	}

	private void DestroyPickups()
	{
		PickupManager instance = PickupManager.Instance;
		if (instance != null)
		{
			if (instance.Mode == PickupSystemMode.Dynamic)
			{
				instance.Reset();
			}
			else if (instance.Mode == PickupSystemMode.Static)
			{
				instance.DeleteActivePickupsByPickupType(PickupType.Weapon);
			}
		}
	}

	private void MakeSoldierInvulnerable()
	{
		SetSoldierInvulnerability(true);
	}

	private void RestoreSoldierHealth()
	{
		SoldierManager instance = SoldierManager.Instance;
		if (instance == null)
		{
			return;
		}
		List<SoldierItem> aliveSoldiers = instance.GetAliveSoldiers();
		foreach (SoldierItem item in aliveSoldiers)
		{
			Health component = item.SoldierObject.GetComponent<Health>();
			if (component != null)
			{
				component.RestoreHealthExact(component.MaxHealth);
			}
		}
	}

	private void UnlockPlayerInput()
	{
		SetPlayerInputLock(false);
	}

	private void RefillSyringes()
	{
		SoldierManager instance = SoldierManager.Instance;
		if (instance == null)
		{
			return;
		}
		List<SoldierItem> aliveSoldiers = instance.GetAliveSoldiers();
		foreach (SoldierItem item in aliveSoldiers)
		{
			PlayerAbilityController component = item.SoldierObject.GetComponent<PlayerAbilityController>();
			if (component != null)
			{
				Ability ability = component.AbilityBySlot(AbilitySlot.SyringeSelf);
				if (ability != null)
				{
					ability.RefillCharges(ability.MaxCharges);
				}
			}
		}
	}

	private void ResetSoldierAbilityCooldown()
	{
		SoldierManager instance = SoldierManager.Instance;
		if (instance == null)
		{
			return;
		}
		List<SoldierItem> aliveSoldiers = instance.GetAliveSoldiers();
		foreach (SoldierItem item in aliveSoldiers)
		{
			PlayerAbilityController component = item.SoldierObject.GetComponent<PlayerAbilityController>();
			if (component != null)
			{
				component.ResetPerkCooldown();
			}
		}
	}

	private void SetPlayerInputLock(bool lockInput)
	{
		if (SoldierManager.Instance != null)
		{
			SoldierManager.Instance.SetLockedInput(lockInput, this);
		}
	}

	private void SetSoldierInvulnerability(bool invulnerable)
	{
		if (SoldierManager.Instance != null)
		{
			SoldierManager.Instance.SetInvulnerable(invulnerable);
		}
	}
}
