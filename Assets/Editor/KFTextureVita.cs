using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

// PS Vita texture import settings (adapted from Borderlands' BOLTextureVita). The rip's .meta files keep the format
// each texture had in the Android build in the legacy top-level "textureFormat" field; Unity 2018 reads the 32-bit ones
// as "uncompressed" and the rest as automatic, so the first Vita build packed 707 MB of textures. The PS Vita override
// follows the Android build's own choice:
//   ETC (opaque, compressed)            -> DXT1 (same 4 bits a pixel)
//   RGBA4444 with mipmaps (world, models) -> DXT5, or DXT1 without alpha (half the size, less banding)
//   RGBA4444 without mipmaps (UI) / RGB565 / Alpha8 / RGB24 -> the same format
//   32-bit with mipmaps (world, models)  -> DXT5, or DXT1 without alpha
//   32-bit without mipmaps (UI atlases, splash and loading screens) -> kept 32-bit, so text and UI keep their quality
// Mipmapped textures are capped at 1024 (the screen is 960 x 544; Borderlands' 2048 textures overflowed video memory).
// Lightmaps are left alone (KFSceneLightmapsSetup). Every texture is made non-readable: the game never reads imported
// textures' pixels and a readable one keeps a second copy in main memory. Other platforms are unchanged.
// Batch: -executeMethod KFTextureVita.Run
public static class KFTextureVita
{
	private const string Platform = "PSP2";

	private const int MaxMipmappedSize = 1024;

	private static readonly Regex LegacyFormat = new Regex(@"^  textureFormat: (-?\d+)", RegexOptions.Multiline);

	[MenuItem("KF/Vita texture settings")]
	public static void Run()
	{
		string[] paths = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" }).Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();
		int changed = 0;
		System.Collections.Generic.Dictionary<string, int> counts = new System.Collections.Generic.Dictionary<string, int>();
		AssetDatabase.StartAssetEditing();
		try
		{
			foreach (string path in paths)
			{
				TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
				if (importer == null || importer.textureType == TextureImporterType.Lightmap)
				{
					continue;
				}
				Match legacy = LegacyFormat.Match(File.ReadAllText(path + ".meta"));
				int original = legacy.Success ? int.Parse(legacy.Groups[1].Value) : -1;
				bool hasAlpha = importer.DoesSourceTextureHaveAlpha();
				TextureImporterFormat format;
				switch (original)
				{
				case 34: // ETC_RGB4
				case 10: // DXT1
					format = TextureImporterFormat.DXT1;
					break;
				case 13: // RGBA4444
				case 2: // ARGB4444
					// world / model textures: DXT is half the size (8 bits a pixel) and bands less than 4444; UI keeps 16-bit
					format = importer.mipmapEnabled ? (hasAlpha ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1) : TextureImporterFormat.RGBA16;
					break;
				case 7: // RGB565
					format = TextureImporterFormat.RGB16;
					break;
				case 1:
					format = TextureImporterFormat.Alpha8;
					break;
				case 3:
					format = TextureImporterFormat.RGB24;
					break;
				case 4: // RGBA32
				case 5: // ARGB32
					format = importer.mipmapEnabled ? (hasAlpha ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1) : TextureImporterFormat.RGBA32;
					break;
				default:
					format = hasAlpha ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1;
					break;
				}
				int size = importer.mipmapEnabled ? Mathf.Min(importer.maxTextureSize, MaxMipmappedSize) : importer.maxTextureSize;
				TextureImporterPlatformSettings vita = importer.GetPlatformTextureSettings(Platform);
				string key = format + (importer.mipmapEnabled ? " (mipmapped)" : "");
				int n;
				counts.TryGetValue(key, out n);
				counts[key] = n + 1;
				if (vita.overridden && vita.format == format && vita.maxTextureSize == size && !importer.isReadable)
				{
					continue;
				}
				vita.overridden = true;
				vita.format = format;
				vita.maxTextureSize = size;
				importer.SetPlatformTextureSettings(vita);
				importer.isReadable = false;
				importer.SaveAndReimport();
				changed++;
			}
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
		}
		AssetDatabase.Refresh();
		Debug.Log("[KFTEX] " + paths.Length + " textures, " + changed + " changed for " + Platform + ": " +
			string.Join(", ", counts.OrderByDescending(kv => kv.Value).Select(kv => kv.Value + " " + kv.Key).ToArray()));
	}
}
