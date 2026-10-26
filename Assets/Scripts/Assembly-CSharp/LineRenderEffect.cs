using EffectEvents;
using EventHelpers;
using UnityEngine;

public class LineRenderEffect : EffectBaseListener
{
	public GameObject LinePrefab;

	public float Duration;

	private GameObject lineInstance;

	private Transform lineTransform;

	private LineRenderer lineRenderer;

	private float counter;

	protected override void Start()
	{
		if (LinePrefab != null)
		{
			lineInstance = (GameObject)Object.Instantiate(LinePrefab);
			lineInstance.SetActive(false);
			lineInstance.hideFlags = HideFlags.HideInHierarchy;
			lineTransform = lineInstance.transform;
			lineRenderer = lineInstance.GetComponent<LineRenderer>();
			base.Start();
		}
		else
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				lineInstance.SetActive(false);
			}
		}
	}

	private void OnDestroy()
	{
		if (lineInstance != null)
		{
			Object.Destroy(lineInstance);
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && lineRenderer != null && e.Id == TriggerId.Projectile)
		{
			Projectile projectile = (Projectile)e;
			Vector3 position = projectile.Bone.position;
			Vector3 target = projectile.Target;
			float distance = Vector3.Distance(position, target);
			DisplayLine(position, projectile.Direction, distance);
		}
	}

	private void DisplayLine(Vector3 source, Vector3 direction, float distance)
	{
		lineInstance.SetActive(true);
		lineTransform.position = source;
		lineTransform.rotation = Quaternion.LookRotation(direction);
		lineRenderer.SetPosition(1, distance * Vector3.forward);
		counter = Duration;
	}
}
