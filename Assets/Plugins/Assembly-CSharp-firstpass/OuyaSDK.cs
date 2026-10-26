using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class OuyaSDK
{
	public enum KeyEnum
	{
		NONE = -1,
		BUTTON_O = 96,
		BUTTON_U = 99,
		BUTTON_Y = 100,
		BUTTON_A = 97,
		BUTTON_LB = 102,
		BUTTON_LT = 104,
		BUTTON_RB = 103,
		BUTTON_RT = 105,
		BUTTON_L3 = 106,
		BUTTON_R3 = 107,
		BUTTON_SYSTEM = 108,
		BUTTON_START = -2,
		BUTTON_SELECT = -1,
		BUTTON_ESCAPE = 41,
		AXIS_LSTICK_X = 0,
		AXIS_LSTICK_Y = 1,
		AXIS_RSTICK_X = 11,
		AXIS_RSTICK_Y = 14,
		BUTTON_DPAD_UP = 19,
		BUTTON_DPAD_RIGHT = 22,
		BUTTON_DPAD_DOWN = 20,
		BUTTON_DPAD_LEFT = 21,
		BUTTON_DPAD_CENTER = 23,
		BUTTON_BACK = 24,
		HARMONIX_ROCK_BAND_GUITAR_GREEN = 25,
		HARMONIX_ROCK_BAND_GUITAR_RED = 26,
		HARMONIX_ROCK_BAND_GUITAR_YELLOW = 27,
		HARMONIX_ROCK_BAND_GUITAR_BLUE = 28,
		HARMONIX_ROCK_BAND_GUITAR_ORANGE = 29,
		HARMONIX_ROCK_BAND_GUITAR_LOWER = 30,
		HARMONIX_ROCK_BAND_GUITAR_WHAMMI = 31,
		HARMONIX_ROCK_BAND_GUITAR_PICKUP = 32,
		HARMONIX_ROCK_BAND_GUITAR_STRUM = 33,
		HARMONIX_ROCK_BAND_DRUMKIT_GREEN = 34,
		HARMONIX_ROCK_BAND_DRUMKIT_RED = 35,
		HARMONIX_ROCK_BAND_DRUMKIT_YELLOW = 36,
		HARMONIX_ROCK_BAND_DRUMKIT_BLUE = 37,
		HARMONIX_ROCK_BAND_DRUMKIT_ORANGE = 38,
		HARMONIX_ROCK_BAND_DRUMKIT_A = 39,
		HARMONIX_ROCK_BAND_DRUMKIT_B = 40,
		HARMONIX_ROCK_BAND_DRUMKIT_X = 41,
		HARMONIX_ROCK_BAND_DRUMKIT_Y = 42
	}

	public enum AxisEnum
	{
		NONE = -1,
		AXIS_LSTICK_X = 0,
		AXIS_LSTICK_Y = 1,
		AXIS_RSTICK_X = 2,
		AXIS_RSTICK_Y = 3,
		AXIS_LTRIGGER = 4,
		AXIS_RTRIGGER = 5
	}

	public enum OuyaPlayer
	{
		player1 = 1,
		player2 = 2,
		player3 = 3,
		player4 = 4,
		player5 = 5,
		player6 = 6,
		player7 = 7,
		player8 = 8,
		player9 = 9,
		player10 = 10,
		player11 = 11,
		none = 0
	}

	public class GenericListener<T>
	{
		public delegate void SuccessDelegate(T data);

		public delegate void FailureDelegate(int errorCode, string errorMessage);

		public SuccessDelegate onSuccess;

		public FailureDelegate onFailure;
	}

	public class CancelIgnoringIapResponseListener<T> : GenericListener<T>
	{
	}

	public class InputButtonListener<T> : GenericListener<T>
	{
	}

	public class InputAxisListener<T> : GenericListener<T>
	{
	}

	public class DeviceListener<T> : GenericListener<T>
	{
	}

	public class Purchasable
	{
		public string productId = string.Empty;

		public Purchasable(string productId)
		{
			this.productId = productId;
		}

		public string getProductId()
		{
			return productId;
		}

		public static implicit operator string(Purchasable rhs)
		{
			return rhs.getProductId();
		}

		public static implicit operator Purchasable(string rhs)
		{
			return new Purchasable(rhs);
		}
	}

	[Serializable]
	public class Product
	{
		public string identifier = string.Empty;

		public string name = string.Empty;

		public int priceInCents;

		public string getIdentifier()
		{
			return identifier;
		}

		public string getName()
		{
			return name;
		}

		public int getPriceInCents()
		{
			return priceInCents;
		}

		public void setIdentifier(string identifier)
		{
			this.identifier = identifier;
		}

		public void setName(string name)
		{
			this.name = name;
		}

		public void setPriceInCents(int priceInCents)
		{
			this.priceInCents = priceInCents;
		}
	}

	[Serializable]
	public class Receipt
	{
		public string identifier = string.Empty;

		public int priceInCents;

		public DateTime purchaseDate = DateTime.MinValue;

		public DateTime generatedDate = DateTime.MinValue;

		public string gamer = string.Empty;

		public string uuid = string.Empty;

		public string getIdentifier()
		{
			return identifier;
		}

		public int getPriceInCents()
		{
			return priceInCents;
		}

		public DateTime getPurchaseDate()
		{
			return purchaseDate;
		}

		public DateTime getGeneratedDate()
		{
			return generatedDate;
		}

		public void setIdentifier(string identifier)
		{
			this.identifier = identifier;
		}

		public void setPriceInCents(int priceInCents)
		{
			this.priceInCents = priceInCents;
		}

		public void setPurchaseDate(DateTime date)
		{
			purchaseDate = date;
		}

		public void setGeneratedDate(DateTime date)
		{
			generatedDate = date;
		}
	}

	public interface IJoystickCalibrationListener
	{
		void OuyaOnJoystickCalibration();
	}

	public interface IMenuButtonUpListener
	{
		void OuyaMenuButtonUp();
	}

	public interface IMenuAppearingListener
	{
		void OuyaMenuAppearing();
	}

	public interface IPauseListener
	{
		void OuyaOnPause();
	}

	public interface IResumeListener
	{
		void OuyaOnResume();
	}

	public interface IFetchGamerUUIDListener
	{
		void OuyaFetchGamerUUIDOnSuccess(string gamerUUID);

		void OuyaFetchGamerUUIDOnFailure(int errorCode, string errorMessage);

		void OuyaFetchGamerUUIDOnCancel();
	}

	public interface IGetProductsListener
	{
		void OuyaGetProductsOnSuccess(List<Product> products);

		void OuyaGetProductsOnFailure(int errorCode, string errorMessage);

		void OuyaGetProductsOnCancel();
	}

	public interface IPurchaseListener
	{
		void OuyaPurchaseOnSuccess(Product product);

		void OuyaPurchaseOnFailure(int errorCode, string errorMessage);

		void OuyaPurchaseOnCancel();
	}

	public interface IGetReceiptsListener
	{
		void OuyaGetReceiptsOnSuccess(List<Receipt> receipts);

		void OuyaGetReceiptsOnFailure(int errorCode, string errorMessage);

		void OuyaGetReceiptsOnCancel();
	}

	public class OuyaJava
	{
		private const string JAVA_CLASS = "tv.ouya.sdk.OuyaUnityPlugin";

		public static void JavaInit()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, "currentActivity", "Landroid/app/Activity;");
			IntPtr staticObjectField = AndroidJNI.GetStaticObjectField(clazz, staticFieldID);
			Debug.Log("obj_Activity = " + staticObjectField);
			IntPtr clazz2 = AndroidJNI.FindClass("tv/ouya/sdk/OuyaUnityPlugin");
			IntPtr methodID = AndroidJNI.GetMethodID(clazz2, "<init>", "(Landroid/app/Activity;)V");
			IntPtr intPtr = AndroidJNI.NewObject(clazz2, methodID, new jvalue[1]
			{
				new jvalue
				{
					l = staticObjectField
				}
			});
			Debug.Log("JavaClass object = " + intPtr);
		}

		public static void JavaSetDeveloperId()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log("JavaSetDeveloperId");
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic<string>("setDeveloperId", new object[1] { m_developerId + "\0" });
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaSetDeveloperId exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaUnityInitialized()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log("JavaUnityInitialized");
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("unityInitialized");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaUnityInitialized exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaSetResolution(string resolutionId)
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log("JavaSetResolution");
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("setResolution", resolutionId);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaSetResolution exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaFetchGamerUUID()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log(string.Format("{0} OuyaSDK.JavaFetchGamerUUID", DateTime.Now));
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("fetchGamerUUID");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaFetchGamerUUID exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaAddGetProduct(Purchasable purchasable)
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log(string.Format("{0} OuyaSDK.JavaAddGetProduct", DateTime.Now));
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("addGetProduct", purchasable.getProductId());
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaAddGetProduct exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaDebugGetProductList()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log(string.Format("{0} OuyaSDK.JavaDebugGetProductList", DateTime.Now));
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("debugGetProductList");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaDebugGetProductList exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaClearGetProductList()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log(string.Format("{0} OuyaSDK.JavaClearGetProductList", DateTime.Now));
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("clearGetProductList");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaClearGetProductList exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaGetProductsAsync()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log("OuyaSDK.JavaGetProductsAsync");
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("getProductsAsync");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaGetProductsAsync exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaRequestPurchaseAsync(Purchasable purchasable)
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log(string.Format("JavaRequestPurchaseAsync purchasable: {0}", purchasable.getProductId()));
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic<string>("requestPurchaseAsync", new object[1] { purchasable.getProductId() + "\0" });
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaRequestPurchaseAsync exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public static void JavaGetReceiptsAsync()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				Debug.Log("OuyaSDK.JavaGetReceiptsAsync");
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.ouya.sdk.OuyaUnityPlugin"))
				{
					androidJavaClass.CallStatic("getReceiptsAsync");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("OuyaSDK.JavaGetReceiptsAsync exception={0}", arg));
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}
	}

	public const string VERSION = "1.0.7.3";

	public const int KEYCODE_BUTTON_A = 96;

	public const int KEYCODE_BUTTON_B = 97;

	public const int KEYCODE_BUTTON_X = 99;

	public const int KEYCODE_BUTTON_Y = 100;

	public const int KEYCODE_BUTTON_L1 = 102;

	public const int KEYCODE_BUTTON_L2 = 104;

	public const int KEYCODE_BUTTON_R1 = 103;

	public const int KEYCODE_BUTTON_R2 = 105;

	public const int KEYCODE_BUTTON_L3 = 106;

	public const int KEYCODE_BUTTON_R3 = 107;

	public const int KEYCODE_BUTTON_SYSTEM = 108;

	public const int AXIS_X = 0;

	public const int AXIS_Y = 1;

	public const int AXIS_Z = 11;

	public const int AXIS_RZ = 14;

	public const int KEYCODE_DPAD_UP = 19;

	public const int KEYCODE_DPAD_DOWN = 20;

	public const int KEYCODE_DPAD_LEFT = 21;

	public const int KEYCODE_DPAD_RIGHT = 22;

	public const int KEYCODE_DPAD_CENTER = 23;

	public const int KEYCODE_BUTTON_SELECT = -1;

	public const int KEYCODE_BUTTON_START = -2;

	public const int KEYCODE_BUTTON_ESCAPE = 41;

	public const int BUTTON_O = 96;

	public const int BUTTON_U = 99;

	public const int BUTTON_Y = 100;

	public const int BUTTON_A = 97;

	public const int BUTTON_LB = 102;

	public const int BUTTON_LT = 104;

	public const int BUTTON_RB = 103;

	public const int BUTTON_RT = 105;

	public const int BUTTON_L3 = 106;

	public const int BUTTON_R3 = 107;

	public const int BUTTON_SYSTEM = 108;

	public const int BUTTON_START = -2;

	public const int BUTTON_SELECT = -1;

	public const int BUTTON_ESCAPE = 41;

	public const int AXIS_LSTICK_X = 0;

	public const int AXIS_LSTICK_Y = 1;

	public const int AXIS_RSTICK_X = 11;

	public const int AXIS_RSTICK_Y = 14;

	public const int BUTTON_DPAD_UP = 19;

	public const int BUTTON_DPAD_RIGHT = 22;

	public const int BUTTON_DPAD_DOWN = 20;

	public const int BUTTON_DPAD_LEFT = 21;

	public const int BUTTON_DPAD_CENTER = 23;

	public static string[] Joysticks;

	private static DateTime m_timerJoysticks;

	private static string m_developerId;

	private static bool m_iapInitComplete;

	private static List<IOuyaController> m_supportedControllers;

	private static List<IJoystickCalibrationListener> m_joystickCalibrationListeners;

	private static List<IMenuButtonUpListener> m_menuButtonUpListeners;

	private static List<IMenuAppearingListener> m_menuAppearingListeners;

	private static List<IPauseListener> m_pauseListeners;

	private static List<IResumeListener> m_resumeListeners;

	private static List<IFetchGamerUUIDListener> m_fetchGamerUUIDListeners;

	private static List<IGetProductsListener> m_getProductsListeners;

	private static List<IPurchaseListener> m_purchaseListeners;

	private static List<IGetReceiptsListener> m_getReceiptsListeners;

	static OuyaSDK()
	{
		Joysticks = null;
		m_timerJoysticks = DateTime.MinValue;
		m_developerId = string.Empty;
		m_iapInitComplete = false;
		m_supportedControllers = new List<IOuyaController>();
		m_joystickCalibrationListeners = new List<IJoystickCalibrationListener>();
		m_menuButtonUpListeners = new List<IMenuButtonUpListener>();
		m_menuAppearingListeners = new List<IMenuAppearingListener>();
		m_pauseListeners = new List<IPauseListener>();
		m_resumeListeners = new List<IResumeListener>();
		m_fetchGamerUUIDListeners = new List<IFetchGamerUUIDListener>();
		m_getProductsListeners = new List<IGetProductsListener>();
		m_purchaseListeners = new List<IPurchaseListener>();
		m_getReceiptsListeners = new List<IGetReceiptsListener>();
		try
		{
			Assembly assembly = Assembly.GetAssembly(typeof(IOuyaController));
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type != typeof(IOuyaController) && typeof(IOuyaController).IsAssignableFrom(type))
				{
					IOuyaController ouyaController = (IOuyaController)Activator.CreateInstance(type);
					if (ouyaController != null)
					{
						m_supportedControllers.Add(ouyaController);
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static void UpdateJoysticks()
	{
		if (!(m_timerJoysticks < DateTime.Now))
		{
			return;
		}
		m_timerJoysticks = DateTime.Now + TimeSpan.FromSeconds(3.0);
		string[] joystickNames = Input.GetJoystickNames();
		bool flag = false;
		if (Joysticks == null)
		{
			flag = true;
		}
		else if (joystickNames.Length != Joysticks.Length)
		{
			flag = true;
		}
		else
		{
			for (int i = 0; i < joystickNames.Length; i++)
			{
				if (joystickNames[i] != Joysticks[i])
				{
					flag = true;
					break;
				}
			}
		}
		Joysticks = joystickNames;
		if (!flag)
		{
			return;
		}
		foreach (IJoystickCalibrationListener joystickCalibrationListener in getJoystickCalibrationListeners())
		{
			joystickCalibrationListener.OuyaOnJoystickCalibration();
		}
	}

	public static bool IsOUYA()
	{
		if (SystemInfo.deviceModel.ToUpper().Contains("OUYA"))
		{
			return true;
		}
		return false;
	}

	public static IOuyaController GetSupportedController(string controllerName)
	{
		foreach (IOuyaController supportedController in m_supportedControllers)
		{
			string[] supportedJoysicks = supportedController.GetSupportedJoysicks();
			foreach (string text in supportedJoysicks)
			{
				if (controllerName.ToUpper().Contains(text.ToUpper()))
				{
					return supportedController;
				}
			}
		}
		return null;
	}

	public static IOuyaController GetSupportedController(OuyaPlayer player)
	{
		if (Joysticks == null)
		{
			return null;
		}
		int num = (int)(player - 1);
		if (num >= Joysticks.Length)
		{
			return null;
		}
		string text = Joysticks[num];
		if (text == null)
		{
			return null;
		}
		return GetSupportedController(text);
	}

	public static KeyEnum[] GetSupportedAxises(OuyaPlayer player)
	{
		IOuyaController supportedController = GetSupportedController(player);
		if (supportedController == null)
		{
			return null;
		}
		return supportedController.GetSupportedAxises();
	}

	public static KeyEnum[] GetSupportedButtons(OuyaPlayer player)
	{
		IOuyaController supportedController = GetSupportedController(player);
		if (supportedController == null)
		{
			return null;
		}
		return supportedController.GetSupportedButtons();
	}

	public static bool HasAxis(KeyEnum keyCode, OuyaPlayer player)
	{
		IOuyaController supportedController = GetSupportedController(player);
		if (supportedController == null)
		{
			return false;
		}
		return supportedController.HasAxis(keyCode);
	}

	public static bool HasButton(KeyEnum keyCode, OuyaPlayer player)
	{
		IOuyaController supportedController = GetSupportedController(player);
		if (supportedController == null)
		{
			return false;
		}
		return supportedController.HasButton(keyCode);
	}

	public static bool GetAxisInverted(KeyEnum keyCode, OuyaPlayer player)
	{
		IOuyaController supportedController = GetSupportedController(player);
		if (supportedController == null)
		{
			return false;
		}
		return supportedController.GetAxisInverted(keyCode);
	}

	public static string GetUnityAxisName(KeyEnum keyCode, OuyaPlayer player)
	{
		IOuyaController supportedController = GetSupportedController(player);
		if (supportedController == null)
		{
			return string.Empty;
		}
		return supportedController.GetUnityAxisName(keyCode, player);
	}

	public static KeyCode GetUnityKeyCode(KeyEnum keyCode, OuyaPlayer player)
	{
		IOuyaController supportedController = GetSupportedController(player);
		if (supportedController == null)
		{
			return (KeyCode)(-1);
		}
		return supportedController.GetUnityKeyCode(keyCode, player);
	}

	public static void initialize(string developerId)
	{
		m_developerId = developerId;
		OuyaJava.JavaSetDeveloperId();
		OuyaJava.JavaUnityInitialized();
	}

	public static string getDeveloperId()
	{
		return m_developerId;
	}

	public static bool isIAPInitComplete()
	{
		return m_iapInitComplete;
	}

	public static void setIAPInitComplete()
	{
		m_iapInitComplete = true;
	}

	public static void fetchGamerUUID()
	{
		OuyaJava.JavaFetchGamerUUID();
	}

	public static void requestProductList(List<Purchasable> purchasables)
	{
		foreach (Purchasable purchasable in purchasables)
		{
			OuyaJava.JavaAddGetProduct(purchasable);
		}
		OuyaJava.JavaGetProductsAsync();
	}

	public static void requestPurchase(Purchasable purchasable)
	{
		OuyaJava.JavaRequestPurchaseAsync(purchasable);
	}

	public static void requestReceiptList()
	{
		OuyaJava.JavaGetReceiptsAsync();
	}

	public static List<IJoystickCalibrationListener> getJoystickCalibrationListeners()
	{
		return m_joystickCalibrationListeners;
	}

	public static void registerJoystickCalibrationListener(IJoystickCalibrationListener listener)
	{
		if (!m_joystickCalibrationListeners.Contains(listener))
		{
			m_joystickCalibrationListeners.Add(listener);
		}
	}

	public static void unregisterJoystickCalibrationListener(IJoystickCalibrationListener listener)
	{
		if (m_joystickCalibrationListeners.Contains(listener))
		{
			m_joystickCalibrationListeners.Remove(listener);
		}
	}

	public static List<IMenuButtonUpListener> getMenuButtonUpListeners()
	{
		return m_menuButtonUpListeners;
	}

	public static void registerMenuButtonUpListener(IMenuButtonUpListener listener)
	{
		if (!m_menuButtonUpListeners.Contains(listener))
		{
			m_menuButtonUpListeners.Add(listener);
		}
	}

	public static void unregisterMenuButtonUpListener(IMenuButtonUpListener listener)
	{
		if (m_menuButtonUpListeners.Contains(listener))
		{
			m_menuButtonUpListeners.Remove(listener);
		}
	}

	public static List<IMenuAppearingListener> getMenuAppearingListeners()
	{
		return m_menuAppearingListeners;
	}

	public static void registerMenuAppearingListener(IMenuAppearingListener listener)
	{
		if (!m_menuAppearingListeners.Contains(listener))
		{
			m_menuAppearingListeners.Add(listener);
		}
	}

	public static void unregisterMenuAppearingListener(IMenuAppearingListener listener)
	{
		if (m_menuAppearingListeners.Contains(listener))
		{
			m_menuAppearingListeners.Remove(listener);
		}
	}

	public static List<IPauseListener> getPauseListeners()
	{
		return m_pauseListeners;
	}

	public static void registerPauseListener(IPauseListener listener)
	{
		if (!m_pauseListeners.Contains(listener))
		{
			m_pauseListeners.Add(listener);
		}
	}

	public static void unregisterPauseListener(IPauseListener listener)
	{
		if (m_pauseListeners.Contains(listener))
		{
			m_pauseListeners.Remove(listener);
		}
	}

	public static List<IResumeListener> getResumeListeners()
	{
		return m_resumeListeners;
	}

	public static void registerResumeListener(IResumeListener listener)
	{
		if (!m_resumeListeners.Contains(listener))
		{
			m_resumeListeners.Add(listener);
		}
	}

	public static void unregisterResumeListener(IResumeListener listener)
	{
		if (m_resumeListeners.Contains(listener))
		{
			m_resumeListeners.Remove(listener);
		}
	}

	public static List<IFetchGamerUUIDListener> getFetchGamerUUIDListeners()
	{
		return m_fetchGamerUUIDListeners;
	}

	public static void registerFetchGamerUUIDListener(IFetchGamerUUIDListener listener)
	{
		if (!m_fetchGamerUUIDListeners.Contains(listener))
		{
			m_fetchGamerUUIDListeners.Add(listener);
		}
	}

	public static void unregisterFetchGamerUUIDListener(IFetchGamerUUIDListener listener)
	{
		if (m_fetchGamerUUIDListeners.Contains(listener))
		{
			m_fetchGamerUUIDListeners.Remove(listener);
		}
	}

	public static List<IGetProductsListener> getGetProductsListeners()
	{
		return m_getProductsListeners;
	}

	public static void registerGetProductsListener(IGetProductsListener listener)
	{
		if (!m_getProductsListeners.Contains(listener))
		{
			m_getProductsListeners.Add(listener);
		}
	}

	public static void unregisterGetProductsListener(IGetProductsListener listener)
	{
		if (m_getProductsListeners.Contains(listener))
		{
			m_getProductsListeners.Remove(listener);
		}
	}

	public static List<IPurchaseListener> getPurchaseListeners()
	{
		return m_purchaseListeners;
	}

	public static void registerPurchaseListener(IPurchaseListener listener)
	{
		if (!m_purchaseListeners.Contains(listener))
		{
			m_purchaseListeners.Add(listener);
		}
	}

	public static void unregisterPurchaseListener(IPurchaseListener listener)
	{
		if (m_purchaseListeners.Contains(listener))
		{
			m_purchaseListeners.Remove(listener);
		}
	}

	public static List<IGetReceiptsListener> getGetReceiptsListeners()
	{
		return m_getReceiptsListeners;
	}

	public static void registerGetReceiptsListener(IGetReceiptsListener listener)
	{
		if (!m_getReceiptsListeners.Contains(listener))
		{
			m_getReceiptsListeners.Add(listener);
		}
	}

	public static void unregisterGetReceiptsListener(IGetReceiptsListener listener)
	{
		if (m_getReceiptsListeners.Contains(listener))
		{
			m_getReceiptsListeners.Remove(listener);
		}
	}
}
