using UnityEngine;

public class ParticleManager : MonoBehaviour
{
	private static ParticleManager instance;

	public GameObject[] ParticleDatabase;

	private GameObject particlesGroup;

	public static ParticleManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Start()
	{
		instance = this;
		CreateGroup();
	}

	private void CreateGroup()
	{
		particlesGroup = new GameObject("Particles");
		particlesGroup.transform.parent = base.transform;
		particlesGroup.transform.localPosition = Vector3.zero;
		particlesGroup.transform.localRotation = Quaternion.identity;
	}

	private GameObject FindParticleInDatabase(string name)
	{
		GameObject[] particleDatabase = ParticleDatabase;
		foreach (GameObject gameObject in particleDatabase)
		{
			if (gameObject != null && name == gameObject.name)
			{
				return gameObject;
			}
		}
		Debug.LogError("ParticleManager:FindParticleInDatabase -> Failed to find '" + name + "' in database");
		return null;
	}

	public GameObject CreateParticle(string name, Vector3 position, Quaternion rotation, ParticleDestroyMethod destroyMethod = ParticleDestroyMethod.Manually)
	{
		GameObject gameObject = FindParticleInDatabase(name);
		if (gameObject != null)
		{
			GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject, position, rotation);
			if (gameObject2.GetComponent<ParticleSystem>() != null)
			{
				gameObject2.transform.parent = particlesGroup.transform;
				if (destroyMethod == ParticleDestroyMethod.Duration)
				{
					gameObject2.AddComponent<ParticleDestroyDuration>();
				}
				return gameObject2;
			}
			Debug.LogError(string.Format("ParticleManager:CreateParticle() Didn't find ParticleSystem in {0}", gameObject2.name));
			Object.Destroy(gameObject2);
			return null;
		}
		return null;
	}
}
