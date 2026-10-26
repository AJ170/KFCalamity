using UnityEngine;

public class TakeDamageAudio : MonoBehaviour
{
	private enum ClipType
	{
		Small = 0,
		Big = 1
	}

	public AudioSource Audio;

	public AudioClip[] SmallDamageClip;

	public AudioClip[] BigDamageClip;

	public float Volume = 1f;

	public bool LetTimeAffectPitch = true;

	private ClipType lastClipType;

	private void Start()
	{
		GlobalHelpers.AddDamageTakenReceiver(base.gameObject, OnDamageTaken);
	}

	private void OnDamageTaken(Health health, int damageTaken, int rawDamage, float armorTypeMultiplier)
	{
		ClipType clipType = ClipTypeByDamage(damageTaken);
		if (!(Audio != null) || (Audio.isPlaying && (clipType != ClipType.Big || lastClipType != 0)))
		{
			return;
		}
		SetRandomClip(clipType);
		if (Audio.clip != null)
		{
			Audio.volume = Volume * AudioManager.VolumeSetting;
			if (LetTimeAffectPitch)
			{
				Audio.pitch = Time.timeScale;
			}
			Audio.Stop();
			Audio.Play();
		}
	}

	private void SetRandomClip(ClipType type)
	{
		lastClipType = type;
		if (BigDamageClip.Length == 0)
		{
			type = ClipType.Small;
		}
		AudioClip[] array = ((type != 0) ? BigDamageClip : SmallDamageClip);
		if (array.Length > 0)
		{
			Audio.clip = array[Random.Range(0, array.Length)];
		}
	}

	private ClipType ClipTypeByDamage(int damage)
	{
		return (damage >= 25) ? ClipType.Big : ClipType.Small;
	}
}
