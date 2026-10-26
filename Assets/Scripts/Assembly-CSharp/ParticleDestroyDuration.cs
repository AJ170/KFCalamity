using UnityEngine;

public class ParticleDestroyDuration : MonoBehaviour
{
	private float elapsedTime;

	private void Start()
	{
	}

	private void Update()
	{
		if (base.GetComponent<ParticleSystem>() != null)
		{
			elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, base.GetComponent<ParticleSystem>().duration);
			if (elapsedTime == base.GetComponent<ParticleSystem>().duration)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
