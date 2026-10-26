using System;
using UnityEngine;

[Serializable]
public class PlaySoundFromAnimation : MonoBehaviour
{
	public virtual void PlaySound()
	{
		GetComponent<AudioSource>().Play();
	}

	public virtual void Main()
	{
	}
}
