using UnityEngine;

[RequireComponent(typeof(Animation))]
public class MenuDoorScript : MonoBehaviour
{
	public Transform Door1;

	public Transform Door2;

	private Animation firstDoorAnim;

	private Animation secondDoorAnim;

	private bool initialized;

	private void Start()
	{
		initialized = true;
		if (Door1 != null)
		{
			firstDoorAnim = Door1.GetComponent<Animation>();
		}
		else
		{
			initialized = false;
		}
		if (Door2 != null)
		{
			secondDoorAnim = Door2.GetComponent<Animation>();
		}
		else
		{
			initialized = false;
		}
	}

	public void Open()
	{
		if (initialized)
		{
			firstDoorAnim.Play();
			secondDoorAnim.Play();
		}
	}
}
