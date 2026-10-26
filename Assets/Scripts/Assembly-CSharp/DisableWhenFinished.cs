using UnityEngine;

public class DisableWhenFinished : MonoBehaviour
{
	public AudioSource AudioSource;

	public ParticleSystem ParticleSystem;

	public bool DestroyObject;

	private void Start()
	{
		if (AudioSource == null && ParticleSystem == null)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		bool flag = false;
		if (AudioSource != null)
		{
			flag = AudioSource.isPlaying;
		}
		if (!flag && ParticleSystem != null)
		{
			flag = ParticleSystem.isPlaying;
		}
		if (!flag)
		{
			if (DestroyObject)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
