using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
	public GameObject AmmunitionPickup;

	public GameObject SyringePickup;

	public GameObject ArmorPickup;

	public GameObject MoneyPickup;

	public GameObject[] BonusPickups;

	public GameObject[] WeaponPickups;

	public float ArmorPickupChance = 0.2f;

	public int AllowedMoneyPickups = 10;

	public PickupLevelRestriction[] LevelRestrictions;

	public static PickupManager instance;

	private GameObject pickupsContainer;

	private PickupSystemMode mode;

	private List<Pickup> pickups = new List<Pickup>();

	private List<PickupSpawnPoint> busySpawnPoints = new List<PickupSpawnPoint>();

	public static PickupManager Instance
	{
		get
		{
			return instance;
		}
	}

	public PickupSystemMode Mode
	{
		get
		{
			return mode;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void Update()
	{
		RemoveDeletedPickups();
		UpdatePickups();
	}

	private void UpdatePickups()
	{
		foreach (Pickup pickup in pickups)
		{
			if (pickup.WantsNewSpawnLocation)
			{
				PickupSpawnPoint spawnPoint = pickup.SpawnPoint;
				PickupSpawnPoint pickupSpawnPoint = RandomAvailableSpawnPoint(pickup.Type);
				if (pickup.SetLocation(pickupSpawnPoint))
				{
					FreeBusySpawnPoint(spawnPoint);
					AddBusySpawnPoint(pickupSpawnPoint);
				}
			}
			pickup.UpdateRespawnTimer();
		}
	}

	public void AddPickup(Pickup pickup, PickupSpawnPoint spawnPoint)
	{
		if (pickup != null)
		{
			if (pickupsContainer == null)
			{
				FindContainer();
			}
			AddBusySpawnPoint(spawnPoint);
			pickup.transform.parent = pickupsContainer.transform;
			pickups.Add(pickup);
		}
	}

	public void AddPickup(Pickup pickup)
	{
		AddPickup(pickup, null);
	}

	private void RemovePickup(Pickup pickup)
	{
		if (pickup != null)
		{
			FreeBusySpawnPoint(pickup.SpawnPoint);
			pickup.Shutdown();
			pickups.Remove(pickup);
		}
	}

	private void RemoveDeletedPickups()
	{
		int count = pickups.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			Pickup pickup = pickups[num];
			if (pickup.IsDeleted)
			{
				RemovePickup(pickup);
			}
		}
	}

	private void AddBusySpawnPoint(PickupSpawnPoint spawnPoint)
	{
		if (spawnPoint != null)
		{
			busySpawnPoints.Add(spawnPoint);
		}
	}

	private void FreeBusySpawnPoint(PickupSpawnPoint spawnPoint)
	{
		if (spawnPoint != null)
		{
			busySpawnPoints.Remove(spawnPoint);
		}
	}

	private bool IsSpawnPointBusy(PickupSpawnPoint spawnPoint)
	{
		return busySpawnPoints.Contains(spawnPoint);
	}

	private PickupSpawnPoint RandomAvailableSpawnPoint(PickupType type)
	{
		List<PickupSpawnPoint> list = PickupSpawnPointByPickupType(type, null);
		list.QuickShuffle();
		foreach (PickupSpawnPoint item in list)
		{
			if (!IsSpawnPointBusy(item))
			{
				return item;
			}
		}
		return null;
	}

	private List<PickupSpawnPoint> PickupSpawnPointByPickupType(PickupType type, GameObject[] spawnPoints)
	{
		List<PickupSpawnPoint> list = new List<PickupSpawnPoint>();
		if (spawnPoints == null)
		{
			spawnPoints = GameObject.FindGameObjectsWithTag("PickupSpawnPoint");
		}
		if (spawnPoints != null)
		{
			PickupSpawnPoint pickupSpawnPoint = null;
			GameObject[] array = spawnPoints;
			foreach (GameObject gameObject in array)
			{
				pickupSpawnPoint = gameObject.GetComponent<PickupSpawnPoint>();
				if (pickupSpawnPoint != null && pickupSpawnPoint.Type == type)
				{
					list.Add(pickupSpawnPoint);
				}
			}
		}
		return list;
	}

	public void SetupStaticPickups()
	{
		if (mode != 0)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("PickupSpawnPoint");
		if (array != null)
		{
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				PickupSpawnPoint component = gameObject.GetComponent<PickupSpawnPoint>();
				if (component != null)
				{
					CreatePickup(component);
				}
			}
		}
		mode = PickupSystemMode.Static;
	}

	public void SetupDynamicPickups()
	{
		if (mode == PickupSystemMode.None)
		{
			GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("PickupSpawnPoint");
			List<PickupSpawnPoint> pickupPoints = PickupSpawnPointByPickupType(PickupType.Ammo, spawnPoints);
			List<PickupSpawnPoint> pickupPoints2 = PickupSpawnPointByPickupType(PickupType.Syringe, spawnPoints);
			List<PickupSpawnPoint> pickupPoints3 = PickupSpawnPointByPickupType(PickupType.Bonus, spawnPoints);
			int numAmmo;
			int numSyringes;
			int numBonuses;
			AllowedDynamicPickups(out numAmmo, out numSyringes, out numBonuses);
			SpawnDynamicPickups(pickupPoints, numAmmo);
			SpawnDynamicPickups(pickupPoints2, numSyringes);
			SpawnDynamicPickups(pickupPoints3, numBonuses);
			mode = PickupSystemMode.Dynamic;
		}
	}

	private void AllowedDynamicPickups(out int numAmmo, out int numSyringes, out int numBonuses)
	{
		int numberOfPlayers = PlayerManager.Instance.NumberOfPlayers;
		string levelName = GameState.Instance.GameSettings.LevelName;
		PickupLevelRestriction pickupLevelRestriction = FindLevelRestriction(levelName);
		numAmmo = 8;
		numSyringes = 5;
		numBonuses = 1;
		if (pickupLevelRestriction != null)
		{
			numAmmo = pickupLevelRestriction.Ammo;
			numSyringes = pickupLevelRestriction.Syringes;
			numBonuses = pickupLevelRestriction.Bonuses;
		}
		numAmmo += ExtraAmmoPickupsByNumPlayers(numberOfPlayers);
		numSyringes += ExtraSyringePickupsByNumPlayers(numSyringes);
	}

	private void SpawnDynamicPickups(List<PickupSpawnPoint> pickupPoints, int numPickupsToSpawn)
	{
		if (pickupPoints == null || pickupPoints.Count <= 0 || numPickupsToSpawn <= 0)
		{
			return;
		}
		numPickupsToSpawn = Mathf.Clamp(numPickupsToSpawn, 1, pickupPoints.Count);
		pickupPoints.Shuffle();
		PickupSpawnPoint pickupSpawnPoint = null;
		for (int i = 0; i < numPickupsToSpawn; i++)
		{
			pickupSpawnPoint = pickupPoints[i];
			if (pickupSpawnPoint != null)
			{
				CreatePickup(pickupSpawnPoint, true);
			}
		}
	}

	private Pickup PickupFromPool(GameObject poolPrefab)
	{
		Pickup pickup = null;
		if (poolPrefab != null)
		{
			GameObject effect = EffectPool.GetEffect(poolPrefab);
			if (effect != null)
			{
				pickup = effect.GetComponent<Pickup>();
				if (pickup != null)
				{
					pickup.gameObject.SetActive(true);
					pickup.PooledPickup = true;
				}
			}
			else
			{
				Debug.LogWarning(string.Format("PickupManager:PickupFromPool() Failed to locate {0} in pool", poolPrefab.name));
			}
		}
		return pickup;
	}

	private int ExtraAmmoPickupsByNumPlayers(int numPlayers)
	{
		Mathf.Clamp(numPlayers, 1, 4);
		return numPlayers * 2 - 2;
	}

	private int ExtraSyringePickupsByNumPlayers(int numPlayers)
	{
		Mathf.Clamp(numPlayers, 1, 4);
		return numPlayers * 1 - 1;
	}

	private Pickup CreatePickup(PickupSpawnPoint spawnPoint, bool usePool = false)
	{
		Pickup pickup = null;
		GameObject gameObject = null;
		PickupType pickupType = spawnPoint.Type;
		if (pickupType == PickupType.Syringe && CheckArmorPickupChance())
		{
			pickupType = PickupType.Armor;
		}
		gameObject = PickupTemplateByPickupType(pickupType);
		if (gameObject != null)
		{
			if (usePool)
			{
				pickup = PickupFromPool(gameObject);
			}
			if (pickup == null)
			{
				GameObject gameObject2 = CloneTemplate(gameObject);
				pickup = gameObject2.GetComponent<Pickup>();
			}
		}
		if (pickup != null)
		{
			pickup.SetLocation(spawnPoint);
			pickup.SetRotation(gameObject.transform.rotation);
			pickup.RespawnTime = spawnPoint.RespawnTime;
			AddPickup(pickup, spawnPoint);
		}
		return pickup;
	}

	public void DeleteAllActivePickups()
	{
		foreach (Pickup pickup in pickups)
		{
			pickup.ForceDelete();
		}
	}

	public void DeleteActivePickupsByPickupType(PickupType type)
	{
		foreach (Pickup pickup in pickups)
		{
			if (pickup.Type == type)
			{
				pickup.ForceDelete();
			}
		}
	}

	public void ForceWeaponCollisionCheck(Transform soldier, float radius = 0.6f)
	{
		int layerMask = 1 << LayerMask.NameToLayer("Pickup");
		Collider[] array = Physics.OverlapSphere(soldier.position, radius, layerMask);
		if (array.Length <= 0)
		{
			return;
		}
		Pickup pickup = null;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			pickup = collider.gameObject.GetComponent<Pickup>();
			if ((bool)pickup && pickup.Type == PickupType.Weapon)
			{
				pickup.ForceCollidePlayer(soldier.GetComponent<Collider>());
				break;
			}
		}
	}

	public WeaponPickup CreateWeaponPickup(GameObject weaponPickupPrefab, Vector3 position)
	{
		if (WeaponPickupMatch(weaponPickupPrefab))
		{
			GameObject gameObject = CloneTemplate(weaponPickupPrefab, position, Quaternion.identity);
			WeaponPickup component = gameObject.GetComponent<WeaponPickup>();
			AddPickup(component);
			return component;
		}
		return null;
	}

	private bool WeaponPickupMatch(GameObject weaponPickupPrefab)
	{
		if (weaponPickupPrefab != null && WeaponPickups != null)
		{
			GameObject[] weaponPickups = WeaponPickups;
			foreach (GameObject gameObject in weaponPickups)
			{
				if (weaponPickupPrefab == gameObject)
				{
					return true;
				}
			}
			Debug.LogWarning(string.Format("PickupManager:WeaponPickupMatch() -> No match found for {0}", weaponPickupPrefab.name));
		}
		return false;
	}

	public bool CanCreateMoneyPickup()
	{
		return NumPickupsByPickupType(PickupType.Money) < AllowedMoneyPickups;
	}

	public MoneyPickup CreateMoneyPickup(Vector3 position)
	{
		if (MoneyPickup != null)
		{
			GameObject gameObject = CloneTemplate(MoneyPickup, position, Quaternion.identity);
			MoneyPickup component = gameObject.GetComponent<MoneyPickup>();
			AddPickup(component);
			return component;
		}
		Debug.LogError("PickupManager:CreateMoneyPickup() -> Missing MoneyPickup-Prefab");
		return null;
	}

	private GameObject RandomBonusTemplate()
	{
		if (BonusPickups != null && BonusPickups.Length > 0)
		{
			return BonusPickups[Random.Range(0, BonusPickups.Length)];
		}
		return null;
	}

	private bool CheckArmorPickupChance()
	{
		return Random.value <= ArmorPickupChance;
	}

	private GameObject CloneTemplate(GameObject go, Vector3 position, Quaternion rotation)
	{
		return Object.Instantiate(go, position, rotation) as GameObject;
	}

	private GameObject CloneTemplate(GameObject go)
	{
		return CloneTemplate(go, Vector3.zero, Quaternion.identity);
	}

	private void FindContainer()
	{
		GameObject gameObject = GameObject.Find("Pickups");
		if (gameObject == null)
		{
			gameObject = new GameObject("Pickups");
		}
		pickupsContainer = gameObject;
	}

	private GameObject PickupTemplateByPickupType(PickupType type)
	{
		GameObject result = null;
		switch (type)
		{
		case PickupType.Ammo:
			result = AmmunitionPickup;
			break;
		case PickupType.Armor:
			result = ArmorPickup;
			break;
		case PickupType.Bonus:
			result = RandomBonusTemplate();
			break;
		case PickupType.Syringe:
		case PickupType.SyringeOnly:
			result = SyringePickup;
			break;
		}
		return result;
	}

	public void Reset()
	{
		mode = PickupSystemMode.None;
		DeleteAllActivePickups();
		busySpawnPoints.Clear();
	}

	private PickupLevelRestriction FindLevelRestriction(string levelName)
	{
		PickupLevelRestriction[] levelRestrictions = LevelRestrictions;
		foreach (PickupLevelRestriction pickupLevelRestriction in levelRestrictions)
		{
			if (pickupLevelRestriction.LevelName == levelName)
			{
				return pickupLevelRestriction;
			}
		}
		return null;
	}

	private int NumPickupsByPickupType(PickupType type)
	{
		int num = 0;
		foreach (Pickup pickup in pickups)
		{
			if (pickup.Type == type)
			{
				num++;
			}
		}
		return num;
	}
}
