using UnityEngine;

public class OldWeaponController : MonoBehaviour
{
	public GameObject DefaultWeapon;

	public Transform WeaponBone;

	private GameObject activeWeapon;

	private Weapon weaponStats;

	private float weaponCooldown;

	private bool canAttack;

	public Weapon WeaponStats
	{
		get
		{
			return weaponStats;
		}
	}

	private void Start()
	{
		if (WeaponBone == null)
		{
			WeaponBone = base.transform;
		}
		if ((bool)DefaultWeapon)
		{
			SetActiveWeapon(DefaultWeapon);
		}
	}

	private void Update()
	{
		UpdateWeaponCooldown();
	}

	public void SetActiveWeapon(GameObject weaponPrefab)
	{
		if (activeWeapon != null)
		{
			Object.Destroy(activeWeapon);
			activeWeapon = null;
			weaponStats = null;
		}
		activeWeapon = (GameObject)Object.Instantiate(weaponPrefab);
		weaponStats = activeWeapon.GetComponent<Weapon>();
		activeWeapon.transform.parent = WeaponBone;
		activeWeapon.transform.localPosition = Vector3.zero;
		activeWeapon.BroadcastMessage("OnMounted", base.gameObject, SendMessageOptions.DontRequireReceiver);
		if (weaponStats == null)
		{
			Debug.LogError("Missing Weapon component on weapon " + activeWeapon.name);
		}
		ResetStats();
	}

	public void Attack()
	{
		if (CanAttack())
		{
			PerformAttack();
		}
	}

	public void ForceAttack()
	{
		PerformAttack();
	}

	private void PerformAttack()
	{
		activeWeapon.GetComponent<Weapon>().Attack();
		canAttack = false;
		weaponCooldown = 1f;
	}

	public bool CanAttack()
	{
		return activeWeapon != null && canAttack;
	}

	private void UpdateWeaponCooldown()
	{
		if (weaponCooldown > 0f)
		{
			weaponCooldown -= Time.deltaTime;
			if (weaponCooldown <= 0f)
			{
				canAttack = true;
			}
		}
	}

	private void ResetStats()
	{
		weaponCooldown = 0f;
		canAttack = true;
	}
}
