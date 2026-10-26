using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{
	public enum SpeechLine
	{
		WeaponPickedUp = 0,
		LowHealth = 1,
		Healing = 2,
		OutOfAmmo = 3,
		Reload = 4,
		OpenShop = 5,
		PlayerRespawn = 6,
		SpecimenBurnedToDeath = 7,
		ClotGrab = 8,
		BloatPuke = 9,
		GorefastKill = 10,
		CrawlerLeap = 11,
		StalkerDetect = 12,
		StalkerKill = 13,
		SirenDetect = 14,
		SirenKill = 15,
		ScrakeDetect = 16,
		FleshpoundEnrage = 17,
		PatriarchGatlingGun = 18,
		PatriarchStealth = 19,
		PatriarchRocketLauncher = 20,
		LargeSpecimenKilled = 21,
		FireAxeEquip = 22,
		HuntingShotgunEquip = 23,
		DualHandcannonEquip = 24,
		LAWEquip = 25,
		MoneyDropped = 26
	}

	public enum GlobalSpeechLine
	{
		Start = 0,
		Win = 1,
		Lose = 2,
		KillStreakShort = 3,
		KillStreakLong = 4,
		KillStreakExtreme = 5
	}

	public enum SpeechSetType
	{
		MaleVoiceOne = 0,
		MaleVoiceTwo = 1
	}

	[Serializable]
	public class SpeechCooldown
	{
		public SpeechLine Line;

		public float Cooldown;

		public SpeechCooldown(SpeechLine line, float cooldown)
		{
			Line = line;
			Cooldown = cooldown;
		}
	}

	[Serializable]
	public class GlobalSpeechCooldown
	{
		public GlobalSpeechLine Line;

		public float Cooldown;

		public GlobalSpeechCooldown(GlobalSpeechLine line, float cooldown)
		{
			Line = line;
			Cooldown = cooldown;
		}
	}

	[Serializable]
	public class SpeechSet
	{
		public SpeechSetType Type;

		public AudioClip[] WeaponPickedUp;

		public AudioClip[] LowHealth;

		public AudioClip[] Healing;

		public AudioClip[] OutOfAmmo;

		public AudioClip[] Reload;

		public AudioClip[] OpenShop;

		public AudioClip[] PlayerRespawn;

		public AudioClip[] MoneyDropped;

		public AudioClip[] SpecimenBurnedToDeath;

		public AudioClip[] ClotGrab;

		public AudioClip[] BloatPuke;

		public AudioClip[] GorefastKill;

		public AudioClip[] CrawlerLeap;

		public AudioClip[] StalkerDetect;

		public AudioClip[] StalkerKill;

		public AudioClip[] SirenDetect;

		public AudioClip[] SirenKill;

		public AudioClip[] ScrakeDetect;

		public AudioClip[] FleshpoundEnrage;

		public AudioClip[] PatriarchGatlingGun;

		public AudioClip[] PatriarchStealth;

		public AudioClip[] PatriarchRocketLauncher;

		public AudioClip[] LargeSpecimenKill;

		public AudioClip[] FireAxeEquip;

		public AudioClip[] HuntingShotgunEquip;

		public AudioClip[] DualHandcannonEquip;

		public AudioClip[] LAWEquip;

		public AudioClip GetClip(SpeechLine line)
		{
			AudioClip result = null;
			AudioClip[] clipGroup = GetClipGroup(line);
			if (clipGroup != null)
			{
				result = GetRandomClip(clipGroup);
			}
			return result;
		}

		public AudioClip[] GetClipGroup(SpeechLine line)
		{
			AudioClip[] result = null;
			switch (line)
			{
			case SpeechLine.WeaponPickedUp:
				result = WeaponPickedUp;
				break;
			case SpeechLine.LowHealth:
				result = LowHealth;
				break;
			case SpeechLine.Healing:
				result = Healing;
				break;
			case SpeechLine.OutOfAmmo:
				result = OutOfAmmo;
				break;
			case SpeechLine.Reload:
				result = Reload;
				break;
			case SpeechLine.OpenShop:
				result = OpenShop;
				break;
			case SpeechLine.PlayerRespawn:
				result = PlayerRespawn;
				break;
			case SpeechLine.MoneyDropped:
				result = MoneyDropped;
				break;
			case SpeechLine.SpecimenBurnedToDeath:
				result = SpecimenBurnedToDeath;
				break;
			case SpeechLine.ClotGrab:
				result = ClotGrab;
				break;
			case SpeechLine.BloatPuke:
				result = BloatPuke;
				break;
			case SpeechLine.GorefastKill:
				result = GorefastKill;
				break;
			case SpeechLine.CrawlerLeap:
				result = CrawlerLeap;
				break;
			case SpeechLine.StalkerDetect:
				result = StalkerDetect;
				break;
			case SpeechLine.StalkerKill:
				result = StalkerKill;
				break;
			case SpeechLine.SirenDetect:
				result = SirenDetect;
				break;
			case SpeechLine.SirenKill:
				result = SirenKill;
				break;
			case SpeechLine.ScrakeDetect:
				result = ScrakeDetect;
				break;
			case SpeechLine.FleshpoundEnrage:
				result = FleshpoundEnrage;
				break;
			case SpeechLine.PatriarchGatlingGun:
				result = PatriarchGatlingGun;
				break;
			case SpeechLine.PatriarchStealth:
				result = PatriarchStealth;
				break;
			case SpeechLine.PatriarchRocketLauncher:
				result = PatriarchRocketLauncher;
				break;
			case SpeechLine.LargeSpecimenKilled:
				result = LargeSpecimenKill;
				break;
			case SpeechLine.FireAxeEquip:
				result = FireAxeEquip;
				break;
			case SpeechLine.HuntingShotgunEquip:
				result = HuntingShotgunEquip;
				break;
			case SpeechLine.DualHandcannonEquip:
				result = DualHandcannonEquip;
				break;
			case SpeechLine.LAWEquip:
				result = LAWEquip;
				break;
			}
			return result;
		}

		private AudioClip GetRandomClip(AudioClip[] group)
		{
			AudioClip result = null;
			if (group.Length > 0)
			{
				int num = UnityEngine.Random.Range(0, group.Length);
				result = group[num];
			}
			return result;
		}
	}

	[Serializable]
	public class GlobalSpeechSet
	{
		public AudioClip[] Start;

		public AudioClip[] Win;

		public AudioClip[] Lose;

		public AudioClip[] KillStreakShort;

		public AudioClip[] KillStreakLong;

		public AudioClip[] KillStreakExtreme;

		public AudioClip GetClip(GlobalSpeechLine line)
		{
			AudioClip result = null;
			AudioClip[] clipGroup = GetClipGroup(line);
			if (clipGroup != null)
			{
				result = GetRandomClip(clipGroup);
			}
			return result;
		}

		public AudioClip[] GetClipGroup(GlobalSpeechLine line)
		{
			AudioClip[] result = null;
			switch (line)
			{
			case GlobalSpeechLine.Start:
				result = Start;
				break;
			case GlobalSpeechLine.Win:
				result = Win;
				break;
			case GlobalSpeechLine.Lose:
				result = Lose;
				break;
			case GlobalSpeechLine.KillStreakShort:
				result = KillStreakShort;
				break;
			case GlobalSpeechLine.KillStreakLong:
				result = KillStreakLong;
				break;
			case GlobalSpeechLine.KillStreakExtreme:
				result = KillStreakExtreme;
				break;
			}
			return result;
		}

		private AudioClip GetRandomClip(AudioClip[] group)
		{
			AudioClip result = null;
			if (group.Length > 0)
			{
				int num = UnityEngine.Random.Range(0, group.Length);
				result = group[num];
			}
			return result;
		}
	}

	private static SpeechManager instance;

	public SpeechSet[] SpeechSets;

	public GlobalSpeechSet GlobalSet;

	public SpeechCooldown[] CooldownSettings;

	public GlobalSpeechCooldown[] GlobalCooldownSettings;

	public AudioSource Source;

	public Transform SourceTransform;

	public float Volume = 0.5f;

	public float GlobalVolume = 0.5f;

	public bool LetTimeAffectPitch = true;

	public bool isPlaying;

	private SpeechController activeController;

	public AudioClip activeClip;

	private List<SpeechCooldown> activeCooldowns = new List<SpeechCooldown>();

	private List<GlobalSpeechCooldown> activeGlobalCooldowns = new List<GlobalSpeechCooldown>();

	public static bool CanPlay
	{
		get
		{
			return instance != null && !instance.isPlaying;
		}
	}

	private void Start()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void Update()
	{
		UpdateCooldowns();
	}

	public static void PlaySpeech(SpeechLine line)
	{
		if (CanPlay && !instance.IsOnCooldown(line))
		{
			instance.PlaySpeechForRandomSoldier(line);
		}
	}

	public static void PlaySpeech(SpeechLine line, GameObject source)
	{
		if (CanPlay && source != null && !instance.IsOnCooldown(line))
		{
			instance.PlaySpeechForSoldier(line, source);
		}
	}

	public static void PlayGlobalSpeech(GlobalSpeechLine line)
	{
		if (CanPlay && !instance.IsOnCooldown(line))
		{
			instance.PlayGlobalSpeechInternal(line);
		}
	}

	private void PlaySpeechForSoldier(SpeechLine line, GameObject soldier)
	{
		SpeechController controller = GetController(soldier);
		if (!(controller != null))
		{
			return;
		}
		SpeechSetType type = controller.Type;
		SpeechSet speechSet = GetSpeechSet(type);
		if (speechSet != null)
		{
			AudioClip clip = speechSet.GetClip(line);
			if (clip != null)
			{
				Vector3 position = controller.FeedbackMount.position;
				PlayClipAtLocation(clip, position);
				controller.DisplayFeedback();
				activeController = controller;
				isPlaying = true;
				activeClip = clip;
				float length = clip.length;
				StartCoroutine(HideFeedbackWhenDone(length));
				SetCooldown(line);
			}
			else
			{
				Debug.Log("Failed to locate speech clip for " + line);
			}
		}
	}

	private void PlaySpeechForRandomSoldier(SpeechLine line)
	{
		GameObject randomSoldier = GetRandomSoldier();
		if (randomSoldier != null)
		{
			PlaySpeechForSoldier(line, randomSoldier);
		}
	}

	private void PlayGlobalSpeechInternal(GlobalSpeechLine line)
	{
		AudioClip clip = GlobalSet.GetClip(line);
		if (clip != null)
		{
			AudioManager.PlayClipAtPoint(clip, Vector3.zero, GlobalVolume, (!LetTimeAffectPitch) ? 1f : Time.timeScale);
			isPlaying = true;
			activeClip = clip;
			float length = clip.length;
			StartCoroutine(HideFeedbackWhenDone(length));
			SetCooldown(line);
		}
	}

	private void PlayClipAtLocation(AudioClip clip, Vector3 location)
	{
		AudioManager.PlayClipAtPoint(clip, location, Volume, (!LetTimeAffectPitch) ? 1f : Time.timeScale);
	}

	private IEnumerator HideFeedbackWhenDone(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (activeController != null)
		{
			activeController.HideFeedback();
			activeController = null;
		}
		isPlaying = false;
		activeClip = null;
	}

	private GameObject GetRandomSoldier()
	{
		GameObject result = null;
		if (SoldierManager.Instance != null)
		{
			List<SoldierItem> aliveSoldiers = SoldierManager.Instance.GetAliveSoldiers();
			if (aliveSoldiers.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, aliveSoldiers.Count);
				result = aliveSoldiers[index].SoldierObject;
			}
		}
		return result;
	}

	private SpeechController GetController(GameObject source)
	{
		return source.GetComponent<SpeechController>();
	}

	private SpeechSetType? GetType(GameObject source)
	{
		SpeechController component = source.GetComponent<SpeechController>();
		if (component != null)
		{
			return component.Type;
		}
		return null;
	}

	private SpeechSet GetSpeechSet(SpeechSetType type)
	{
		SpeechSet[] speechSets = SpeechSets;
		foreach (SpeechSet speechSet in speechSets)
		{
			if (speechSet.Type == type)
			{
				return speechSet;
			}
		}
		return null;
	}

	private void SetSourceLocation(Vector3 location)
	{
		SourceTransform.position = location;
	}

	private bool IsOnCooldown(SpeechLine line)
	{
		foreach (SpeechCooldown activeCooldown in activeCooldowns)
		{
			if (activeCooldown.Line == line)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsOnCooldown(GlobalSpeechLine line)
	{
		foreach (GlobalSpeechCooldown activeGlobalCooldown in activeGlobalCooldowns)
		{
			if (activeGlobalCooldown.Line == line)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateCooldowns()
	{
		if (activeCooldowns.Count > 0)
		{
			float deltaTime = Time.deltaTime;
			for (int num = activeCooldowns.Count - 1; num >= 0; num--)
			{
				SpeechCooldown speechCooldown = activeCooldowns[num];
				speechCooldown.Cooldown -= deltaTime;
				if (speechCooldown.Cooldown <= 0f)
				{
					activeCooldowns.RemoveAt(num);
				}
			}
		}
		if (activeGlobalCooldowns.Count <= 0)
		{
			return;
		}
		float deltaTime2 = Time.deltaTime;
		for (int num2 = activeGlobalCooldowns.Count - 1; num2 >= 0; num2--)
		{
			GlobalSpeechCooldown globalSpeechCooldown = activeGlobalCooldowns[num2];
			globalSpeechCooldown.Cooldown -= deltaTime2;
			if (globalSpeechCooldown.Cooldown <= 0f)
			{
				activeGlobalCooldowns.RemoveAt(num2);
			}
		}
	}

	private void SetCooldown(SpeechLine line)
	{
		float? cooldown = GetCooldown(line);
		if (cooldown.HasValue)
		{
			activeCooldowns.Add(new SpeechCooldown(line, cooldown.Value));
		}
	}

	private void SetCooldown(GlobalSpeechLine line)
	{
		float? cooldown = GetCooldown(line);
		if (cooldown.HasValue)
		{
			activeGlobalCooldowns.Add(new GlobalSpeechCooldown(line, cooldown.Value));
		}
	}

	private float? GetCooldown(SpeechLine line)
	{
		float? result = null;
		SpeechCooldown[] cooldownSettings = CooldownSettings;
		foreach (SpeechCooldown speechCooldown in cooldownSettings)
		{
			if (speechCooldown.Line == line)
			{
				result = speechCooldown.Cooldown;
				break;
			}
		}
		return result;
	}

	private float? GetCooldown(GlobalSpeechLine line)
	{
		float? result = null;
		GlobalSpeechCooldown[] globalCooldownSettings = GlobalCooldownSettings;
		foreach (GlobalSpeechCooldown globalSpeechCooldown in globalCooldownSettings)
		{
			if (globalSpeechCooldown.Line == line)
			{
				result = globalSpeechCooldown.Cooldown;
				break;
			}
		}
		return result;
	}
}
