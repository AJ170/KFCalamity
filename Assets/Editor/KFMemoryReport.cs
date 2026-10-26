using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Estimates the PS Vita memory of what each build scene and each Resources folder pulls in, from the PS Vita import
// settings (textures: format and size of the PSP2 override; meshes: vertex and index data, twice when readable since a
// readable mesh keeps a CPU copy). Third Vita build ran out of memory loading the tutorial: video memory (104 MB) was
// full and 140 MB of textures and meshes had spilled into main memory. Writes Logs/memory_report.txt.
// Batch: -executeMethod KFMemoryReport.Run
public static class KFMemoryReport
{
	private const string Platform = "PSP2";

	private class Item
	{
		public string Path;
		public long Bytes;
		public string Info;
	}

	[MenuItem("KF/Vita memory report")]
	public static void Run()
	{
		Dictionary<string, Item> cache = new Dictionary<string, Item>();
		StringBuilder report = new StringBuilder();
		List<string> roots = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToList();
		foreach (string dir in Directory.GetDirectories("Assets/Resources").Select(d => d.Replace('\\', '/')))
		{
			roots.Add(dir);
		}
		roots.Add("Assets/Resources (files at top level)");
		foreach (string root in roots)
		{
			string[] seeds;
			if (root.EndsWith(".unity"))
			{
				seeds = new[] { root };
			}
			else if (root.StartsWith("Assets/Resources ("))
			{
				seeds = Directory.GetFiles("Assets/Resources").Where(f => !f.EndsWith(".meta")).Select(f => f.Replace('\\', '/')).ToArray();
			}
			else
			{
				seeds = AssetDatabase.FindAssets("", new[] { root }).Select(AssetDatabase.GUIDToAssetPath).Where(p => !AssetDatabase.IsValidFolder(p)).ToArray();
			}
			List<Item> items = AssetDatabase.GetDependencies(seeds, true).Distinct().Select(p => Measure(p, cache)).Where(i => i != null && i.Bytes > 0).OrderByDescending(i => i.Bytes).ToList();
			long textures = items.Where(i => i.Info.StartsWith("tex")).Sum(i => i.Bytes);
			long meshes = items.Where(i => i.Info.StartsWith("mesh")).Sum(i => i.Bytes);
			report.AppendFormat("== {0}: textures {1:0.0} MB, meshes {2:0.0} MB\n", root, textures / 1048576f, meshes / 1048576f);
			foreach (Item i in items.Take(25))
			{
				report.AppendFormat("  {0,7:0.00} MB  {1,-40} {2}\n", i.Bytes / 1048576f, i.Info, i.Path);
			}
		}
		Directory.CreateDirectory("Logs");
		File.WriteAllText("Logs/memory_report.txt", report.ToString());
		Debug.Log("[KFMEM] wrote Logs/memory_report.txt");
	}

	private static Item Measure(string path, Dictionary<string, Item> cache)
	{
		Item item;
		if (cache.TryGetValue(path, out item))
		{
			return item;
		}
		item = null;
		AssetImporter importer = AssetImporter.GetAtPath(path);
		TextureImporter ti = importer as TextureImporter;
		if (ti != null)
		{
			Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
			if (tex != null)
			{
				TextureImporterPlatformSettings vita = ti.GetPlatformTextureSettings(Platform);
				TextureImporterFormat format = vita.overridden ? vita.format : TextureImporterFormat.Automatic;
				int max = vita.overridden ? vita.maxTextureSize : ti.maxTextureSize;
				int w = tex.width, h = tex.height;
				while (w > max || h > max)
				{
					w = Mathf.Max(1, w / 2);
					h = Mathf.Max(1, h / 2);
				}
				float bpp = BytesPerPixel(format, ti.textureType == TextureImporterType.Lightmap);
				long bytes = (long)(w * h * bpp * (ti.mipmapEnabled ? 4f / 3f : 1f));
				item = new Item { Path = path, Bytes = bytes, Info = string.Format("tex {0}x{1} {2}{3}", w, h, ti.textureType == TextureImporterType.Lightmap ? "lightmap" : format.ToString(), ti.mipmapEnabled ? " mip" : "") };
			}
		}
		else if (path.EndsWith(".asset") || importer is ModelImporter)
		{
			long bytes = 0;
			int count = 0;
			bool readable = false;
			foreach (Mesh mesh in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Mesh>())
			{
				int stride = 12 + (mesh.normals.Length > 0 ? 12 : 0) + (mesh.tangents.Length > 0 ? 16 : 0) + (mesh.colors32.Length > 0 ? 4 : 0) +
					(mesh.uv.Length > 0 ? 8 : 0) + (mesh.uv2.Length > 0 ? 8 : 0) + (mesh.boneWeights.Length > 0 ? 32 : 0);
				long indices = 0;
				for (int s = 0; s < mesh.subMeshCount; s++)
				{
					indices += mesh.GetIndexCount(s);
				}
				long one = (long)mesh.vertexCount * stride + indices * 2;
				readable |= mesh.isReadable;
				bytes += mesh.isReadable ? one * 2 : one;
				count++;
			}
			if (count > 0)
			{
				item = new Item { Path = path, Bytes = bytes, Info = "mesh" + (readable ? " readable" : "") };
			}
		}
		cache[path] = item;
		return item;
	}

	private static float BytesPerPixel(TextureImporterFormat format, bool lightmap)
	{
		if (lightmap)
		{
			return 1f; // RGBM in DXT5
		}
		switch (format)
		{
		case TextureImporterFormat.DXT1:
		case TextureImporterFormat.DXT1Crunched:
		case TextureImporterFormat.PVRTC_RGB4:
		case TextureImporterFormat.PVRTC_RGBA4:
		case TextureImporterFormat.ETC_RGB4:
			return 0.5f;
		case TextureImporterFormat.DXT5:
		case TextureImporterFormat.DXT5Crunched:
		case TextureImporterFormat.Alpha8:
			return 1f;
		case TextureImporterFormat.RGB16:
		case TextureImporterFormat.RGBA16:
		case TextureImporterFormat.ARGB16:
			return 2f;
		case TextureImporterFormat.RGB24:
			return 3f;
		default:
			return 4f;
		}
	}
}
