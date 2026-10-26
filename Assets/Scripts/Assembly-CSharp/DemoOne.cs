using UnityEngine;

public class DemoOne : MonoBehaviour
{
	public Transform cube;

	private Vector2 _scrollPosition;

	private void OnGUI()
	{
		GUI.skin.button.padding = new RectOffset(10, 10, 20, 20);
		GUI.skin.button.fixedWidth = 250f;
		GUILayout.BeginArea(new Rect((float)Screen.width - GUI.skin.button.fixedWidth - 20f, 0f, GUI.skin.button.fixedWidth + 20f, Screen.height));
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(GUI.skin.button.fixedWidth + 20f), GUILayout.Height(Screen.height));
		if (GUILayout.Button("Add Tap Recognizer"))
		{
			TKTapRecognizer tKTapRecognizer = new TKTapRecognizer();
			tKTapRecognizer.boundaryFrame = new TKRect(0f, 0f, 50f, 50f);
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				tKTapRecognizer.numberOfTouchesRequired = 2;
			}
			tKTapRecognizer.gestureRecognizedEvent += delegate(TKTapRecognizer r)
			{
				Debug.Log("tap recognizer fired: " + r);
			};
			TouchKit.addGestureRecognizer(tKTapRecognizer);
		}
		if (GUILayout.Button("Add Long Press Recognizer"))
		{
			TKLongPressRecognizer tKLongPressRecognizer = new TKLongPressRecognizer();
			tKLongPressRecognizer.gestureRecognizedEvent += delegate(TKLongPressRecognizer r)
			{
				Debug.Log("long press recognizer fired: " + r);
			};
			tKLongPressRecognizer.gestureCompleteEvent += delegate(TKLongPressRecognizer r)
			{
				Debug.Log("long press recognizer finished: " + r);
			};
			TouchKit.addGestureRecognizer(tKLongPressRecognizer);
		}
		if (GUILayout.Button("Add Pan Recognizer"))
		{
			TKPanRecognizer recognizer4 = new TKPanRecognizer();
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				recognizer4.minimumNumberOfTouches = 2;
			}
			recognizer4.gestureRecognizedEvent += delegate(TKPanRecognizer r)
			{
				Camera.main.transform.position -= new Vector3(recognizer4.deltaTranslation.x, recognizer4.deltaTranslation.y) / 100f;
				Debug.Log("pan recognizer fired: " + r);
			};
			recognizer4.gestureCompleteEvent += delegate
			{
				Debug.Log("pan gesture complete");
			};
			TouchKit.addGestureRecognizer(recognizer4);
		}
		if (GUILayout.Button("Add Horizontal Swipe Recognizer"))
		{
			TKSwipeRecognizer tKSwipeRecognizer = new TKSwipeRecognizer(TKSwipeDirection.Horizontal);
			tKSwipeRecognizer.gestureRecognizedEvent += delegate(TKSwipeRecognizer r)
			{
				Debug.Log("swipe recognizer fired: " + r);
			};
			TouchKit.addGestureRecognizer(tKSwipeRecognizer);
		}
		if (GUILayout.Button("Add Pinch Recognizer"))
		{
			TKPinchRecognizer recognizer3 = new TKPinchRecognizer();
			recognizer3.gestureRecognizedEvent += delegate(TKPinchRecognizer r)
			{
				cube.transform.localScale += Vector3.one * recognizer3.deltaScale;
				Debug.Log("pinch recognizer fired: " + r);
			};
			TouchKit.addGestureRecognizer(recognizer3);
		}
		if (GUILayout.Button("Add Rotation Recognizer"))
		{
			TKRotationRecognizer recognizer2 = new TKRotationRecognizer();
			recognizer2.gestureRecognizedEvent += delegate(TKRotationRecognizer r)
			{
				cube.Rotate(Vector3.back, recognizer2.deltaRotation);
				Debug.Log("rotation recognizer fired: " + r);
			};
			TouchKit.addGestureRecognizer(recognizer2);
		}
		if (GUILayout.Button("Add Button Recognizer"))
		{
			TKButtonRecognizer tKButtonRecognizer = new TKButtonRecognizer(new TKRect(5f, 145f, 80f, 30f), 10f);
			tKButtonRecognizer.zIndex = 1u;
			tKButtonRecognizer.onSelectedEvent += delegate(TKButtonRecognizer r)
			{
				Debug.Log("button recognizer selected: " + r);
			};
			tKButtonRecognizer.onDeselectedEvent += delegate(TKButtonRecognizer r)
			{
				Debug.Log("button recognizer deselected: " + r);
			};
			tKButtonRecognizer.onTouchUpInsideEvent += delegate(TKButtonRecognizer r)
			{
				Debug.Log("button recognizer touch up inside: " + r);
			};
			TouchKit.addGestureRecognizer(tKButtonRecognizer);
		}
		if (GUILayout.Button("Add One Finger Rotation Recognizer"))
		{
			TKOneFingerRotationRecognizer recognizer = new TKOneFingerRotationRecognizer();
			recognizer.targetPosition = Camera.main.WorldToScreenPoint(cube.position);
			recognizer.gestureRecognizedEvent += delegate(TKOneFingerRotationRecognizer r)
			{
				cube.Rotate(Vector3.back, recognizer.deltaRotation);
				Debug.Log("one finger rotation recognizer fired: " + r);
			};
			TouchKit.addGestureRecognizer(recognizer);
		}
		if (GUILayout.Button("Add Any Touch Recognizer"))
		{
			TKAnyTouchRecognizer tKAnyTouchRecognizer = new TKAnyTouchRecognizer(new TKRect(10f, 10f, 80f, 50f));
			tKAnyTouchRecognizer.zIndex = 1u;
			tKAnyTouchRecognizer.onEnteredEvent += delegate(TKAnyTouchRecognizer r)
			{
				Debug.Log("any touch entered: " + r);
			};
			tKAnyTouchRecognizer.onExitedEvent += delegate(TKAnyTouchRecognizer r)
			{
				Debug.Log("any touch exited: " + r);
			};
			TouchKit.addGestureRecognizer(tKAnyTouchRecognizer);
		}
		if (GUILayout.Button("Remove All Recognizers"))
		{
			TouchKit.removeAllGestureRecognizers();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
