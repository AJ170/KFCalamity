using UnityEngine;

public class ShootingRangeEnemy : MonoBehaviour
{
	public ZedType EnemyType;

	public EnemyLaneType LaneType;

	private GameObject waypoint;

	public GameObject Waypoint
	{
		get
		{
			return waypoint;
		}
	}

	private void Start()
	{
		Zed component = GetComponent<Zed>();
		component.Bounty = 0;
		component.Experience = 0;
		component.WeaponDropChance = 0f;
		AIMovementMotor component2 = GetComponent<AIMovementMotor>();
		component2.StoppingDistance = 1.5f;
		Health component3 = GetComponent<Health>();
		component3.OnDeathSignal.AddReceiver(OnUnitDeath);
	}

	public void SetWaypointOccupied(GameObject waypoint)
	{
		this.waypoint = waypoint;
	}

	private void OnUnitDeath(Health health)
	{
	}
}
