using UnityEngine;

public class KillTrigger : MonoBehaviour
{
	private void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.gameObject != null)
		{
			KillUnit(collisionInfo.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject != null)
		{
			KillUnit(collider.gameObject);
		}
	}

	private void KillUnit(GameObject go)
	{
		Health component = go.GetComponent<Health>();
		if (component != null)
		{
			component.Kill();
		}
		else
		{
			Object.Destroy(go);
		}
	}
}
