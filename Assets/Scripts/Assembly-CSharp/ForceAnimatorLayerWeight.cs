using System;
using UnityEngine;

public class ForceAnimatorLayerWeight : MonoBehaviour
{
	[Serializable]
	public class AnimatorLayerData
	{
		public int ID;

		public int Weight;
	}

	public AnimatorLayerData[] Layers;

	private void Start()
	{
		Debug.LogError(string.Format("Unit {0} uses depricated class 'ForceAnimatorLayerWeight'.", base.gameObject.name));
	}
}
