using UnityEngine;

public class MoneyPickup : Pickup
{
	public AudioClip[] PickupSounds;

	public AudioClip HitGroundSound;

	public float DespawnTime = 60f;

	private int lastLayer;

	private int wealth = 50;

	private float despawnTimer;

	public int Wealth
	{
		get
		{
			return wealth;
		}
		set
		{
			wealth = value;
		}
	}

	private void Awake()
	{
		type = PickupType.Money;
		lastLayer = base.gameObject.layer;
		base.gameObject.layer = LayerMask.NameToLayer("Boundries");
	}

	private void Start()
	{
		PickupSpawnHelper.Arc(base.gameObject, 3f);
		sampleNavMeshForGroundCollision = true;
		despawnTimer = DespawnTime;
	}

	protected override void OnUpdate()
	{
		if (despawnTimer > 0f)
		{
			despawnTimer = Mathf.Max(despawnTimer - Time.deltaTime, 0f);
			if (despawnTimer == 0f)
			{
				deleted = true;
			}
		}
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
		OnMoneyPickedUp(collider.gameObject);
	}

	private void OnMoneyPickedUp(GameObject player)
	{
		if (!(player != null))
		{
			return;
		}
		Soldier component = player.GetComponent<Soldier>();
		if (!(component != null))
		{
			return;
		}
		PlayerStats stats = component.Stats;
		if (stats != null)
		{
			PlayRandomPickupSound();
			PlayPickupParticle();
			stats.AddMoney(Wealth);
			PlayerGUILink playerGUILink = GUILinkByObject(player);
			if (playerGUILink != null)
			{
				playerGUILink.OnMoneyGained(Wealth);
			}
			deleted = true;
		}
	}

	private void PlayRandomPickupSound()
	{
		if (PickupSounds.Length > 0)
		{
			PlayAudioClip(PickupSounds[Random.Range(0, PickupSounds.Length)]);
		}
	}

	protected void PlayHitGroundSound()
	{
		if (HitGroundSound != null)
		{
			PlayAudioClip(HitGroundSound);
		}
	}
}
