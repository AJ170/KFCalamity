using UnityEngine;

public class GA_ExampleWall : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<Renderer>().material.color = Color.gray;
	}
}
