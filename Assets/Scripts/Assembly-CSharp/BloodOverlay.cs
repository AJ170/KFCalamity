using UnityEngine;

public class BloodOverlay : MonoBehaviour
{
	public Renderer Renderer;

	public float BlinkTime = 0.1f;

	public Color HitColor;

	private Material material;

	private float timer;

	private Color defaultColor;

	private void Start()
	{
		if (Renderer != null)
		{
			material = Renderer.material;
		}
		else
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
	}

	public void OnHit(float healthFraction)
	{
		if (base.enabled)
		{
			material.SetFloat("_BloodAmount", healthFraction);
		}
	}
}
