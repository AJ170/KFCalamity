using System.Collections.Generic;
using UnityEngine;

public class TouchKit : MonoBehaviour
{
	[HideInInspector]
	public bool simulateTouches = true;

	[HideInInspector]
	public bool simulateMultitouch = true;

	[HideInInspector]
	public bool drawTouches;

	[HideInInspector]
	public bool drawDebugBoundaryFrames;

	public bool autoScaleRectsAndDistances = true;

	public bool shouldAutoUpdateTouches = true;

	public Vector2 designTimeResolution = new Vector2(320f, 180f);

	public int maxTouchesToProcess = 2;

	private List<TKAbstractGestureRecognizer> _gestureRecognizers = new List<TKAbstractGestureRecognizer>();

	private TKTouch[] _touchCache;

	private List<TKTouch> _liveTouches = new List<TKTouch>();

	private bool _shouldCheckForLostTouches;

	private static TouchKit _instance;

	public Vector2 runtimeScaleModifier { get; private set; }

	public float runtimeDistanceModifier { get; private set; }

	public Vector2 pixelsToUnityUnitsMultiplier { get; private set; }

	public static TouchKit instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(TouchKit)) as TouchKit;
				if (!_instance)
				{
					GameObject gameObject = new GameObject("TouchKit");
					_instance = gameObject.AddComponent<TouchKit>();
					Object.DontDestroyOnLoad(gameObject);
				}
				Camera camera = Camera.main ?? Camera.allCameras[0];
				if (camera.orthographic)
				{
					Vector2 vector = new Vector2(camera.aspect * camera.orthographicSize * 2f, camera.orthographicSize * 2f);
					_instance.pixelsToUnityUnitsMultiplier = new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height);
				}
				else
				{
					_instance.pixelsToUnityUnitsMultiplier = Vector2.one;
				}
				_instance.runtimeScaleModifier = new Vector2((float)Screen.width / _instance.designTimeResolution.x, (float)Screen.height / _instance.designTimeResolution.y);
				_instance.runtimeDistanceModifier = (_instance.runtimeScaleModifier.x + _instance.runtimeScaleModifier.y) / 2f;
				if (!_instance.autoScaleRectsAndDistances)
				{
					_instance.runtimeScaleModifier = Vector2.one;
					_instance.runtimeDistanceModifier = 1f;
				}
			}
			return _instance;
		}
	}

	private void addTouchesUnityForgotToEndToLiveTouchesList()
	{
		for (int i = 0; i < _touchCache.Length; i++)
		{
			if (_touchCache[i].phase != TouchPhase.Ended)
			{
				Debug.LogWarning("found touch Unity forgot to end with phase: " + _touchCache[i].phase);
				_touchCache[i].phase = TouchPhase.Ended;
				_liveTouches.Add(_touchCache[i]);
			}
		}
	}

	private void internalUpdateTouches()
	{
		if (TouchHandler.instance.GetNumTouches() > 0)
		{
			_shouldCheckForLostTouches = true;
			int num = Mathf.Min(TouchHandler.instance.GetNumTouches(), maxTouchesToProcess);
			for (int i = 0; i < num; i++)
			{
				TouchData touch = TouchHandler.instance.GetTouch(i);
				if (touch.fingerId < maxTouchesToProcess)
				{
					_liveTouches.Add(_touchCache[touch.fingerId].populateWithTouch(touch));
				}
			}
		}
		else if (_shouldCheckForLostTouches)
		{
			addTouchesUnityForgotToEndToLiveTouchesList();
			_shouldCheckForLostTouches = false;
		}
		if (_liveTouches.Count > 0)
		{
			for (int j = 0; j < _gestureRecognizers.Count; j++)
			{
				_gestureRecognizers[j].recognizeTouches(_liveTouches);
			}
			_liveTouches.Clear();
		}
	}

	private void Awake()
	{
		_touchCache = new TKTouch[maxTouchesToProcess];
		for (int i = 0; i < maxTouchesToProcess; i++)
		{
			_touchCache[i] = new TKTouch(i);
		}
	}

	private void Update()
	{
		if (shouldAutoUpdateTouches)
		{
			internalUpdateTouches();
		}
	}

	private void OnApplicationQuit()
	{
		_instance = null;
		Object.Destroy(base.gameObject);
	}

	public static void updateTouches()
	{
		_instance.internalUpdateTouches();
	}

	public static void addGestureRecognizer(TKAbstractGestureRecognizer recognizer)
	{
		instance._gestureRecognizers.Add(recognizer);
		if (recognizer.zIndex != 0)
		{
			_instance._gestureRecognizers.Sort();
			_instance._gestureRecognizers.Reverse();
		}
	}

	public static void removeGestureRecognizer(TKAbstractGestureRecognizer recognizer)
	{
		if (!_instance._gestureRecognizers.Contains(recognizer))
		{
			Debug.LogError("Trying to remove gesture recognizer that has not been added: " + recognizer);
			return;
		}
		recognizer.reset();
		instance._gestureRecognizers.Remove(recognizer);
	}

	public static void removeAllGestureRecognizers()
	{
		instance._gestureRecognizers.Clear();
	}
}
