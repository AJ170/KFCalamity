using UnityEngine;

public class BulletManager : UnitPool
{
	private static BulletManager instance;

	public string DefaultBulletName;

	public static BulletManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}
		Debug.LogWarning("[BulletManager] Multiple bullet managers detected!");
		Object.Destroy(base.gameObject);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (instance == this)
		{
			instance = null;
		}
	}

	public void SpawnBullet(Vector3 spawnPosition, Quaternion spawnRotation, Vector3 targetPosition)
	{
		GameObject unit = GetUnit(DefaultBulletName);
		if (unit != null)
		{
			Transform transform = unit.transform;
			transform.position = spawnPosition;
			transform.rotation = spawnRotation;
			Bullet component = unit.GetComponent<Bullet>();
			if (component != null)
			{
				component.SetMaxDistance(Vector3.Distance(spawnPosition, targetPosition));
				component.Fire();
			}
		}
		else
		{
			Debug.Log("[BulletManager:SpawnBullet] Failed to spawn bullet: out of instances.");
		}
	}
}
