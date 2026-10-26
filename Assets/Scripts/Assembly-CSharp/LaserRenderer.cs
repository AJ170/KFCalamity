using System.Collections;
using UnityEngine;

public class LaserRenderer : MonoBehaviour
{
	public float ScrollSpeed = 0.3f;

	public float PulseSpeed = 1f;

	public float NoiseSize = 0.5f;

	public float MinWidth = 0.3f;

	public float MaxWidth = 0.5f;

	public float animationDirection = 1f;

	private LineRenderer lineRenderer;

	private Material material;

	private float widthMultiplier = 1f;

	private float scrollOffset;

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer != null)
		{
			material = lineRenderer.material;
			material.renderQueue = lineRenderer.material.shader.renderQueue + 1;
		}
	}

	private IEnumerator ChoseNewAnimationTargetCoroutine()
	{
		while (true)
		{
			animationDirection = animationDirection * 0.9f + Random.Range(0.5f, 1.5f) * 0.1f;
			yield return new WaitForSeconds(1f + Random.value * 2f - 1f);
		}
	}

	private void Update()
	{
		if (lineRenderer != null)
		{
			scrollOffset += animationDirection * ScrollSpeed * Time.deltaTime;
			material.SetTextureOffset("_MainTex", new Vector2(scrollOffset, 0f));
			material.SetTextureOffset("_NoiseTex", new Vector2(animationDirection * ScrollSpeed * (0f - Time.time), 0f));
			float num = Mathf.PingPong(Time.time * PulseSpeed, 1f);
			num = (MaxWidth - (MaxWidth - MinWidth) * num) * widthMultiplier;
			lineRenderer.SetWidth(num, num);
		}
	}

	public void SetDistance(float distance)
	{
		if (lineRenderer != null)
		{
			lineRenderer.SetPosition(1, distance * Vector3.forward);
			material.mainTextureScale = new Vector2(distance * 0.1f, 1f);
			material.SetTextureScale("_NoiseTex", new Vector2(distance * 0.1f * NoiseSize, NoiseSize));
		}
	}

	public void SetWidthMultiplier(float multiplier)
	{
		widthMultiplier = multiplier;
	}
}
