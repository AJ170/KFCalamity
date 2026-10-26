using UnityEngine;

public class Player
{
	private int id;

	private Perk selectedPerk;

	private string selectedSoldier;

	private string selectedCharacter;

	private GameObject soldier;

	private int inputID;

	private PlayerGUI playerGui;

	public int ID
	{
		get
		{
			return id;
		}
	}

	public Perk SelectedPerk
	{
		get
		{
			return selectedPerk;
		}
	}

	public string SelectedSoldier
	{
		get
		{
			return selectedSoldier;
		}
	}

	public string SelectedCharacter
	{
		get
		{
			return selectedCharacter;
		}
	}

	public int InputID
	{
		get
		{
			return inputID;
		}
		set
		{
			inputID = value;
		}
	}

	public PlayerGUI PlayerGUI
	{
		get
		{
			return playerGui;
		}
	}

	public Player(int id)
		: this(id, id)
	{
	}

	public Player(int id, int inputID)
	{
		this.id = id;
		this.inputID = inputID;
	}

	public void Update()
	{
	}

	public void Shutdown()
	{
	}

	public void SetSelectedSoldier(Perk perk, string character, string soldier = "")
	{
		selectedPerk = perk;
		selectedCharacter = character;
		selectedSoldier = soldier;
	}

	public void CreateIngameGUI(GuiInGameManager guiManager, PlayerShopWindow shopWindow = null)
	{
		if (guiManager != null)
		{
			playerGui = guiManager.CreatePlayerGui(this, shopWindow);
		}
	}

	public void SetSoldier(GameObject newSoldier)
	{
		if (soldier != null)
		{
			Debug.LogWarning(string.Format("[Player:SetSoldier] Overwriting existing soldier for player {0}.", id));
		}
		soldier = newSoldier;
	}

	public bool HasSoldier()
	{
		return soldier != null;
	}

	public void ClearSoldier()
	{
		soldier = null;
	}

	public GameObject GetSoldier()
	{
		return soldier;
	}
}
