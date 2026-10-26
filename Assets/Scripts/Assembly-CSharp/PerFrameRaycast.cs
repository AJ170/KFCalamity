using UnityEngine;

public class PerFrameRaycast : MonoBehaviour
{
	public bool Enabled = true;

	public Transform Bone;

	public float Range = 100f;

	public LayerMask Layer;

	public float Delay = 0.1f;

	private RaycastHit hit;

	private float counter;

	private bool dirty;

	public bool Dirty
	{
		get
		{
			return dirty;
		}
		set
		{
			dirty = value;
		}
	}

	private void Start()
	{
		if (Bone == null)
		{
			Bone = base.transform;
		}
	}

	private void Update()
	{
		if (Enabled)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				hit = default(RaycastHit);
				Physics.Raycast(Bone.position, Bone.forward, out hit, Range, Layer.value);
				dirty = true;
				counter = Delay;
			}
		}
	}

	public RaycastHit GetHitInfo()
	{
		return hit;
	}
}
