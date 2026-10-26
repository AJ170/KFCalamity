using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
	public List<ParticleSystem> Systems;

	public bool OnlyPlayFirstSystem = true;

	public void Play()
	{
		if (Systems.Count > 0)
		{
			if (OnlyPlayFirstSystem)
			{
				Systems[0].Play();
			}
			else
			{
				PlayAll();
			}
		}
	}

	public void Reset()
	{
		if (Systems.Count > 0)
		{
			if (OnlyPlayFirstSystem)
			{
				Systems[0].Clear();
			}
			else
			{
				ClearAll();
			}
		}
	}

	public void PlayAll()
	{
		foreach (ParticleSystem system in Systems)
		{
			system.Play();
		}
	}

	public void ClearAll()
	{
		foreach (ParticleSystem system in Systems)
		{
			system.Clear();
		}
	}

	public void SetColor(Color color)
	{
		foreach (ParticleSystem system in Systems)
		{
			system.startColor = color;
		}
	}
}
