using UnityEngine;

public class ToggleBloom : MonoBehaviour
{
	public bool Toggle = true;

	private void Awake()
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		FastBloom component = gameObject.GetComponent<FastBloom>();
		component.enabled = Toggle;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
