using UnityEngine.AI;
using System;
using UnityEngine;

public class WeaponPickup : Pickup
{
	public AudioClip SpawnSound;

	public AudioClip HitGroundSound;

	public GameObject WeaponPrefab;

	public bool FallInArc = true;

	private LerpVector3 lerpToBounds = new LerpVector3();

	private GameObject lastOwner;

	private StoredWeaponData weaponData;

	private int lastLayer;

	public GameObject LastOwner
	{
		get
		{
			return lastOwner;
		}
		set
		{
			lastOwner = value;
		}
	}

	public StoredWeaponData WeaponData
	{
		get
		{
			return weaponData;
		}
		set
		{
			weaponData = value;
		}
	}

	private void Awake()
	{
		type = PickupType.Weapon;
		if (WeaponPrefab == null)
		{
			Debug.LogError(string.Format("WeaponPickup:Awake() No weapon specified for {0}", base.gameObject.name));
		}
		lastLayer = base.gameObject.layer;
		base.gameObject.layer = LayerMask.NameToLayer("Boundries");
	}

	private void Start()
	{
		Vector3 position = base.transform.position;
		if (!IsOnNavMesh(base.transform.position))
		{
			Vector3? vector = FindClosestNavMeshPoint(position);
			if (vector.HasValue)
			{
				StartLerpToBounds(position, vector.Value, 1.5f);
			}
			else
			{
				deleted = true;
			}
		}
		else if (FallInArc)
		{
			PickupSpawnHelper.Arc(base.gameObject, 3f);
			sampleNavMeshForGroundCollision = true;
		}
		PlaySpawnSound();
	}

	protected override void OnUpdate()
	{
		UpdateLerpToBounds();
	}

	protected override void OnCollideGround()
	{
		PlayHitGroundSound();
		base.GetComponent<Collider>().isTrigger = true;
		base.GetComponent<Rigidbody>().isKinematic = true;
		base.gameObject.layer = lastLayer;
	}

	protected override void OnCollidePlayer(Collider collider)
	{
		if (WeaponPrefab == null)
		{
			return;
		}
		GameObject gameObject = collider.gameObject;
		PlayerWeaponController playerWeaponController = WeaponController(gameObject);
		if (playerWeaponController != null)
		{
			Weapon weapon = WeaponByWeaponPrefab(WeaponPrefab);
			if (weapon != null)
			{
				InventoryWeaponSlot inventoryWeaponSlot = AvailableInventorySlot(playerWeaponController, weapon.Slot);
				if (inventoryWeaponSlot != InventoryWeaponSlot.None && !playerWeaponController.HasWeapon(weapon.Name))
				{
					playerWeaponController.AddWeaponToSlot(WeaponPrefab, inventoryWeaponSlot, WeaponData);
					if (playerWeaponController.ShouldAutoEquipWeapon())
					{
						playerWeaponController.SetActiveWeapon(inventoryWeaponSlot);
					}
					OnWeaponPickedUp(gameObject, weapon);
				}
				else if (TryRefillAmmo(playerWeaponController))
				{
					OnWeaponPickedUp(gameObject, weapon);
				}
				else
				{
					OnFailedPickup(gameObject);
				}
			}
			else
			{
				Debug.LogError(string.Format("WeaponPickup:OnCollidePlayer() -> Didn't find Weapon component in {0}", WeaponPrefab.name));
			}
		}
		else
		{
			Debug.LogError(string.Format("WeaponPickup:OnCollidePlayer() -> Didn't find PlayerWeaponController in {0}", gameObject.name));
		}
	}

	private void OnWeaponPickedUp(GameObject player, Weapon weaponTemplate)
	{
		if (player != null && weaponTemplate != null)
		{
			PlayPickupSound();
			PlayPickupParticle();
			PlayerGUILink playerGUILink = GUILinkByObject(player);
			if (playerGUILink != null)
			{
				playerGUILink.OnWeaponPickup(weaponTemplate);
			}
			if (lastOwner != player)
			{
				SpeechManager.PlaySpeech(SpeechManager.SpeechLine.WeaponPickedUp, player);
			}
		}
		deleted = true;
	}

	private Vector3? FindClosestNavMeshPoint(Vector3 position, float radius = 8f)
	{
		Vector3? result = null;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(position, out hit, radius, 1))
		{
			result = hit.position;
		}
		return result;
	}

	private void StartLerpToBounds(Vector3 start, Vector3 end, float time)
	{
		base.GetComponent<Collider>().enabled = false;
		lerpToBounds.Lerp(start, end, 1f / time, null);
	}

	private void UpdateLerpToBounds()
	{
		if (lerpToBounds.Lerping)
		{
			lerpToBounds.Update();
			float progress = lerpToBounds.Progress;
			Vector3 current = lerpToBounds.Current;
			current.y += 2.5f * Mathf.Sin((float)Math.PI * progress);
			base.transform.position = current;
			if (progress == 1f)
			{
				base.GetComponent<Collider>().enabled = true;
				sampleNavMeshForGroundCollision = true;
			}
		}
	}

	protected void PlaySpawnSound()
	{
		if (SpawnSound != null)
		{
			PlayAudioClip(SpawnSound);
		}
	}

	protected void PlayHitGroundSound()
	{
		if (HitGroundSound != null)
		{
			PlayAudioClip(HitGroundSound);
		}
	}

	private PlayerWeaponController WeaponController(GameObject go)
	{
		return go.GetComponent<PlayerWeaponController>();
	}

	private InventoryWeaponSlot AvailableInventorySlot(PlayerWeaponController weaponController, WeaponSlot weaponSlot)
	{
		if (weaponController != null && weaponController.IsSlotAvailable(weaponSlot))
		{
			InventoryWeaponSlot result = weaponController.WeaponSlotToInventorySlot(weaponSlot);
			if (weaponSlot == WeaponSlot.Primary)
			{
				result = ((!weaponController.IsSlotAvailable(InventoryWeaponSlot.Primary1)) ? InventoryWeaponSlot.Primary2 : InventoryWeaponSlot.Primary1);
			}
			return result;
		}
		return InventoryWeaponSlot.None;
	}

	private bool TryRefillAmmo(PlayerWeaponController weaponController)
	{
		if (weaponController != null)
		{
			InventoryWeaponSlot inventorySlot = weaponController.GetInventorySlot(WeaponPrefab);
			if (inventorySlot != InventoryWeaponSlot.None && !weaponController.AmmunitionInSlotFull(inventorySlot))
			{
				weaponController.RefillAmmunitionBoxForSlot(inventorySlot);
				return true;
			}
		}
		return false;
	}

	private Weapon WeaponByWeaponPrefab(GameObject weaponPrefab)
	{
		if (weaponPrefab != null)
		{
			return weaponPrefab.GetComponent<Weapon>();
		}
		return null;
	}
}
