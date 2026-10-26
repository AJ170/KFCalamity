using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class RealtimeReflection : MonoBehaviour
{
	public int cubemapSize;

	public bool oneFacePerFrame;

	private Camera cam;

	private RenderTexture rtex;

	public RealtimeReflection()
	{
		cubemapSize = 128;
	}

	public virtual void Start()
	{
		UpdateCubemap(63);
	}

	public virtual void LateUpdate()
	{
		if (oneFacePerFrame)
		{
			int num = Time.frameCount % 6;
			int faceMask = 1 << num;
			UpdateCubemap(faceMask);
		}
		else
		{
			UpdateCubemap(63);
		}
	}

	public virtual void UpdateCubemap(int faceMask)
	{
		if (!cam)
		{
			GameObject gameObject = new GameObject("CubemapCamera", typeof(Camera));
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.transform.position = transform.position;
			gameObject.transform.rotation = Quaternion.identity;
			cam = gameObject.GetComponent<Camera>();
			cam.farClipPlane = 100f;
			cam.enabled = false;
		}
		if (!rtex)
		{
			rtex = new RenderTexture(cubemapSize, cubemapSize, 16);
			rtex.isCubemap = true;
			rtex.hideFlags = HideFlags.HideAndDontSave;
			GetComponent<Renderer>().sharedMaterial.SetTexture("_Cube", rtex);
		}
		cam.transform.position = transform.position;
		cam.RenderToCubemap(rtex, faceMask);
	}

	public virtual void OnDisable()
	{
		UnityEngine.Object.DestroyImmediate(cam);
		UnityEngine.Object.DestroyImmediate(rtex);
	}

	public virtual void Main()
	{
	}
}
