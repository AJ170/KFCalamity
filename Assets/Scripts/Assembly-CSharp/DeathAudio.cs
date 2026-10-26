using UnityEngine;

public class DeathAudio : MonoBehaviour
{
	public AudioClip[] DeathClips;

	public AudioClip[] DeathByHeadshotClips;

	public float Volume = 1f;

	public bool LetTimeAffectPitch = true;

	private void Start()
	{
		GlobalHelpers.AddDeathReceiver(base.gameObject, OnDeath);
	}

	private void OnDeath(Health health)
	{
		AudioClip[] array = DeathClips;
		if (health.KilledByHeadshot && DeathByHeadshotClips.Length > 0)
		{
			array = DeathByHeadshotClips;
		}
		if (array.Length > 0)
		{
			AudioManager.PlayClipAtPoint(array[Random.Range(0, array.Length)], base.transform.position, Volume, (!LetTimeAffectPitch) ? 1f : Time.timeScale);
		}
	}
}
