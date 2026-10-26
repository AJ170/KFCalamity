using System;
using System.Collections.Generic;
using EventHelpers;
using UnityEngine;

public class MuzzleFlashEffect : EffectBaseListener
{
	public enum ProjectileBoneOffset
	{
		None = 0,
		RightMuzzle = 1,
		LeftMuzzle = 2
	}

	[Serializable]
	public class ScaleVariable
	{
		public float Min = 0.5f;

		public float Max = 1.5f;
	}

	public Transform Muzzle;

	public GameObject[] MuzzleFlashes;

	public Vector3 Offset = Vector3.zero;

	public ScaleVariable Scale;

	public float Duration = 0.05f;

	public ProjectileBoneOffset PresetOffset;

	private List<GameObject> muzzleFlashInstances = new List<GameObject>();

	private GameObject activeMuzzleFlash;

	private float muzzleFlashCounter;

	private GameObject parent;

	protected override void Awake()
	{
		base.Awake();
		if (MuzzleFlashes.Length > 0)
		{
			GameObject[] muzzleFlashes = MuzzleFlashes;
			foreach (GameObject original in muzzleFlashes)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
				Transform transform = gameObject.transform;
				transform.parent = ((!(Muzzle != null)) ? base.transform : Muzzle);
				transform.localPosition = Offset;
				transform.localRotation = Quaternion.identity;
				gameObject.SetActive(false);
				muzzleFlashInstances.Add(gameObject);
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	private void Update()
	{
		if (muzzleFlashCounter > 0f)
		{
			muzzleFlashCounter -= Time.deltaTime;
			if (muzzleFlashCounter <= 0f && activeMuzzleFlash != null)
			{
				activeMuzzleFlash.SetActive(false);
				activeMuzzleFlash = null;
			}
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (Muzzle == null)
		{
			if (PresetOffset != 0)
			{
				FindMuzzle();
			}
			else
			{
				Muzzle = base.transform;
			}
		}
		GameObject randomMuzzleFlash = GetRandomMuzzleFlash();
		if (randomMuzzleFlash != null)
		{
			if (activeMuzzleFlash != null)
			{
				activeMuzzleFlash.SetActive(false);
			}
			activeMuzzleFlash = randomMuzzleFlash;
			activeMuzzleFlash.transform.localScale = Vector3.one * UnityEngine.Random.Range(Scale.Min, Scale.Max);
			activeMuzzleFlash.transform.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 90f));
			activeMuzzleFlash.SetActive(true);
			muzzleFlashCounter = Duration;
		}
	}

	private GameObject GetRandomMuzzleFlash()
	{
		GameObject result = null;
		int count = muzzleFlashInstances.Count;
		if (count > 0)
		{
			result = muzzleFlashInstances[UnityEngine.Random.Range(0, count)];
		}
		return result;
	}

	private void FindMuzzle()
	{
		if (parent != null)
		{
			WeaponController component = parent.GetComponent<WeaponController>();
			if (component != null)
			{
				Weapon activeWeapon = component.ActiveWeapon;
				if (activeWeapon != null)
				{
					Transform transform = null;
					switch (PresetOffset)
					{
					case ProjectileBoneOffset.LeftMuzzle:
						transform = activeWeapon.SecondaryMuzzleBone;
						break;
					case ProjectileBoneOffset.RightMuzzle:
						transform = activeWeapon.PrimaryMuzzleBone;
						break;
					}
					if (transform != null)
					{
						for (int i = 0; i < muzzleFlashInstances.Count; i++)
						{
							Transform transform2 = muzzleFlashInstances[i].transform;
							transform2.parent = transform;
							transform2.localPosition = Offset;
							transform2.localRotation = Quaternion.identity;
						}
						return;
					}
				}
			}
		}
		Muzzle = base.transform;
	}
}
