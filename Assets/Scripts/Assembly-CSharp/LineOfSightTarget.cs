using System;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightTarget : LineOfSightComponent
{
	public class RendererData
	{
		public Renderer Renderer;

		public Material Material;

		public Shader DefaultShader;
	}

	public bool DisableRendererWhenNotSeen;

	public bool DisableRenderersOnStart;

	public Renderer[] Renderers;

	public bool UseFade;

	public float TimeBeforeFade = 1f;

	public float FadeOutTime = 1f;

	public float FadeInTime = 0.25f;

	private Action gainVisibilityListener;

	private Action lostVisibilityListener;

	private float timeSinceLastSeen;

	private bool spotted;

	private bool visible;

	private bool forcedVisible;

	private bool renderersVisible;

	private AlphaLerp alphaLerp;

	private List<RendererData> rendererData = new List<RendererData>();

	private Shader fadeShader;

	private bool usingFadeShader;

	private bool faded;

	public bool Visible
	{
		get
		{
			return visible;
		}
	}

	public float TimeSinceLastSeen
	{
		get
		{
			return timeSinceLastSeen;
		}
	}

	public bool Fading
	{
		get
		{
			return alphaLerp.Lerping;
		}
	}

	public bool RenderersVisible
	{
		get
		{
			return renderersVisible;
		}
	}

	public float Alpha
	{
		get
		{
			if (Fading)
			{
				return alphaLerp.Alpha;
			}
			if (visible)
			{
				return 1f;
			}
			return 0f;
		}
	}

	protected override void Start()
	{
		alphaLerp = new AlphaLerp((!DisableRenderersOnStart) ? 1f : 0f);
		SetupRendererData();
		fadeShader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
		if (LineOfSightManager.Instance != null && base.enabled && DisableRenderersOnStart)
		{
			SetRenderersVisible(false);
			faded = true;
		}
		base.Start();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Renderers = null;
		rendererData.Clear();
		fadeShader = null;
		gainVisibilityListener = null;
		lostVisibilityListener = null;
	}

	private void Update()
	{
		if (!visible)
		{
			timeSinceLastSeen += Time.deltaTime;
			if (UseFade && !faded && timeSinceLastSeen > TimeBeforeFade)
			{
				FadeOut();
			}
		}
		if (alphaLerp.Lerping)
		{
			alphaLerp.Update();
			UpdateFadeAlpha();
		}
	}

	protected override void Register()
	{
		LineOfSightManager.Instance.RegisterTarget(this);
	}

	protected override void Unregister()
	{
		LineOfSightManager.Instance.UnregisterTarget(this);
	}

	protected override void OnGainVisibility(LineOfSightComponent source)
	{
		base.OnGainVisibility(source);
		if (!Visible)
		{
			OnSeen();
		}
	}

	protected override void OnLostVisibility(LineOfSightComponent source)
	{
		base.OnLostVisibility(source);
		visible = forcedVisible || visibleTargets.Count > 0;
		if (!visible)
		{
			OnNoLongerSeen();
		}
	}

	private void OnSeen()
	{
		if (!spotted)
		{
			OnSpotted();
		}
		if (UseFade)
		{
			FadeIn();
		}
		else if (!renderersVisible)
		{
			SetRenderersVisible(true);
		}
		TriggerGainVisibilityListener();
		visible = true;
		timeSinceLastSeen = 0f;
	}

	private void OnNoLongerSeen()
	{
		if (UseFade)
		{
			if (TimeBeforeFade <= 0f)
			{
				FadeOut();
			}
		}
		else if (renderersVisible)
		{
			SetRenderersVisible(false);
		}
		TriggerLostVisibilityListener();
	}

	private void OnSpotted()
	{
		spotted = true;
	}

	public void SetForcedVisibility(bool forcedVisibility)
	{
		forcedVisible = forcedVisibility;
		if (forcedVisibility)
		{
			if (!visible)
			{
				OnSeen();
			}
			return;
		}
		visible = visibleTargets.Count > 0;
		if (!visible)
		{
			OnNoLongerSeen();
		}
	}

	private void FadeIn()
	{
		float time = (Fading ? (FadeInTime * (1f - alphaLerp.Alpha)) : FadeInTime);
		if (!renderersVisible)
		{
			SetRenderersVisible(true);
		}
		if (!usingFadeShader)
		{
			SetFadeShader();
		}
		faded = false;
		alphaLerp.Lerp(1f, time, OnFadedIn, true);
	}

	private void FadeOut()
	{
		float time = (Fading ? (FadeInTime * alphaLerp.Alpha) : FadeOutTime);
		if (!renderersVisible)
		{
			SetRenderersVisible(true);
		}
		if (!usingFadeShader)
		{
			SetFadeShader();
		}
		faded = true;
		alphaLerp.Lerp(0f, time, OnFadedOut, true);
	}

	private void OnFadedIn()
	{
		if (usingFadeShader)
		{
			SetDefaultShader();
		}
	}

	private void OnFadedOut()
	{
		if (renderersVisible)
		{
			SetRenderersVisible(false);
		}
		if (usingFadeShader)
		{
			SetDefaultShader();
		}
	}

	private void SetFadeShader()
	{
		foreach (RendererData rendererDatum in rendererData)
		{
			rendererDatum.Material.shader = fadeShader;
		}
		usingFadeShader = true;
	}

	private void SetDefaultShader()
	{
		foreach (RendererData rendererDatum in rendererData)
		{
			rendererDatum.Material.shader = rendererDatum.DefaultShader;
		}
		usingFadeShader = false;
	}

	private void UpdateFadeAlpha()
	{
		if (!usingFadeShader)
		{
			return;
		}
		foreach (RendererData rendererDatum in rendererData)
		{
			Color color = rendererDatum.Material.color;
			color.a = alphaLerp.Alpha;
			rendererDatum.Material.color = color;
		}
	}

	public void AddRenderers(Renderer[] renderers)
	{
		foreach (Renderer renderer in renderers)
		{
			AddRenderer(renderer);
		}
	}

	public void AddRenderer(Renderer renderer)
	{
		RendererData rendererData = new RendererData();
		rendererData.Renderer = renderer;
		rendererData.Material = renderer.material;
		rendererData.DefaultShader = rendererData.Material.shader;
		this.rendererData.Add(rendererData);
	}

	public void RemoveRenderers(Renderer[] renderers)
	{
		foreach (Renderer renderer in renderers)
		{
			RemoveRenderer(renderer);
		}
	}

	public void RemoveRenderer(Renderer renderer)
	{
		for (int i = 0; i < rendererData.Count; i++)
		{
			if (rendererData[i].Renderer == renderer)
			{
				rendererData.RemoveAt(i);
				break;
			}
		}
	}

	private void SetupRendererData()
	{
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			AddRenderer(renderer);
		}
	}

	private void SetRenderersVisible(bool visible)
	{
		foreach (RendererData rendererDatum in rendererData)
		{
			rendererDatum.Renderer.enabled = visible;
		}
		renderersVisible = visible;
	}

	public void RegisterGainVisibilityListener(Action action)
	{
		gainVisibilityListener = (Action)Delegate.Combine(gainVisibilityListener, action);
	}

	public void RegisterLostVisibilityListener(Action action)
	{
		lostVisibilityListener = (Action)Delegate.Combine(lostVisibilityListener, action);
	}

	public void UnregisterGainVisibilityListener(Action action)
	{
		gainVisibilityListener = (Action)Delegate.Remove(gainVisibilityListener, action);
	}

	public void UnregisterLostVisibilityListener(Action action)
	{
		lostVisibilityListener = (Action)Delegate.Remove(lostVisibilityListener, action);
	}

	private void TriggerGainVisibilityListener()
	{
		if (gainVisibilityListener != null)
		{
			gainVisibilityListener();
		}
	}

	private void TriggerLostVisibilityListener()
	{
		if (lostVisibilityListener != null)
		{
			lostVisibilityListener();
		}
	}
}
