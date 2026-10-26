using UnityEngine;

// Unity 4 lightmaps of a level (PS Vita port; adapted from Borderlands' BOLSceneLightmaps).
//
// The exported scenes keep each renderer's Unity 4 lightmap index and scale/offset and the LightmapFar-N maps, but in
// Unity 4's LightmapSettings format, which Unity 2018 drops (it keeps lightmaps in a lighting data asset), so a level
// loads unlit. Assets/Editor/KFSceneLightmapsSetup.cs records them here (one per scene, with the maps converted to
// HDR so Unity encodes them for the platform) and they are put back when the scene loads.
//
// The lights are told they are baked too. Unity 4 drew these levels in single lightmap mode: an Auto light is in the
// lightmaps and lights only renderers without one (players, zeds, pickups) in real time; a Baked Only light lights
// nothing in real time. Without bake data Unity 2018 would light every lightmapped wall again in real time with every
// one of the level's lights (hundreds), twice as bright and an extra pass per light on the PS Vita.
//
// The level's Unity 4 light probes (converted by KFLightProbesConvert) are assigned too: players, zeds, NPCs and pickups
// were lit by them, and without them they get only the dark flat ambient.
//
// The level's static scenery (m_staticBatch, chosen by the setup: lightmapped, not animated, scripted or referenced by a
// script) is then static batched. The Android build had no static batching (thousands of draw calls a level); it is
// done here, after the lightmaps are applied, because batching bakes each renderer's lightmap scale/offset into the
// combined mesh and a build-time batch would see the cleared Unity 4 values.
public class KFSceneLightmaps : MonoBehaviour
{
	// Unity 4 Light.m_Lightmapping values
	public const int RealtimeOnly = 0;

	public const int Auto = 1;

	public const int BakedOnly = 2;

	public Texture2D[] m_lightmaps;

	public Renderer[] m_renderers;

	public int[] m_indices;

	public Vector4[] m_scaleOffsets;

	public Light[] m_lights;

	public int[] m_lightModes;

	public GameObject[] m_staticBatch;

	public LightProbes m_lightProbes;

	private void Awake()
	{
		Apply();
	}

	public void Apply()
	{
		LightmapData[] data = new LightmapData[m_lightmaps.Length];
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = new LightmapData { lightmapColor = m_lightmaps[i] };
		}
		LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
		LightmapSettings.lightmaps = data;
		if (m_lightProbes != null)
		{
			LightmapSettings.lightProbes = m_lightProbes;
		}
		for (int i = 0; i < m_renderers.Length; i++)
		{
			if (m_renderers[i] != null)
			{
				m_renderers[i].lightmapIndex = m_indices[i];
				m_renderers[i].lightmapScaleOffset = m_scaleOffsets[i];
			}
		}
		BakedLights();
		if (m_staticBatch != null && m_staticBatch.Length > 0)
		{
			StaticBatchingUtility.Combine(m_staticBatch, base.gameObject);
		}
	}

	private void BakedLights()
	{
		if (m_lights == null || m_lightModes == null || m_lights.Length != m_lightModes.Length)
		{
			return;
		}
		for (int i = 0; i < m_lights.Length; i++)
		{
			Light light = m_lights[i];
			if (light == null || m_lightModes[i] == RealtimeOnly)
			{
				continue;
			}
			LightBakingOutput output = new LightBakingOutput();
			output.isBaked = true;
			output.occlusionMaskChannel = -1;
			output.probeOcclusionLightIndex = -1;
			if (m_lightModes[i] == Auto)
			{
				output.lightmapBakeType = LightmapBakeType.Mixed;
				output.mixedLightingMode = MixedLightingMode.Subtractive; // in the lightmaps; real time for the rest
			}
			else
			{
				output.lightmapBakeType = LightmapBakeType.Baked;
			}
			light.bakingOutput = output;
		}
	}
}
