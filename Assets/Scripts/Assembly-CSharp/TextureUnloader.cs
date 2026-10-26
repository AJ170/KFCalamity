using UnityEngine;

public class TextureUnloader : MonoBehaviour
{
	public bool UnloadNow;

	private void Awake()
	{
	}

	private void Update()
	{
		if (UnloadNow)
		{
			Unload();
		}
	}

	private void Unload()
	{
		Resources.UnloadUnusedAssets();
		UnloadNow = false;
	}
}
