using UnityEngine;

public class TutorialSetStartAnimation : MonoBehaviour
{
	private PlayerWeaponController pwc;

	private Animator anim;

	private bool isKnifeAnim;

	private void Start()
	{
		anim = GetComponent<Animator>();
		pwc = base.transform.parent.GetComponent<PlayerWeaponController>();
		anim.SetBool("eqKnife", true);
		isKnifeAnim = true;
	}

	private void Update()
	{
		if (pwc.ActiveWeapon == null && !isKnifeAnim)
		{
			anim.SetBool("eqKnife", true);
			isKnifeAnim = true;
		}
		else if (pwc.ActiveWeapon != null && isKnifeAnim)
		{
			anim.SetBool("eqKnife", false);
			isKnifeAnim = false;
		}
	}
}
