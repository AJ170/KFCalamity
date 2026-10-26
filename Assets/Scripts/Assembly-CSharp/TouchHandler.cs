using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class TouchHandler : MonoBehaviour
{
	private const int MAX_TOUCHES = 128;

	public bool SimulateTouch;

	public static TouchHandler _instance;

	private List<InternalTouchData> mTouchData = new List<InternalTouchData>(128);

	private int mLastNumTouches;

	private bool mInitialized;

	private int mNumTouches;

	private float mCheckModeTimer;

	private bool mIsSlateMode;

	private List<short> mIDs = new List<short>();

	public static TouchHandler instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("TouchHandler");
				_instance = gameObject.AddComponent<TouchHandler>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		_instance = this;
		for (int num = 128; num >= 0; num--)
		{
			mIDs.Add((short)num);
		}
	}

	private int GetNextID()
	{
		int result = mIDs.LastItem();
		mIDs.RemoveAt(mIDs.Count - 1);
		return result;
	}

	private void RestoreID(int id)
	{
		mIDs.Add((short)id);
		mIDs.Sort((short a, short b) => -a.CompareTo(b));
	}

	public int GetNumTouches()
	{
		return Input.touchCount;
	}

	public TouchData GetTouch(int index)
	{
		Touch touch = Input.GetTouch(index);
		TouchData touchData = new TouchData();
		touchData.fingerId = touch.fingerId;
		touchData.position = touch.position;
		touchData.phase = touch.phase;
		touchData.deltaPosition = touch.deltaPosition;
		touchData.tapCount = touch.tapCount;
		return touchData;
	}
}
