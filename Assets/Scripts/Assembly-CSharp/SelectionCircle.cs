using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
	public GameObject Prefab;

	public GameObject MeleeAimPrefab;

	public Transform ParentBone;

	public Vector3 Offset;

	private GameObject circleInstance;

	private GameObject aimInstance;

	private MeshRenderer meshRenderer;

	private void Start()
	{
		if (ParentBone == null)
		{
			ParentBone = base.transform;
		}
		if (Prefab != null)
		{
			circleInstance = (GameObject)Object.Instantiate(Prefab);
			Transform transform = circleInstance.transform;
			transform.parent = ParentBone;
			transform.localPosition = Offset;
			int num = 0;
			Soldier component = base.gameObject.GetComponent<Soldier>();
			if (component != null)
			{
				num = component.PlayerID;
			}
			SelectionCircleFeedback component2 = circleInstance.GetComponent<SelectionCircleFeedback>();
			if (component2 != null)
			{
				component2.SetPlayerColorByID(num);
			}
			MaterialSelector component3 = circleInstance.GetComponent<MaterialSelector>();
			if (component3 != null)
			{
				component3.SetMaterial(num);
			}
		}
		if (MeleeAimPrefab != null)
		{
			aimInstance = (GameObject)Object.Instantiate(MeleeAimPrefab);
			Transform transform2 = aimInstance.transform;
			transform2.parent = ParentBone;
			transform2.localPosition = Offset;
			transform2.localRotation = Quaternion.Euler(-90f, 0f, 0f);
			meshRenderer = aimInstance.GetComponentInChildren<MeshRenderer>();
			meshRenderer.enabled = false;
			int material = 0;
			Soldier component4 = base.gameObject.GetComponent<Soldier>();
			if (component4 != null)
			{
				material = component4.PlayerID;
			}
			MaterialSelector component5 = transform2.GetComponent<MaterialSelector>();
			if (component5 != null)
			{
				component5.SetMaterial(material);
			}
		}
	}

	public void OnWeaponSwitch(Weapon weapon)
	{
		if ((bool)meshRenderer)
		{
			if (weapon.Type == WeaponType.Melee)
			{
				meshRenderer.enabled = true;
			}
			else
			{
				meshRenderer.enabled = false;
			}
		}
	}
}
