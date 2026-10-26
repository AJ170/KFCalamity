using UnityEngine;

[RequireComponent(typeof(PerFrameRaycast))]
public class LaserSight : MonoBehaviour
{
	public Transform Muzzle;

	public GameObject LaserPrefab;

	public GameObject DotPrefab;

	public float MaxDistance = 100f;

	private GameObject laserInstance;

	private PerFrameRaycast raycast;

	private LaserRenderer laserRenderer;

	private Vector3 lastHitPosition = Vector3.zero;

	private float lastDistance;

	private void Start()
	{
		if (Muzzle == null)
		{
			Muzzle = base.transform;
		}
		raycast = GetComponent<PerFrameRaycast>();
	}

	public void SpawnLaser(int id)
	{
		if (laserInstance == null)
		{
			if (!(LaserPrefab != null))
			{
				base.enabled = false;
				return;
			}
			laserInstance = (GameObject)Object.Instantiate(LaserPrefab);
			laserInstance.transform.parent = Muzzle;
			laserInstance.transform.localPosition = Vector3.zero;
			laserInstance.transform.localRotation = Quaternion.identity;
			laserRenderer = laserInstance.GetComponent<LaserRenderer>();
		}
		MaterialSelector component = laserInstance.GetComponent<MaterialSelector>();
		if (component != null)
		{
			component.SetMaterial(id);
		}
	}

	private void OnEnable()
	{
		if (laserInstance != null)
		{
			laserInstance.SetActive(true);
		}
	}

	private void OnDisable()
	{
		if (laserInstance != null)
		{
			laserInstance.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (laserInstance != null)
		{
			Object.Destroy(laserInstance);
		}
	}

	private void Update()
	{
		if (laserInstance != null)
		{
			RaycastHit hitInfo = raycast.GetHitInfo();
			float num;
			if (hitInfo.transform != null)
			{
				lastHitPosition = hitInfo.point;
				num = Vector3.Distance(Muzzle.position, lastHitPosition);
			}
			else
			{
				lastHitPosition = Vector3.zero;
				num = MaxDistance;
			}
			if (laserRenderer != null && num != lastDistance)
			{
				laserRenderer.SetDistance(num);
				lastDistance = num;
			}
		}
	}

	public void SetWidthMultiplier(float multiplier)
	{
		if (laserRenderer != null)
		{
			laserRenderer.SetWidthMultiplier(multiplier);
		}
	}
}
