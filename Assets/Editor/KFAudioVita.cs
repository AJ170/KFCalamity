using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// PS Vita sound import settings (Borderlands' BOLAudioVita rule, set by the owner there): everything at 22050 Hz;
// music and background loops (8 s or more, or named like music / ambience) streamed from the memory card as MP3, so
// they are not held in memory (the first Vita build packed 178 MB of sound); everything else as HEVAG (the PS Vita's
// hardware-decoded ADPCM) kept compressed in memory. Other platforms are unchanged. Writes Logs/audio_vita.txt.
// Batch: -executeMethod KFAudioVita.Run
public static class KFAudioVita
{
	private const string Platform = "PSP2";

	private const int SampleRate = 22050;

	private const float StreamFromSeconds = 8f;

	// (not "menu" / "loop": menu clicks and short weapon fire loops play often and must stay in memory)
	private static readonly string[] StreamWords = { "music", "theme", "ambient", "abient", "ambience" };

	[MenuItem("KF/Vita audio settings")]
	public static void Run()
	{
		string[] paths = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets" }).Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();
		int streamed = 0, effects = 0, changed = 0;
		List<string> lines = new List<string>();
		AssetDatabase.StartAssetEditing();
		try
		{
			foreach (string path in paths)
			{
				AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
				AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
				if (importer == null || clip == null)
				{
					continue;
				}
				string lower = path.ToLowerInvariant();
				bool stream = clip.length >= StreamFromSeconds || StreamWords.Any(w => System.IO.Path.GetFileName(lower).Contains(w));
				AudioImporterSampleSettings wanted = importer.GetOverrideSampleSettings(Platform);
				wanted.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
				wanted.sampleRateOverride = SampleRate;
				wanted.loadType = stream ? AudioClipLoadType.Streaming : AudioClipLoadType.CompressedInMemory;
				wanted.compressionFormat = stream ? AudioCompressionFormat.MP3 : AudioCompressionFormat.HEVAG;
				wanted.quality = 0.5f;
				if (stream)
				{
					streamed++;
					lines.Add(string.Format("  {0,6:0.0} s  {1}", clip.length, path));
				}
				else
				{
					effects++;
				}
				importer.SetOverrideSampleSettings(Platform, wanted);
				importer.preloadAudioData = !stream; // a streamed clip is read when it plays, not when the scene loads
				importer.SaveAndReimport();
				changed++;
			}
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
		}
		lines.Sort();
		lines.Insert(0, streamed + " streamed (MP3), " + effects + " effects (HEVAG in memory), all " + SampleRate + " Hz on " + Platform + ":");
		System.IO.Directory.CreateDirectory("Logs");
		System.IO.File.WriteAllLines("Logs/audio_vita.txt", lines.ToArray());
		Debug.Log("[KFAUDIO] " + lines[0] + " " + changed + " clips set");
	}
}
