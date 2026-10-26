using System;
using System.Collections.Generic;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class StealthController : MonoBehaviour, IEffectTriggerListener
{
	public enum StealthState
	{
		Stealthed = 0,
		Detected = 1,
		Visible = 2
	}

	public delegate void StealthUpdateDelegate(StealthState steathState);

	public bool StealthAtStart;

	public float StealthVisibility = 0.1f;

	public float DetectedVisibility = 0.6f;

	public float StealthCooldown = 1f;

	public float FadeSpeed = 1f;

	public bool CanBeDetected = true;

	public bool DamageBreaksStealth = true;

	public bool AttackBreaksStealth;

	public bool AbilityUsageBreaksStealth;

	public UnstealthAudio unstealthAudio;

	private StealthManager stealthManager;

	private AlphaLerp alphaLerp = new AlphaLerp();

	private List<MaterialData> materials = new List<MaterialData>();

	private Shader transparentDiffuse;

	private bool stealthed;

	private bool detected;

	private bool transparentShader;

	private float stealthCooldownCounter;

	private StealthUpdateDelegate stealthUpdateListeners;

	public bool IsStealthed
	{
		get
		{
			return stealthed;
		}
	}

	public bool IsDetected
	{
		get
		{
			return detected;
		}
	}

	public bool CanStealth
	{
		get
		{
			return !stealthed && stealthCooldownCounter == 0f && materials.Count > 0;
		}
	}

	private void Awake()
	{
		if (DamageBreaksStealth || AttackBreaksStealth || AbilityUsageBreaksStealth)
		{
			GlobalHelpers.RegisterEffectEventListener(base.gameObject, this);
		}
		GlobalHelpers.AddWeaponMountReceiver(base.gameObject, OnWeaponMounted);
		Mathf.Clamp01(StealthVisibility);
		transparentDiffuse = Shader.Find("Legacy Shaders/Transparent/Diffuse");
	}

	private void Start()
	{
		stealthManager = StealthManager.Instance;
		if (stealthManager != null)
		{
			stealthManager.RegisterStealther(this);
		}
		if (unstealthAudio == null)
		{
			unstealthAudio = GetComponentInChildren<UnstealthAudio>();
		}
	}

	private void OnDestroy()
	{
		if (stealthManager != null)
		{
			stealthManager.UnregisterStealther(this);
		}
		stealthUpdateListeners = null;
		unstealthAudio = null;
	}

	private void OnWeaponMounted(GameObject weaponInstance)
	{
		FindMaterials();
		if (StealthAtStart)
		{
			Stealth();
		}
	}

	private void Update()
	{
		UpdateStealthCooldown();
		alphaLerp.Update();
		if (alphaLerp.Lerping)
		{
			UpdateAlpha();
		}
	}

	private void UpdateAlpha()
	{
		if (!transparentShader)
		{
			return;
		}
		foreach (MaterialData material in materials)
		{
			Color color = material.Material.color;
			color.a = alphaLerp.Alpha;
			material.Material.color = color;
		}
	}

	private void UpdateStealthCooldown()
	{
		if (stealthCooldownCounter > 0f)
		{
			stealthCooldownCounter -= Time.deltaTime;
			if (stealthCooldownCounter <= 0f)
			{
				stealthCooldownCounter = 0f;
			}
		}
	}

	public void OnEffectTrigger(TriggerBase e)
	{
		if (((DamageBreaksStealth && e.Id == TriggerId.DamageReceived) || (AttackBreaksStealth && e.Id == TriggerId.AIAttack) || (AbilityUsageBreaksStealth && e.Id == TriggerId.AIUseAbility)) && stealthed)
		{
			Unstealth();
		}
	}

	public void Stealth()
	{
		stealthed = true;
		SetTransparentShader();
		float goal;
		if (IsDetected && CanBeDetected)
		{
			goal = DetectedVisibility;
		}
		else
		{
			detected = false;
			goal = StealthVisibility;
		}
		alphaLerp.Lerp(goal, FadeSpeed, OnLerpDone, true);
		TriggerUpdateEvent();
	}

	public void Unstealth()
	{
		stealthed = false;
		detected = false;
		alphaLerp.Lerp(1f, FadeSpeed, OnLerpDone, true);
		stealthCooldownCounter = StealthCooldown;
		if (unstealthAudio != null)
		{
			unstealthAudio.Play();
		}
		TriggerUpdateEvent();
	}

	public void OnDetected()
	{
		detected = true;
		SetTransparentShader();
		alphaLerp.Lerp(DetectedVisibility, FadeSpeed, OnLerpDone, true);
		stealthCooldownCounter = StealthCooldown;
		TriggerUpdateEvent();
	}

	public void OnRestealth()
	{
		detected = false;
		SetTransparentShader();
		alphaLerp.Lerp(StealthVisibility, FadeSpeed, OnLerpDone, true);
		TriggerUpdateEvent();
	}

	private void OnLerpDone()
	{
		UpdateAlpha();
		if (!stealthed)
		{
			SetDefaultShader();
		}
	}

	public void RegisterListener(StealthUpdateDelegate callback)
	{
		stealthUpdateListeners = (StealthUpdateDelegate)Delegate.Combine(stealthUpdateListeners, callback);
	}

	public void UnregisterListener(StealthUpdateDelegate callback)
	{
		stealthUpdateListeners = (StealthUpdateDelegate)Delegate.Remove(stealthUpdateListeners, callback);
	}

	private void TriggerUpdateEvent()
	{
		if (stealthUpdateListeners != null)
		{
			stealthUpdateListeners(GetStealthState());
		}
	}

	public StealthState GetStealthState()
	{
		if (stealthed)
		{
			if (detected)
			{
				return StealthState.Detected;
			}
			return StealthState.Stealthed;
		}
		return StealthState.Visible;
	}

	private void FindMaterials()
	{
		SetDefaultShader();
		materials = GlobalHelpers.FindMaterials<Renderer>(base.transform.root);
	}

	private void SetTransparentShader()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.shader = transparentDiffuse;
		}
		transparentShader = true;
	}

	private void SetDefaultShader()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.shader = material.DefaultShader;
		}
		transparentShader = false;
	}
}
