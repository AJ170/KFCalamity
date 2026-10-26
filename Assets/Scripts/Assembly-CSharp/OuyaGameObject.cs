using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class OuyaGameObject : MonoBehaviour
{
	public string DEVELOPER_ID = "310a8f51-4d6e-4ae5-bda0-b93878e5f5d0";

	public bool debugOff;

	public string[] Purchasables = new string[3] { "long_sword", "sharp_axe", "__DECLINED__THIS_PURCHASE" };

	[HideInInspector]
	private static string m_inputData;

	private static OuyaGameObject m_instance;

	private List<OuyaSDK.Product> m_products = new List<OuyaSDK.Product>();

	private List<OuyaSDK.Receipt> m_receipts = new List<OuyaSDK.Receipt>();

	public static string InputData
	{
		get
		{
			return m_inputData;
		}
		set
		{
			m_inputData = value;
		}
	}

	public static OuyaGameObject Singleton
	{
		get
		{
			if (null == m_instance)
			{
				GameObject gameObject = GameObject.Find("OuyaGameObject");
				if ((bool)gameObject)
				{
					m_instance = gameObject.GetComponent<OuyaGameObject>();
				}
			}
			return m_instance;
		}
	}

	public void onMenuButtonUp(string ignore)
	{
		foreach (OuyaSDK.IMenuButtonUpListener menuButtonUpListener in OuyaSDK.getMenuButtonUpListeners())
		{
			menuButtonUpListener.OuyaMenuButtonUp();
		}
	}

	public void onMenuAppearing(string ignore)
	{
		foreach (OuyaSDK.IMenuAppearingListener menuAppearingListener in OuyaSDK.getMenuAppearingListeners())
		{
			menuAppearingListener.OuyaMenuAppearing();
		}
	}

	public void onPause()
	{
		foreach (OuyaSDK.IPauseListener pauseListener in OuyaSDK.getPauseListeners())
		{
			pauseListener.OuyaOnPause();
		}
	}

	public void onResume()
	{
		foreach (OuyaSDK.IResumeListener resumeListener in OuyaSDK.getResumeListeners())
		{
			resumeListener.OuyaOnResume();
		}
	}

	public void FetchGamerUUIDSuccessListener(string gamerUUID)
	{
		Debug.LogError(string.Format("FetchGamerUUIDSuccessListener gamerUUID={0}", gamerUUID));
		InvokeOuyaFetchGamerUUIDOnSuccess(gamerUUID);
	}

	public void FetchGamerUUIDFailureListener(string jsonData)
	{
		Debug.LogError(string.Format("FetchGamerUUIDFailureListener jsonData={0}", jsonData));
		InvokeOuyaFetchGamerUUIDOnFailure(0, jsonData);
	}

	public void FetchGamerUUIDCancelListener(string ignore)
	{
		InvokeOuyaFetchGamerUUIDOnCancel();
	}

	public void ProductListClearListener(string ignore)
	{
		m_products.Clear();
	}

	public void ProductListListener(string jsonData)
	{
		if (string.IsNullOrEmpty(jsonData))
		{
			Debug.LogError("OuyaSDK.ProductListListener: received empty jsondata");
			return;
		}
		Debug.Log(string.Format("OuyaSDK.ProductListListener: jsonData={0}", jsonData));
		OuyaSDK.Product item = JsonMapper.ToObject<OuyaSDK.Product>(jsonData);
		m_products.Add(item);
	}

	public void ProductListFailureListener(string jsonData)
	{
		Debug.LogError(string.Format("ProductListFailureListener jsonData={0}", jsonData));
		InvokeOuyaGetReceiptsOnFailure(0, jsonData);
	}

	public void ProductListCompleteListener(string ignore)
	{
		foreach (OuyaSDK.Product product in m_products)
		{
			Debug.Log(string.Format("ProductListCompleteListener Product id={0} name={1} price={2}", product.getIdentifier(), product.getName(), product.getPriceInCents()));
		}
		InvokeOuyaGetProductsOnSuccess(m_products);
	}

	public void PurchaseSuccessListener(string jsonData)
	{
		Debug.LogError(string.Format("PurchaseSuccessListener jsonData={0}", jsonData));
		OuyaSDK.Product product = JsonMapper.ToObject<OuyaSDK.Product>(jsonData);
		InvokeOuyaPurchaseOnSuccess(product);
	}

	public void PurchaseFailureListener(string jsonData)
	{
		Debug.LogError(string.Format("PurchaseFailureListener jsonData={0}", jsonData));
		InvokeOuyaPurchaseOnFailure(0, jsonData);
	}

	public void PurchaseCancelListener(string ignore)
	{
		InvokeOuyaPurchaseOnCancel();
	}

	public void ReceiptListClearListener(string ignore)
	{
		m_receipts.Clear();
	}

	public void ReceiptListListener(string jsonData)
	{
		if (string.IsNullOrEmpty(jsonData))
		{
			Debug.LogError("OuyaSDK.ReceiptListListener: received empty jsondata");
			return;
		}
		Debug.Log(string.Format("OuyaSDK.ReceiptListListener: jsonData={0}", jsonData));
		OuyaSDK.Receipt item = JsonMapper.ToObject<OuyaSDK.Receipt>(jsonData);
		m_receipts.Add(item);
	}

	public void ReceiptListCompleteListener(string ignore)
	{
		foreach (OuyaSDK.Receipt receipt in m_receipts)
		{
			Debug.Log(string.Format("ReceiptListCompleteListener Receipt id={0} price={1} purchaseDate={2} generatedDate={3}", receipt.getIdentifier(), receipt.getPriceInCents(), receipt.getPurchaseDate(), receipt.getGeneratedDate()));
		}
		InvokeOuyaGetReceiptsOnSuccess(m_receipts);
	}

	public void ReceiptListFailureListener(string jsonData)
	{
		Debug.LogError(string.Format("ReceiptListFailureListener jsonData={0}", jsonData));
		InvokeOuyaGetReceiptsOnFailure(0, jsonData);
	}

	public void ReceiptListCancelListener(string ignore)
	{
		InvokeOuyaGetReceiptsOnCancel();
	}

	private void Awake()
	{
		m_instance = this;
	}

	private void Start()
	{
		Input.ResetInputAxes();
		Application.targetFrameRate = 60;
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		try
		{
			OuyaSDK.initialize(DEVELOPER_ID);
		}
		catch (Exception arg)
		{
			Debug.LogError(string.Format("Failed to initialize OuyaSDK exception={0}", arg));
		}
	}

	public void RequestUnityAwake(string ignore)
	{
		OuyaSDK.initialize(DEVELOPER_ID);
	}

	public void SendIAPInitComplete(string ignore)
	{
		OuyaSDK.setIAPInitComplete();
	}

	public void InvokeOuyaFetchGamerUUIDOnSuccess(string gamerUUID)
	{
		foreach (OuyaSDK.IFetchGamerUUIDListener fetchGamerUUIDListener in OuyaSDK.getFetchGamerUUIDListeners())
		{
			if (fetchGamerUUIDListener != null)
			{
				fetchGamerUUIDListener.OuyaFetchGamerUUIDOnSuccess(gamerUUID);
			}
		}
	}

	public void InvokeOuyaFetchGamerUUIDOnFailure(int errorCode, string errorMessage)
	{
		Debug.LogError(string.Format("InvokeOuyaFetchGamerUUIDOnFailure: error={0} errorMessage={1}", errorCode, errorMessage));
		foreach (OuyaSDK.IFetchGamerUUIDListener fetchGamerUUIDListener in OuyaSDK.getFetchGamerUUIDListeners())
		{
			if (fetchGamerUUIDListener != null)
			{
				fetchGamerUUIDListener.OuyaFetchGamerUUIDOnFailure(errorCode, errorMessage);
			}
		}
	}

	public void InvokeOuyaFetchGamerUUIDOnCancel()
	{
		foreach (OuyaSDK.IFetchGamerUUIDListener fetchGamerUUIDListener in OuyaSDK.getFetchGamerUUIDListeners())
		{
			if (fetchGamerUUIDListener != null)
			{
				fetchGamerUUIDListener.OuyaFetchGamerUUIDOnCancel();
			}
		}
	}

	public void InvokeOuyaGetProductsOnSuccess(List<OuyaSDK.Product> products)
	{
		foreach (OuyaSDK.IGetProductsListener getProductsListener in OuyaSDK.getGetProductsListeners())
		{
			if (getProductsListener != null)
			{
				getProductsListener.OuyaGetProductsOnSuccess(products);
			}
		}
	}

	public void InvokeOuyaGetProductsOnFailure(int errorCode, string errorMessage)
	{
		Debug.LogError(string.Format("InvokeOuyaGetProductsOnFailure: error={0} errorMessage={1}", errorCode, errorMessage));
		foreach (OuyaSDK.IGetProductsListener getProductsListener in OuyaSDK.getGetProductsListeners())
		{
			if (getProductsListener != null)
			{
				getProductsListener.OuyaGetProductsOnFailure(errorCode, errorMessage);
			}
		}
	}

	public void InvokeOuyaPurchaseOnSuccess(OuyaSDK.Product product)
	{
		foreach (OuyaSDK.IPurchaseListener purchaseListener in OuyaSDK.getPurchaseListeners())
		{
			if (purchaseListener != null)
			{
				purchaseListener.OuyaPurchaseOnSuccess(product);
			}
		}
	}

	public void InvokeOuyaPurchaseOnFailure(int errorCode, string errorMessage)
	{
		Debug.LogError(string.Format("InvokeOuyaPurchaseOnFailure: error={0} errorMessage={1}", errorCode, errorMessage));
		foreach (OuyaSDK.IPurchaseListener purchaseListener in OuyaSDK.getPurchaseListeners())
		{
			if (purchaseListener != null)
			{
				purchaseListener.OuyaPurchaseOnFailure(errorCode, errorMessage);
			}
		}
	}

	public void InvokeOuyaPurchaseOnCancel()
	{
		foreach (OuyaSDK.IPurchaseListener purchaseListener in OuyaSDK.getPurchaseListeners())
		{
			if (purchaseListener != null)
			{
				purchaseListener.OuyaPurchaseOnCancel();
			}
		}
	}

	public void InvokeOuyaGetReceiptsOnSuccess(List<OuyaSDK.Receipt> receipts)
	{
		foreach (OuyaSDK.IGetReceiptsListener getReceiptsListener in OuyaSDK.getGetReceiptsListeners())
		{
			if (getReceiptsListener != null)
			{
				getReceiptsListener.OuyaGetReceiptsOnSuccess(receipts);
			}
		}
	}

	public void InvokeOuyaGetReceiptsOnFailure(int errorCode, string errorMessage)
	{
		Debug.LogError(string.Format("InvokeOuyaGetReceiptsOnFailure: error={0} errorMessage={1}", errorCode, errorMessage));
		foreach (OuyaSDK.IGetReceiptsListener getReceiptsListener in OuyaSDK.getGetReceiptsListeners())
		{
			if (getReceiptsListener != null)
			{
				getReceiptsListener.OuyaGetReceiptsOnFailure(errorCode, errorMessage);
			}
		}
	}

	public void InvokeOuyaGetReceiptsOnCancel()
	{
		foreach (OuyaSDK.IGetReceiptsListener getReceiptsListener in OuyaSDK.getGetReceiptsListeners())
		{
			if (getReceiptsListener != null)
			{
				getReceiptsListener.OuyaGetReceiptsOnCancel();
			}
		}
	}

	private void FixedUpdate()
	{
		OuyaSDK.UpdateJoysticks();
	}

	public void DebugLog(string message)
	{
		Debug.Log(message);
	}

	public void DebugLogError(string message)
	{
		Debug.LogError(message);
	}
}
