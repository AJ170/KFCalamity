using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
	[Serializable]
	public class ShootingLane
	{
		public string LaneName;

		public ZedType EnemyType;

		public GameObject SpawnPoint;

		public GameObject Waypoint;

		public GameObject StartEnemy;
	}

	public ShootingLane[] ShootingLanes;

	public GameObject[] GrenadePitEnemies;

	public GameObject GrenadePitWaypointParent;

	public GameObject GrenadePitSpawnpoint;

	private GameUnitManager gameUnitManager;

	private List<GameObject> freeGrenadePitWaypoints = new List<GameObject>();

	private void Start()
	{
		Setup();
	}

	private void Setup()
	{
		GameObject gameObject = GameObject.Find("GameUnitManager");
		if (gameObject != null)
		{
			gameUnitManager = gameObject.GetComponent<GameUnitManager>();
		}
		ShootingLane[] shootingLanes = ShootingLanes;
		foreach (ShootingLane shootingLane in shootingLanes)
		{
			SetupEnemy(shootingLane.StartEnemy, shootingLane.EnemyType, EnemyLaneType.Normal, null);
		}
		freeGrenadePitWaypoints.AddRange(GetAllGrenadePitWaypoints());
		GameObject[] grenadePitEnemies = GrenadePitEnemies;
		foreach (GameObject gameObject2 in grenadePitEnemies)
		{
			SetupEnemy(gameObject2, ZedType.Clot, EnemyLaneType.Grenade, null);
		}
	}

	private void SetupEnemy(GameObject gameObject, ZedType enemyType, EnemyLaneType laneType, GameObject waypoint)
	{
		Health component = gameObject.GetComponent<Health>();
		component.OnDeathSignal.AddReceiver(OnEnemyDeath);
		ShootingRangeEnemy component2 = gameObject.GetComponent<ShootingRangeEnemy>();
		if (component2 == null)
		{
			component2 = gameObject.AddComponent<ShootingRangeEnemy>();
			component2.EnemyType = enemyType;
			component2.LaneType = laneType;
			component2.SetWaypointOccupied(waypoint);
		}
		AIController component3 = gameObject.GetComponent<AIController>();
		AITargetHandler component4 = gameObject.GetComponent<AITargetHandler>();
		if (component3 != null)
		{
			UnityEngine.Object.Destroy(component4);
			UnityEngine.Object.Destroy(component3);
		}
	}

	private void GotoWaypoint(GameObject enemy, Vector3 waypointPos)
	{
		AIMovementMotor component = enemy.GetComponent<AIMovementMotor>();
		component.MoveToTargetPosition(waypointPos);
	}

	private GameObject[] GetAllGrenadePitWaypoints()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in GrenadePitWaypointParent.transform)
		{
			list.Add(item.gameObject);
		}
		return list.ToArray();
	}

	private GameObject GetRandomGrenadePitWaypoint()
	{
		int index = UnityEngine.Random.Range(0, freeGrenadePitWaypoints.Count);
		GameObject result = freeGrenadePitWaypoints[index];
		freeGrenadePitWaypoints.RemoveAt(index);
		return result;
	}

	private void SpawnEnemy(EnemyLaneType laneType, ZedType enemyType)
	{
		Vector3 position = Vector3.zero;
		Vector3 waypointPos = Vector3.zero;
		GameObject gameObject = null;
		switch (laneType)
		{
		case EnemyLaneType.Normal:
		{
			ShootingLane shootingLane = LaneFromZedType(enemyType);
			if (shootingLane != null)
			{
				position = shootingLane.SpawnPoint.transform.position;
				waypointPos = shootingLane.Waypoint.transform.position;
			}
			else
			{
				Debug.LogError("ShootingRangeManager:SpawnEnemy() - No ShootingLane found for enemy of type: " + enemyType);
			}
			break;
		}
		case EnemyLaneType.Grenade:
			position = GrenadePitSpawnpoint.transform.position;
			gameObject = GetRandomGrenadePitWaypoint();
			waypointPos = gameObject.transform.position;
			break;
		default:
			Debug.LogError("ShootingRangeManager:SpawnEnemy() - Invalid LaneType: " + laneType);
			return;
		}
		GameObject enemy = gameUnitManager.CreateUnit(enemyType, position, Quaternion.identity);
		SetupEnemy(enemy, enemyType, laneType, gameObject);
		GotoWaypoint(enemy, waypointPos);
	}

	private void OnEnemyDeath(Health health)
	{
		ShootingRangeEnemy component = health.gameObject.GetComponent<ShootingRangeEnemy>();
		if (component != null)
		{
			if (component.LaneType == EnemyLaneType.Grenade && component.Waypoint != null)
			{
				freeGrenadePitWaypoints.Add(component.Waypoint);
			}
			SpawnEnemy(component.LaneType, component.EnemyType);
		}
		else
		{
			Debug.LogError("ShootingRangeManager:OnEnemyDeath() - GameObject: '" + health.gameObject.name + "' has no ShootingRangeEnemy component!");
		}
	}

	private void Update()
	{
	}

	private ShootingLane LaneFromZedType(ZedType zedType)
	{
		return Array.Find(ShootingLanes, (ShootingLane a) => a.EnemyType == zedType);
	}
}
