using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CameraPathHandler : MonoBehaviour
{
	[Serializable]
	public class CameraTravelPaths
	{
		public MenuStates First;

		public MenuStates Second;

		public AnimationClip ToFirst;

		public AnimationClip ToSecond;
	}

	public List<CameraTravelPaths> Paths;

	public GameObject MenuManagerObject;

	public GameObject GameDoors;

	private MenuDoorScript startDoors;

	private GuiMenuManager menuManager;

	private Animation animationComponent;

	private void Awake()
	{
		CameraFade.StartAlphaFade(Color.black, true, 0.6f, 0.1f);
	}

	private void Start()
	{
		if (MenuManagerObject != null)
		{
			menuManager = MenuManagerObject.GetComponent<GuiMenuManager>();
		}
		if (GameDoors != null)
		{
			startDoors = GameDoors.GetComponent<MenuDoorScript>();
		}
		animationComponent = base.gameObject.GetComponent<Animation>();
		foreach (CameraTravelPaths path in Paths)
		{
			if (path.ToFirst != null)
			{
				animationComponent.AddClip(path.ToFirst, path.Second.ToString() + "To" + path.First);
			}
			if (path.ToSecond != null)
			{
				animationComponent.AddClip(path.ToSecond, path.First.ToString() + "To" + path.Second);
			}
		}
	}

	public void Play(MenuStates from, MenuStates to, bool skip = false)
	{
		bool flag = false;
		float time = 0f;
		if (to == MenuStates.GameStart && startDoors != null)
		{
			flag = true;
			time = 1.5f;
			CameraFade.StartAlphaFade(Color.black, false, 2f, 0.5f);
			startDoors.Open();
		}
		if (to == MenuStates.Exit)
		{
			flag = true;
			time = 1.8f;
			CameraFade.StartAlphaFade(Color.black, false, 2f, 0.5f);
		}
		if (to == MenuStates.Tutorial)
		{
			flag = true;
			time = 3.2f;
			CameraFade.StartAlphaFade(Color.black, false, 2f, 1.8f);
		}
		if (MusicManager.Instance != null && to == MenuStates.Exit)
		{
			MusicManager.Instance.SetTemporaryVolume(0f, 1f / 3f);
		}
		animationComponent.clip = animationComponent.GetClip(from.ToString() + "To" + to);
		if (!(animationComponent.clip != null))
		{
			return;
		}
		animationComponent.Play();
		skip = false;
		if (skip)
		{
			foreach (AnimationState item in animationComponent)
			{
				item.normalizedTime = 1f;
				OnAnimationComplete();
			}
			return;
		}
		if (!flag)
		{
			Invoke("OnAnimationComplete", animationComponent.clip.length);
		}
		else
		{
			Invoke("OnAnimationComplete", time);
		}
	}

	private void OnAnimationComplete()
	{
		menuManager.OnCameraTransitionComplete();
	}
}
