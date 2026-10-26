using UnityEngine;

public class LightTween : MonoBehaviour
{
	public float Time = 5f;

	public float TargetIntensity;

	private new Light light;

	private float timer;

	private float startvalue;

	private void Start()
	{
	}

	public void OnEnable()
	{
		if (light == null)
		{
			light = GetComponent<Light>();
			if (light == null)
			{
				light = GetComponentInChildren<Light>();
				if (light == null)
				{
					Debug.LogError("Couldn't find a light on the game object: " + base.name + ". Disabling component");
					base.enabled = false;
				}
			}
		}
		if (light != null)
		{
			startvalue = light.intensity;
			timer = 0f;
		}
	}

	private void Update()
	{
		timer += UnityEngine.Time.deltaTime;
		float num = Mathf.Clamp01(timer / Time);
		light.intensity = Mathf.Lerp(startvalue, TargetIntensity, num);
		if (num >= 1f)
		{
			base.enabled = false;
		}
	}
}
