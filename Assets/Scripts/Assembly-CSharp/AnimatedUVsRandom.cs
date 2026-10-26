using UnityEngine;

public class AnimatedUVsRandom : MonoBehaviour
{
	public int materialIndex;

	public string textureName = "_MainTex";

	public float delay;

	private Vector2 uvOffset = Vector2.zero;

	private float counter;

	private void LateUpdate()
	{
		counter -= Time.deltaTime;
		if (counter <= 0f)
		{
			uvOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
			if (base.GetComponent<Renderer>().enabled)
			{
				base.GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
			}
			counter = delay;
		}
	}
}
