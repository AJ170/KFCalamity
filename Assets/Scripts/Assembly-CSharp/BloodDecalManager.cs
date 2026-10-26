using System.Collections.Generic;
using UnityEngine;

public class BloodDecalManager : MonoBehaviour
{
	private static BloodDecalManager instance;

	public float BloodDecalRange = 1.5f;

	private List<GameObject> bloodDecals = new List<GameObject>();

	private float bloodDecalRangeSqr;

	public static BloodDecalManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		bloodDecalRangeSqr = BloodDecalRange * BloodDecalRange;
	}

	public void RegisterBloodDecal(GameObject blood)
	{
		if (!bloodDecals.Contains(blood))
		{
			bloodDecals.Add(blood);
		}
	}

	public bool IsPositionClear(Vector3 position)
	{
		foreach (GameObject bloodDecal in bloodDecals)
		{
			float sqrMagnitude = (position - bloodDecal.transform.position).sqrMagnitude;
			if (sqrMagnitude < bloodDecalRangeSqr)
			{
				return false;
			}
		}
		return true;
	}
}
