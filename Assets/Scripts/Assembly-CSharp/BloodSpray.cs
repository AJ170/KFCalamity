using UnityEngine;

public class BloodSpray : MonoBehaviour
{
	public GameObject EffectPrefab;

	public Transform EmitterTransform;

	private Transform particleTransform;

	private new ParticleSystem particleSystem;

	private void Start()
	{
		if (EmitterTransform == null)
		{
			EmitterTransform = base.transform;
		}
		GameObject gameObject = (GameObject)Object.Instantiate(EffectPrefab, Vector3.zero, Quaternion.identity);
		particleTransform = gameObject.transform;
		particleTransform.parent = EmitterTransform;
		particleTransform.localPosition = Vector3.zero;
		particleSystem = gameObject.GetComponent<ParticleSystem>();
	}

	private void Update()
	{
	}

	private void TriggerBloodSpray(Vector3 direction)
	{
		particleSystem.Play();
	}
}
