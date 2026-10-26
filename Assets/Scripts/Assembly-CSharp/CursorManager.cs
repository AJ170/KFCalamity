using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
	[Serializable]
	public class CursorInfo
	{
		public CursorType Type;

		public Texture2D Texture;

		public Vector2 HotSpot;
	}

	public CursorInfo[] Cursors;

	private static CursorManager instance;

	public static CursorManager Instance
	{
		get
		{
			return instance;
		}
	}

	public static void SetCursorByType(CursorType cursorType)
	{
	}

	private void Awake()
	{
		instance = this;
	}
}
