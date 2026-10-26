using UnityEngine;

public class TutorialSpeechManager : MonoBehaviour
{
	public float Volume = 1f;

	public AudioSource Audio;

	public AudioClip MovementGreeting;

	public AudioClip MovementDescription;

	public AudioClip MovementLookAround;

	public AudioClip MovemnentDodge;

	public AudioClip MovementProceed;

	public AudioClip AvoidClot;

	public AudioClip AvoidEnough;

	public AudioClip AvoidProceed;

	public AudioClip ShootPickup;

	public AudioClip ShootSwitch;

	public AudioClip ShootDummies;

	public AudioClip ShootReload;

	public AudioClip ShootProceed;

	public AudioClip AimDescription;

	public AudioClip AimFail;

	public AudioClip AimProceed;

	public AudioClip GrenadePickup;

	public AudioClip GrenadeDescription;

	public AudioClip GrenadeProceed;

	public AudioClip SyringePickup;

	public AudioClip SyringeDescription;

	public AudioClip SyringeTargetDescription;

	public AudioClip SyringeFail;

	public AudioClip SyringeThrowWeapon;

	public AudioClip SyringeProceed;

	public AudioClip TurretDescription;

	public AudioClip TurretIncoming;

	public AudioClip TurretProceed;

	public AudioClip ShopDescription;

	public AudioClip ShopProceed;

	public AudioClip WavesLastWarning;

	public AudioClip WavesIncoming;

	public AudioClip WavesKillstreak;

	public AudioClip WavesCleared;

	public AudioClip WavesDone;

	public AudioClip WavesMoreDone;

	public AudioClip Win;

	public AudioClip Lose;

	private static TutorialSpeechManager instance;

	public static TutorialSpeechManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Start()
	{
		instance = this;
		if (Audio == null)
		{
			Audio = base.GetComponent<AudioSource>();
		}
	}

	public static void Play(TutorialSpeechLine line)
	{
		if (instance != null)
		{
			instance.PlayInternal(line);
		}
	}

	public static void Stop()
	{
		if (instance != null)
		{
			instance.StopInternal();
		}
	}

	private void PlayInternal(TutorialSpeechLine line)
	{
		if (!(Audio != null))
		{
			return;
		}
		AudioClip clip = GetClip(line);
		if (clip != null)
		{
			if (Audio.isPlaying)
			{
				Audio.Stop();
			}
			Audio.clip = clip;
			Audio.Play();
		}
	}

	private void StopInternal()
	{
		if (Audio != null && Audio.isPlaying)
		{
			Audio.Stop();
		}
	}

	private AudioClip GetClip(TutorialSpeechLine line)
	{
		AudioClip result = null;
		switch (line)
		{
		case TutorialSpeechLine.MovementGreeting:
			result = MovementGreeting;
			break;
		case TutorialSpeechLine.MovementDescription:
			result = MovementDescription;
			break;
		case TutorialSpeechLine.MovementLookAround:
			result = MovementLookAround;
			break;
		case TutorialSpeechLine.MovemnentDodge:
			result = MovemnentDodge;
			break;
		case TutorialSpeechLine.MovementProceed:
			result = MovementProceed;
			break;
		case TutorialSpeechLine.AvoidClot:
			result = AvoidClot;
			break;
		case TutorialSpeechLine.AvoidEnough:
			result = AvoidEnough;
			break;
		case TutorialSpeechLine.AvoidProceed:
			result = AvoidProceed;
			break;
		case TutorialSpeechLine.ShootPickup:
			result = ShootPickup;
			break;
		case TutorialSpeechLine.ShootSwitch:
			result = ShootSwitch;
			break;
		case TutorialSpeechLine.ShootDummies:
			result = ShootDummies;
			break;
		case TutorialSpeechLine.ShootReload:
			result = ShootReload;
			break;
		case TutorialSpeechLine.ShootProceed:
			result = ShootProceed;
			break;
		case TutorialSpeechLine.AimDescription:
			result = AimDescription;
			break;
		case TutorialSpeechLine.AimFail:
			result = AimFail;
			break;
		case TutorialSpeechLine.AimProceed:
			result = AimProceed;
			break;
		case TutorialSpeechLine.GrenadePickup:
			result = GrenadePickup;
			break;
		case TutorialSpeechLine.GrenadeDescription:
			result = GrenadeDescription;
			break;
		case TutorialSpeechLine.GrenadeProceed:
			result = GrenadeProceed;
			break;
		case TutorialSpeechLine.SyringePickup:
			result = SyringePickup;
			break;
		case TutorialSpeechLine.SyringeDescription:
			result = SyringeDescription;
			break;
		case TutorialSpeechLine.SyringeTargetDescription:
			result = SyringeTargetDescription;
			break;
		case TutorialSpeechLine.SyringeFail:
			result = SyringeFail;
			break;
		case TutorialSpeechLine.SyringeThrowWeapon:
			result = SyringeThrowWeapon;
			break;
		case TutorialSpeechLine.SyringeProceed:
			result = SyringeProceed;
			break;
		case TutorialSpeechLine.TurretDescription:
			result = TurretDescription;
			break;
		case TutorialSpeechLine.TurretIncoming:
			result = TurretIncoming;
			break;
		case TutorialSpeechLine.TurretProceed:
			result = TurretProceed;
			break;
		case TutorialSpeechLine.ShopDescription:
			result = ShopDescription;
			break;
		case TutorialSpeechLine.ShopProceed:
			result = ShopProceed;
			break;
		case TutorialSpeechLine.WavesLastWarning:
			result = WavesLastWarning;
			break;
		case TutorialSpeechLine.WavesIncoming:
			result = WavesIncoming;
			break;
		case TutorialSpeechLine.WavesKillstreak:
			result = WavesKillstreak;
			break;
		case TutorialSpeechLine.WavesCleared:
			result = WavesCleared;
			break;
		case TutorialSpeechLine.WavesDone:
			result = WavesDone;
			break;
		case TutorialSpeechLine.WavesMoreDone:
			result = WavesMoreDone;
			break;
		case TutorialSpeechLine.Win:
			result = Win;
			break;
		case TutorialSpeechLine.Lose:
			result = Lose;
			break;
		}
		return result;
	}
}
