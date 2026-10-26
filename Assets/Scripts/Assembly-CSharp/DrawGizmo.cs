using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
	public string Texture;

	public bool Enabled = true;

	public bool OnlyWhenSelected;

	private void OnDrawGizmos()
	{
		if (Enabled && !OnlyWhenSelected && !string.IsNullOrEmpty(Texture))
		{
			Gizmos.DrawIcon(base.transform.position, Texture);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Enabled && OnlyWhenSelected && !string.IsNullOrEmpty(Texture))
		{
			Gizmos.DrawIcon(base.transform.position, Texture);
		}
	}
}
