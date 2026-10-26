using UnityEngine;

public class RagdollSetupHelper : MonoBehaviour
{
	private void Awake()
	{
		DoStuff();
	}

	public void DoStuff()
	{
		RagdollController component = GetComponent<RagdollController>();
		if (component != null)
		{
			component.Head = GetTransform("CHR_Head");
			component.Chest = GetTransform("CHR_Spine3");
			component.Pelvis = GetTransform("CHR_Pelvis");
			component.LeftOverArm = GetTransform("bone_overarm.l");
			component.LeftForeArm = GetTransform("CHR_LArmForeArm");
			component.RightOverArm = GetTransform("bone_overarm.r");
			component.RightForeArm = GetTransform("CHR_RArmForeArm");
			component.LeftThigh = GetTransform("CHR_LCalf");
			component.LeftCalf = GetTransform("bone_leg.l");
			component.RightThigh = GetTransform("CHR_RCalf");
			component.RightCalf = GetTransform("bone_leg.r");
			component.UseLegacyPoseCopy = false;
			return;
		}
		RagdollTargets component2 = GetComponent<RagdollTargets>();
		if (component2 != null)
		{
			component2.Head = GetTransform("CHR_Head");
			component2.Chest = GetTransform("CHR_Spine3");
			component2.Pelvis = GetTransform("CHR_Pelvis");
			component2.LeftOverArm = GetTransform("bone_overarm.l");
			component2.LeftForeArm = GetTransform("CHR_LArmForeArm");
			component2.RightOverArm = GetTransform("bone_overarm.r");
			component2.RightForeArm = GetTransform("CHR_RArmForeArm");
			component2.LeftThigh = GetTransform("CHR_LCalf");
			component2.LeftCalf = GetTransform("bone_leg.l");
			component2.RightThigh = GetTransform("CHR_RCalf");
			component2.RightCalf = GetTransform("bone_leg.r");
		}
	}

	private Transform GetTransform(string name)
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.name == name)
			{
				return componentsInChildren[i];
			}
		}
		Debug.Log("didn't find no " + name);
		return null;
	}

	private Transform GetTransformFromRigidbody(string name)
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.name == name)
			{
				return componentsInChildren[i].transform;
			}
		}
		return null;
	}
}
