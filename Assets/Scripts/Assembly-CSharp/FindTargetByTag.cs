using UnityEngine;

public class FindTargetByTag : MonoBehaviour
{
	public string Tag;

	public float SearchFrequency = 3f;

	private CameraScript cameraScript;

	private float searchCooldown;

	private void Start()
	{
		cameraScript = GetComponent<CameraScript>();
	}

	private void Update()
	{
		if (!(cameraScript.Target == null) || string.IsNullOrEmpty(Tag))
		{
			return;
		}
		searchCooldown -= Time.deltaTime;
		if (searchCooldown <= 0f)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag(Tag);
			if (gameObject != null)
			{
				cameraScript.Target = gameObject.transform;
			}
			searchCooldown = 1f / SearchFrequency;
		}
	}
}
