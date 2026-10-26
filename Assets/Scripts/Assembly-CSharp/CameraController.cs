using System.Collections.Generic;
using Calamity.Camera;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public FollowCamera FollowCamera;

	public FocusCamera FocusCamera;

	public bool ForceEffectsDraw;

	private IdleCamera idleCamera = new IdleCamera();

	private CameraState activeState;

	private CameraStateType activeStateType;

	private List<CameraEffect> effects = new List<CameraEffect>();

	private new Transform transform;

	private Vector3 totalCameraOffset = Vector3.zero;

	private RenderTexture effectRenderTexture;

	private bool usingRenderTexture;

	private bool dirtyRenderTexture;

	private bool staticVisualEffects;

	private void Start()
	{
		transform = base.transform;
		if (FollowCamera != null)
		{
			FollowCamera.Start(transform);
		}
		if (FocusCamera != null)
		{
			FocusCamera.Start(transform);
		}
		idleCamera.Start(transform);
		SetActiveState(CameraStateType.Follow);
	}

	private void OnDestroy()
	{
		if (FollowCamera != null)
		{
			FollowCamera.Shutdown();
		}
		if (FocusCamera != null)
		{
			FocusCamera.Shutdown();
		}
		idleCamera.Shutdown();
		if (effectRenderTexture != null)
		{
			effectRenderTexture.DiscardContents();
			if (effectRenderTexture.IsCreated())
			{
				effectRenderTexture.Release();
			}
			effectRenderTexture = null;
		}
		for (int i = 0; i < effects.Count; i++)
		{
			effects[i].Shutdown();
		}
		effects.Clear();
	}

	private void Update()
	{
		totalCameraOffset = Vector3.zero;
		if (activeState != null)
		{
			activeState.Update();
		}
		UpdateEffects();
	}

	private void LateUpdate()
	{
		if (activeState != null)
		{
			activeState.LateUpdate();
		}
		UpdateTransform();
	}

	private void UpdateTransform()
	{
		if (activeState != null)
		{
			transform.position = activeState.Position + totalCameraOffset;
			transform.rotation = activeState.Rotation;
		}
	}

	public void SetActiveState(CameraStateType type)
	{
		if (type == activeStateType)
		{
			return;
		}
		if (type == CameraStateType.None)
		{
			if (activeState != null)
			{
				activeState.OnDeactivated();
			}
			activeStateType = type;
			return;
		}
		CameraState state = GetState(type);
		if (state != null)
		{
			if (activeState != null)
			{
				activeState.OnDeactivated();
			}
			activeState = state;
			activeState.OnActivated();
			activeStateType = type;
		}
	}

	private CameraState GetState(CameraStateType type)
	{
		switch (type)
		{
		case CameraStateType.Follow:
			return FollowCamera;
		case CameraStateType.Focus:
			return FocusCamera;
		case CameraStateType.Idle:
			return idleCamera;
		default:
			return null;
		}
	}

	public void SetFocusTarget(GameObject target)
	{
		if (FocusCamera != null)
		{
			FocusCamera.SetTarget(target);
		}
	}

	public void AddEffect(CameraEffect addon)
	{
		addon.Start(transform);
		effects.Add(addon);
		OnEffectAdded();
	}

	public void RemoveEffect(CameraEffect addon)
	{
		if (effects.Remove(addon))
		{
			addon.Shutdown();
			OnEffectRemoved();
		}
	}

	private void RemoveEffect(int index)
	{
		if (index >= 0 && index < effects.Count)
		{
			effects[index].Shutdown();
			effects.RemoveAt(index);
			OnEffectRemoved();
		}
	}

	public void RemoveEffectByType(CameraEffectType type)
	{
		for (int num = effects.Count - 1; num >= 0; num--)
		{
			if (effects[num].Type == type)
			{
				RemoveEffect(num);
			}
		}
	}

	public void RemoveEffectsBySource(GameObject source)
	{
		if (!(source != null))
		{
			return;
		}
		for (int num = effects.Count - 1; num >= 0; num--)
		{
			if (effects[num].Source == source)
			{
				RemoveEffect(num);
			}
		}
	}

	private void OnEffectAdded()
	{
		UpdateRenderTextureStatus();
	}

	private void OnEffectRemoved()
	{
		UpdateRenderTextureStatus();
	}

	public bool HasEffect(CameraEffectType type)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			if (effects[i].Type == type)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateEffects()
	{
		for (int num = effects.Count - 1; num >= 0; num--)
		{
			CameraEffect cameraEffect = effects[num];
			cameraEffect.Update();
			if (cameraEffect.IsDone)
			{
				RemoveEffect(num);
			}
			else if (cameraEffect.Archetype == CameraEffectArchetype.Transform)
			{
				totalCameraOffset += cameraEffect.Position;
			}
		}
	}

	private void UpdateRenderTextureStatus()
	{
		usingRenderTexture = false;
		staticVisualEffects = true;
		for (int i = 0; i < effects.Count; i++)
		{
			if (effects[i].Archetype == CameraEffectArchetype.Visual)
			{
				usingRenderTexture = true;
				if (!effects[i].IsStatic)
				{
					staticVisualEffects = false;
				}
			}
		}
		if (usingRenderTexture)
		{
			dirtyRenderTexture = true;
		}
		else if (effectRenderTexture != null && effectRenderTexture.IsCreated())
		{
			effectRenderTexture.Release();
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (usingRenderTexture && (dirtyRenderTexture || !staticVisualEffects || ForceEffectsDraw))
		{
			if (effectRenderTexture == null)
			{
				effectRenderTexture = new RenderTexture(source.width, source.height, source.depth, source.format);
			}
			else if (effectRenderTexture.IsCreated())
			{
				effectRenderTexture.DiscardContents();
			}
			bool flag = false;
			if (effects.Count > 0)
			{
				for (int i = 0; i < effects.Count; i++)
				{
					if (effects[i].Archetype == CameraEffectArchetype.Visual && effects[i].OnRenderImage(source, effectRenderTexture))
					{
						flag = true;
						source = effectRenderTexture;
					}
				}
			}
			if (flag)
			{
				dirtyRenderTexture = false;
			}
			else
			{
				usingRenderTexture = false;
			}
		}
		if (usingRenderTexture)
		{
			Graphics.Blit(effectRenderTexture, destination);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
