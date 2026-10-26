using UnityEngine;

public class HudMoneyHandler
{
	private UILabel moneyLabel;

	private HudCashScroll moneyScroller;

	private int displayMoney;

	private float moneyMultiplier;

	private bool isFirstEntry = true;

	private PlayerHudBase playerHud;

	private PlayerGUI playerGui;

	public PlayerGUI PlayerGui
	{
		get
		{
			return playerGui;
		}
		set
		{
			playerGui = value;
		}
	}

	public PlayerHudBase PlayerHud
	{
		get
		{
			return playerHud;
		}
		set
		{
			playerHud = value;
		}
	}

	public HudCashScroll MoneyScroll
	{
		get
		{
			return moneyScroller;
		}
		set
		{
			moneyScroller = value;
		}
	}

	public UILabel MoneyLabel
	{
		get
		{
			return moneyLabel;
		}
		set
		{
			moneyLabel = value;
		}
	}

	public int DisplayMoney
	{
		get
		{
			return displayMoney;
		}
		set
		{
			displayMoney = value;
		}
	}

	public float MoneyMultiplier
	{
		get
		{
			return moneyMultiplier;
		}
		set
		{
			moneyMultiplier = value;
		}
	}

	public void AddMoney(int baseAmount, int money, float multiplier)
	{
		string currentHexCodeColor = PlayerHud.GetCurrentHexCodeColor();
		MoneyMultiplier = multiplier;
		string surfix = ((!(multiplier <= 1f)) ? (currentHexCodeColor + "x" + multiplier.ToString(".0#") + "[-]") : string.Empty);
		moneyScroller.Add(baseAmount, money, Color.white, surfix);
		if (isFirstEntry)
		{
			SyncMoney();
			isFirstEntry = false;
		}
	}

	public void SyncMoney()
	{
		if (PlayerHud != null)
		{
			if (PlayerGui == null)
			{
				PlayerGui = playerHud.PlayerGui;
			}
			int num = PlayerGui.Money();
			int num2 = ((moneyScroller != null) ? moneyScroller.TotalQueueMoney : 0);
			DisplayMoney = num - num2;
			if (DisplayMoney < 0)
			{
				MoneyLabel.text = "£ 0";
			}
			else
			{
				MoneyLabel.text = "£ " + DisplayMoney;
			}
		}
	}
}
