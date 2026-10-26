using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Checks KFSceneLightmaps' load-time static batching on a level: renders it with the Unity 4 lightmaps applied, then
// again after batching its m_staticBatch, from the scene camera and from above the level, and compares the pictures
// (batching must not change how anything looks, lightmap UVs included). Also reports Unity's draw call counts.
// Writes Logs/batch_verify.txt and Logs/batch_<view>_<before|after>.png. The scene is not saved.
// Batch (not -nographics): -executeMethod KFStaticBatchVerify.Run [-scene Assets/Scenes/asylum_scene.unity]
public static class KFStaticBatchVerify
{
	private const int Width = 480;

	private const int Height = 272;

	public static void Run()
	{
		string[] args = System.Environment.GetCommandLineArgs();
		int a = System.Array.IndexOf(args, "-scene");
		string scenePath = a >= 0 && a + 1 < args.Length ? args[a + 1] : "Assets/Scenes/asylum_scene.unity";
		EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
		KFSceneLightmaps lightmaps = Object.FindObjectOfType<KFSceneLightmaps>();
		GameObject[] batch = lightmaps.m_staticBatch;
		lightmaps.m_staticBatch = null;
		lightmaps.Apply();
		Bounds bounds = new Bounds();
		bool first = true;
		foreach (Renderer r in Object.FindObjectsOfType<Renderer>())
		{
			if (first)
			{
				bounds = r.bounds;
				first = false;
			}
			else
			{
				bounds.Encapsulate(r.bounds);
			}
		}
		Camera camera = new GameObject("KFVerifyCamera").AddComponent<Camera>();
		camera.farClipPlane = 2000f;
		Camera sceneCamera = null;
		foreach (Camera c in Object.FindObjectsOfType<Camera>())
		{
			if (c != camera && c.enabled && c.gameObject.activeInHierarchy && sceneCamera == null)
			{
				sceneCamera = c;
			}
		}
		Pose[] views =
		{
			sceneCamera != null ? new Pose(sceneCamera.transform.position, sceneCamera.transform.rotation) : new Pose(bounds.center + Vector3.up * 10f, Quaternion.Euler(60f, 0f, 0f)),
			new Pose(bounds.center + Vector3.up * bounds.extents.magnitude, Quaternion.Euler(90f, 0f, 0f)),
			new Pose(bounds.center + new Vector3(0f, 3f, 0f), Quaternion.Euler(20f, 45f, 0f))
		};
		Texture2D[] before = new Texture2D[views.Length];
		int[] drawsBefore = new int[views.Length];
		for (int i = 0; i < views.Length; i++)
		{
			before[i] = Render(camera, views[i], out drawsBefore[i]);
		}
		float start = Time.realtimeSinceStartup;
		StaticBatchingUtility.Combine(batch, lightmaps.gameObject);
		float combineMs = (Time.realtimeSinceStartup - start) * 1000f;
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		int inBatch = 0;
		System.Collections.Generic.HashSet<Mesh> combined = new System.Collections.Generic.HashSet<Mesh>();
		foreach (GameObject go in batch)
		{
			Renderer r = go.GetComponent<Renderer>();
			if (r != null && r.isPartOfStaticBatch)
			{
				inBatch++;
				combined.Add(go.GetComponent<MeshFilter>().sharedMesh);
			}
		}
		sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1} objects given, {2} now part of a static batch ({3} combined meshes), {4:0} ms (editor)\n", scenePath, batch.Length, inBatch, combined.Count, combineMs);
		Directory.CreateDirectory("Logs");
		for (int i = 0; i < views.Length; i++)
		{
			int drawsAfter;
			Texture2D after = Render(camera, views[i], out drawsAfter);
			Color32[] p = before[i].GetPixels32(), q = after.GetPixels32();
			long diff = 0;
			int changed = 0;
			for (int k = 0; k < p.Length; k++)
			{
				int d = Mathf.Abs(p[k].r - q[k].r) + Mathf.Abs(p[k].g - q[k].g) + Mathf.Abs(p[k].b - q[k].b);
				diff += d;
				if (d > 24)
				{
					changed++;
				}
			}
			sb.AppendFormat(CultureInfo.InvariantCulture, "view {0}: draw calls {1} -> {2}, mean difference {3:0.00}/255 per channel, pixels changed {4:0.00}%\n",
				i, drawsBefore[i], drawsAfter, diff / (3.0 * p.Length), 100.0 * changed / p.Length);
			File.WriteAllBytes("Logs/batch_view" + i + "_before.png", before[i].EncodeToPNG());
			File.WriteAllBytes("Logs/batch_view" + i + "_after.png", after.EncodeToPNG());
		}
		File.WriteAllText("Logs/batch_verify.txt", sb.ToString());
		Debug.Log("[KFBatchVerify] " + sb);
	}

	private static Texture2D Render(Camera camera, Pose view, out int draws)
	{
		camera.transform.SetPositionAndRotation(view.position, view.rotation);
		RenderTexture rt = new RenderTexture(Width, Height, 24);
		camera.targetTexture = rt;
		UnityStats.batches.ToString();
		camera.Render();
		draws = UnityStats.drawCalls;
		RenderTexture.active = rt;
		Texture2D shot = new Texture2D(Width, Height, TextureFormat.RGB24, false);
		shot.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
		RenderTexture.active = null;
		camera.targetTexture = null;
		rt.Release();
		return shot;
	}
}
