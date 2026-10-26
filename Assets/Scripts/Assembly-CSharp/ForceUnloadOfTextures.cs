using UnityEngine;

public class ForceUnloadOfTextures : MonoBehaviour
{
	public int ByteThreshold = 1000000;

	private void Awake()
	{
		Resources.UnloadUnusedAssets();
	}
}
