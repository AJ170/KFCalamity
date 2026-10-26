using System;
using System.Collections.Generic;
using UnityEngine;

public class OuyaPurchaseManager : MonoBehaviour, OuyaSDK.IFetchGamerUUIDListener, OuyaSDK.IGetProductsListener, OuyaSDK.IPurchaseListener, OuyaSDK.IGetReceiptsListener
{
	public enum PurchaseOutcome
	{
		Success = 0,
		Failure = 1,
		Cancelled = 2,
		Busy = 3
	}

	public enum InternalState
	{
		Inactive = 0,
		CheckingStoredData = 1,
		WaitingForIAPInit = 2,
		RequestingUUID = 3,
		RequestingInitialReceipt = 4,
		PreparingProduct = 5,
		Passive = 6,
		Purchasing = 7,
		AwaitingReceipt = 8,
		Done = 9
	}

	private const string fullGameProductID = "CalamityFull";

	private const string purchasesFileName = "purchases";

	public static OuyaPurchaseManager Instance;

	private OuyaSDK.Receipt fullGameReceipt;

	private string gamerUUID = string.Empty;

	private bool gamePurchased;

	private InternalState state;

	private Action<PurchaseOutcome, string> callback;

	public bool GamePurchased
	{
		get
		{
			return gamePurchased;
		}
	}

	public bool CanPurchase
	{
		get
		{
			return state == InternalState.Passive;
		}
	}

	public bool InitializationDone
	{
		get
		{
			return state >= InternalState.PreparingProduct;
		}
	}

	private void Awake()
	{
		if (!KFCompat.IsAndroid)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		OuyaSDK.registerGetProductsListener(this);
		OuyaSDK.registerPurchaseListener(this);
		OuyaSDK.registerGetReceiptsListener(this);
		OuyaSDK.registerFetchGamerUUIDListener(this);
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		OuyaSDK.unregisterGetProductsListener(this);
		OuyaSDK.unregisterPurchaseListener(this);
		OuyaSDK.unregisterGetReceiptsListener(this);
		OuyaSDK.unregisterFetchGamerUUIDListener(this);
		Instance = null;
	}

	private void Start()
	{
		CheckStoredData();
	}

	private void Update()
	{
		if (state == InternalState.WaitingForIAPInit && OuyaSDK.isIAPInitComplete())
		{
			state = InternalState.RequestingUUID;
			RequestUUID();
		}
	}

	public void PurchaseGame(Action<PurchaseOutcome, string> callback)
	{
		if (CanPurchase)
		{
			this.callback = callback;
			RequestPurchaseOfGame();
		}
		else
		{
			callback(PurchaseOutcome.Busy, null);
		}
	}

	private void RequestUUID()
	{
		OuyaSDK.fetchGamerUUID();
	}

	private void RequestPurchaseOfGame()
	{
		state = InternalState.Purchasing;
		OuyaSDK.requestPurchase("CalamityFull");
	}

	private void RequestReceipt()
	{
		OuyaSDK.requestReceiptList();
	}

	private void CallCallback(PurchaseOutcome outcome, string message = null)
	{
		if (callback != null)
		{
			callback(outcome, message);
		}
	}

	private void GoInactive()
	{
		state = InternalState.Inactive;
		base.enabled = false;
	}

	private void ReturnToPassive()
	{
		callback = null;
		state = InternalState.Passive;
	}

	public void OuyaGetProductsOnSuccess(List<OuyaSDK.Product> products)
	{
	}

	public void OuyaGetProductsOnFailure(int errorCode, string errorMessage)
	{
	}

	public void OuyaGetProductsOnCancel()
	{
	}

	public void OuyaPurchaseOnSuccess(OuyaSDK.Product product)
	{
		if (state == InternalState.Purchasing)
		{
			state = InternalState.AwaitingReceipt;
			RequestReceipt();
		}
	}

	public void OuyaPurchaseOnFailure(int errorCode, string errorMessage)
	{
		if (state == InternalState.Purchasing)
		{
			string message = string.Format("OuyaPurchaseOnFailure: error={0} errorMessage={1}", errorCode, errorMessage);
			CallCallback(PurchaseOutcome.Failure, message);
			ReturnToPassive();
		}
	}

	public void OuyaPurchaseOnCancel()
	{
		CallCallback(PurchaseOutcome.Cancelled);
		ReturnToPassive();
	}

	public void OuyaGetReceiptsOnSuccess(List<OuyaSDK.Receipt> receipts)
	{
		if (state != InternalState.RequestingInitialReceipt && state != InternalState.AwaitingReceipt)
		{
			return;
		}
		foreach (OuyaSDK.Receipt receipt in receipts)
		{
			if (receipt.getIdentifier() == "CalamityFull")
			{
				fullGameReceipt = receipt;
				if (state == InternalState.AwaitingReceipt)
				{
					CallCallback(PurchaseOutcome.Success);
					callback = null;
				}
				gamePurchased = true;
				SaveStoredData();
				state = InternalState.Done;
			}
		}
		if (state == InternalState.RequestingInitialReceipt)
		{
			state = InternalState.Passive;
		}
		else if (state == InternalState.AwaitingReceipt)
		{
			CallCallback(PurchaseOutcome.Failure, "Failed to receive receipt");
			callback = null;
			state = InternalState.Passive;
		}
	}

	public void OuyaGetReceiptsOnFailure(int errorCode, string errorMessage)
	{
		state = InternalState.Inactive;
	}

	public void OuyaGetReceiptsOnCancel()
	{
		state = InternalState.Inactive;
	}

	public void OuyaFetchGamerUUIDOnSuccess(string gamerUUID)
	{
		this.gamerUUID = gamerUUID;
		state = InternalState.RequestingInitialReceipt;
		RequestReceipt();
	}

	public void OuyaFetchGamerUUIDOnFailure(int errorCode, string errorMessage)
	{
		state = InternalState.Inactive;
	}

	public void OuyaFetchGamerUUIDOnCancel()
	{
		state = InternalState.Inactive;
	}

	private void CheckStoredData()
	{
		state = InternalState.CheckingStoredData;
		FileManager instance = FileManager.Instance;
		if (instance != null)
		{
			List<string> list = instance.Load(instance.RootPath + "purchases");
			if (list != null && list.Contains("CalamityFull"))
			{
				gamePurchased = true;
			}
		}
		if (gamePurchased)
		{
			state = InternalState.Done;
			base.enabled = false;
		}
		else
		{
			state = InternalState.WaitingForIAPInit;
		}
	}

	private void SaveStoredData()
	{
		FileManager instance = FileManager.Instance;
		if (instance != null)
		{
			instance.Save(instance.RootPath + "purchases", new List<string> { "CalamityFull" });
		}
	}

	private void print(string text)
	{
		Debug.Log("[PurchaseManager] " + text);
		DevelopmentDisplay.OverrideString = text;
	}
}
