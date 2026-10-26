using UnityEngine;

internal class TronTrailSectionCS
{
	public Vector3 Pos;

	public Vector3 Pos2;

	public Vector3 UpDir;

	public float Time;

	public TronTrailSectionCS(Vector3 pos, Vector3 pos2, Vector3 upDir, float time)
	{
		Pos = pos;
		Pos2 = pos2;
		UpDir = upDir;
		Time = time;
	}
}
