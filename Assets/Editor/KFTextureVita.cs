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
// Textures without mipmaps (UI atlases, loading screens) 1536 or more across are imported at half size: they were made
// for 1920 x 1080 phones, so half size is still one texel a Vita pixel, and at full size the menu alone needed 134 MB
// (third Vita build overflowed video memory loading the tutorial). NGUI divides its pixel rectangles by the texture
// size, so their original sizes are written to Assets/Scripts/Port/KFUISourceSize.Entries.cs for KFUISourceSize.
// Lightmaps are left alone (KFSceneLightmapsSetup). Every texture is made non-readable: the game never reads imported
// textures' pixels and a readable one keeps a second copy in main memory. Other platforms are unchanged.
// Batch: -executeMethod KFTextureVita.Run
public static class KFTextureVita
{
	private const string Platform = "PSP2";

	private const int MaxMipmappedSize = 1024;

	private const int HalveFrom = 1536;

	private const string EntriesPath = "Assets/Scripts/Port/KFUISourceSize.Entries.cs";

	private static readonly System.Reflection.MethodInfo SourceSize = typeof(TextureImporter).GetMethod("GetWidthAndHeight",
		System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

	private static readonly Regex LegacyFormat = new Regex(@"^  textureFormat: (-?\d+)", RegexOptions.Multiline);

	[MenuItem("KF/Vita texture settings")]
	public static void Run()
	{
		string[] paths = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" }).Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();
		int changed = 0;
		System.Collections.Generic.Dictionary<string, int> counts = new System.Collections.Generic.Dictionary<string, int>();
		System.Collections.Generic.List<string> halved = new System.Collections.Generic.List<string>();
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
				object[] source = { 0, 0 };
				SourceSize.Invoke(importer, source);
				int sourceWidth = (int)source[0], sourceHeight = (int)source[1];
				int size = importer.mipmapEnabled ? Mathf.Min(importer.maxTextureSize, MaxMipmappedSize) : importer.maxTextureSize;
				bool halve = !importer.mipmapEnabled && Mathf.Max(sourceWidth, sourceHeight) >= HalveFrom;
				if (halve)
				{
					size = Mathf.Min(size, Mathf.ClosestPowerOfTwo(Mathf.Max(sourceWidth, sourceHeight)) / 2);
					halved.Add(path + "|" + sourceWidth + "|" + sourceHeight);
				}
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
		WriteEntries(halved);
		Debug.Log("[KFTEX] " + paths.Length + " textures, " + changed + " changed for " + Platform + ": " +
			string.Join(", ", counts.OrderByDescending(kv => kv.Value).Select(kv => kv.Value + " " + kv.Key).ToArray()) + "; " + halved.Count + " halved");
	}

	// KFUISourceSize's table: name, PS Vita imported size, size made at
	private static void WriteEntries(System.Collections.Generic.List<string> halved)
	{
		System.Collections.Generic.List<string> lines = new System.Collections.Generic.List<string>
		{
			"// Generated by KFTextureVita (KF > Vita texture settings): UI textures imported at half size on the PS Vita.",
			"public static partial class KFUISourceSize",
			"{",
			"\tprivate static readonly Entry[] Entries =",
			"\t{"
		};
		System.Collections.Generic.HashSet<string> names = new System.Collections.Generic.HashSet<string>();
		foreach (string line in halved.OrderBy(l => l))
		{
			string[] f = line.Split('|');
			Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(f[0]);
			if (!names.Add(tex.name))
			{
				Debug.LogWarning("[KFTEX] two halved textures named " + tex.name + ": NGUI may get the wrong size for " + f[0]);
			}
			lines.Add(string.Format("\t\tnew Entry(\"{0}\", {1}, {2}, {3}, {4}), // {5}", tex.name.Replace("\"", "\\\""), tex.width, tex.height, f[1], f[2], f[0]));
		}
		lines.Add("\t};");
		lines.Add("}");
		string text = string.Join("\r\n", lines.ToArray()) + "\r\n";
		if (!File.Exists(EntriesPath) || File.ReadAllText(EntriesPath) != text)
		{
			File.WriteAllText(EntriesPath, text);
			AssetDatabase.ImportAsset(EntriesPath);
		}
	}
}
