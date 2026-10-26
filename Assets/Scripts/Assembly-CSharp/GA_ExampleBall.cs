using UnityEngine;

public class GA_ExampleBall : MonoBehaviour
{
	public float Speed = 1f;

	private void Start()
	{
		base.GetComponent<Rigidbody>().velocity = new Vector3(Random.value * 0.2f - 0.1f, -1f, 0f) * Speed;
	}

	private void Update()
	{
		base.GetComponent<Rigidbody>().AddForce(Vector3.down * 0.0001f);
		base.GetComponent<Rigidbody>().velocity = base.GetComponent<Rigidbody>().velocity.normalized * Speed;
	}
}
