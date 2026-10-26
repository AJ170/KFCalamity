using UnityEngine;

public class SphereCollisionBase : EffectBaseListener
{
	public string Name;

	public float Radius = 2f;

	public LayerMask Layer;

	public LayerMask HitLayer = 2816;

	public bool CanHitSource;

	private GameObject root;

	private GameObject source;

	private int sourceLayer = -1;

	private static int playerLayer;

	protected bool friendlyFire;

	protected override void Start()
	{
		base.Start();
		root = base.transform.root.gameObject;
		if (source == null)
		{
			SetSource(root);
		}
		if (playerLayer == 0)
		{
			playerLayer = LayerMask.NameToLayer("Player");
		}
	}

	private void OnMounted(GameObject parent)
	{
		SetSource(parent);
	}

	private void SetSource(GameObject source)
	{
		if (source != null)
		{
			this.source = StandaloneEntity.Source(source);
			sourceLayer = this.source.layer;
		}
	}

	protected bool IsColliderAccepted(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		bool flag = true;
		if (source != root && gameObject == root)
		{
			flag = false;
		}
		if (flag && !CanHitSource && gameObject == source)
		{
			flag = false;
		}
		if (flag && !friendlyFire && sourceLayer == playerLayer && source != gameObject && gameObject.layer == playerLayer)
		{
			flag = false;
		}
		return flag;
	}

	protected bool HitLayerMatch(int layer)
	{
		return (HitLayer.value & (1 << layer)) != 0;
	}
}
