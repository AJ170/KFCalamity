using UnityEngine;

public class ObjectStartHealth : MonoBehaviour
{
	public int StartHealth;

	private bool hasRun;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (!hasRun)
		{
			Health component = GetComponent<Health>();
			component.SetHealth(StartHealth);
			hasRun = true;
			Object.Destroy(this);
		}
	}
}
