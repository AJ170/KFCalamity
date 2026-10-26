using UnityEngine.AI;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public AudioClip PickupSound;

	public AudioClip FailedPickupSound;

	public string PickupParticle;

	public float Volume = 1f;

	protected PickupType type;

	private PickupSpawnPoint spawnPoint;

	protected bool activated;

	protected bool deleted;

	private bool respawning;

	protected float respawnTime;

	private float respawnTimer;

	private bool acquireNewSpawnLocation;

	protected bool sampleNavMeshForGroundCollision;

	private bool pooledPickup;

	public float RespawnTime
	{
		get
		{
			return respawnTime;
		}
		set
		{
			respawnTime = value;
		}
	}

	public bool WantsNewSpawnLocation
	{
		get
		{
			return acquireNewSpawnLocation;
		}
	}

	public PickupType Type
	{
		get
		{
			return type;
		}
	}

	public PickupSpawnPoint SpawnPoint
	{
		get
		{
			return spawnPoint;
		}
		set
		{
			spawnPoint = value;
		}
	}

	public bool IsActivated
	{
		get
		{
			return activated;
		}
	}

	public bool IsDeleted
	{
		get
		{
			return deleted;
		}
	}

	public bool IsRespawning
	{
		get
		{
			return respawning;
		}
	}

	public bool PooledPickup
	{
		get
		{
			return pooledPickup;
		}
		set
		{
			pooledPickup = value;
		}
	}

	public virtual void Shutdown()
	{
		if (pooledPickup)
		{
			activated = false;
			deleted = false;
			respawning = false;
			acquireNewSpawnLocation = false;
			sampleNavMeshForGroundCollision = false;
			spawnPoint = null;
			HidePickupObject();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public bool SetLocation(PickupSpawnPoint spawnPoint)
	{
		acquireNewSpawnLocation = false;
		if (spawnPoint != null)
		{
			this.spawnPoint = spawnPoint;
			base.transform.position = spawnPoint.transform.position;
			return true;
		}
		return false;
	}

	public void SetRotation(Quaternion rotation)
	{
		base.transform.rotation = rotation;
	}

	private void Update()
	{
		if (!IsActivated)
		{
			Activate();
		}
		CheckNavMeshCollision();
		OnUpdate();
	}

	protected virtual void OnUpdate()
	{
	}

	protected virtual void OnFailedPickup(GameObject player)
	{
		if (Type != 0 && player != null)
		{
			MaterialPulse component = base.gameObject.GetComponent<MaterialPulse>();
			if (component != null)
			{
				component.Pulse();
			}
			PlayerGUILink playerGUILink = GUILinkByObject(player);
			if (playerGUILink != null)
			{
				playerGUILink.OnPickupFailed(Type);
			}
			PlayFailedPickupSound();
		}
	}

	private void Activate()
	{
		activated = true;
		OnActivated();
	}

	protected virtual void OnActivated()
	{
	}

	protected bool Respawn()
	{
		if (respawnTime > 0f)
		{
			PickupManager instance = PickupManager.Instance;
			if (instance != null)
			{
				if (instance.Mode == PickupSystemMode.Static)
				{
					respawning = true;
					respawnTimer = respawnTime;
					return true;
				}
				if (Type == PickupType.Ammo)
				{
					respawning = true;
					respawnTimer = respawnTime;
					acquireNewSpawnLocation = true;
					return true;
				}
			}
		}
		return false;
	}

	public void UpdateRespawnTimer()
	{
		if (IsRespawning)
		{
			respawnTimer = Mathf.Max(respawnTimer - Time.deltaTime, 0f);
			if (respawnTimer == 0f)
			{
				respawning = false;
				OnRespawn();
			}
		}
	}

	protected virtual void OnRespawn()
	{
	}

	protected void ShowPickupObject()
	{
		base.gameObject.SetActive(true);
	}

	protected void HidePickupObject()
	{
		base.gameObject.SetActive(false);
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		string text = LayerMask.LayerToName(collision.gameObject.layer);
		if (text == "Ground")
		{
			CollisionGround();
		}
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Soldier")
		{
			CollisionPlayer(collider);
		}
	}

	private void CollisionGround()
	{
		OnCollideGround();
		sampleNavMeshForGroundCollision = false;
	}

	private void CollisionPlayer(Collider collider)
	{
		if (!deleted)
		{
			OnCollidePlayer(collider);
		}
	}

	protected virtual void OnCollideGround()
	{
	}

	protected virtual void OnCollidePlayer(Collider collider)
	{
	}

	public void ForceCollidePlayer(Collider collider)
	{
		CollisionPlayer(collider);
	}

	private void CheckNavMeshCollision()
	{
		if (sampleNavMeshForGroundCollision && IsOnNavMesh(base.transform.position, 0.35f))
		{
			CollisionGround();
		}
	}

	protected void PlayPickupSound()
	{
		if (PickupSound != null)
		{
			PlayAudioClip(PickupSound);
		}
	}

	protected void PlayFailedPickupSound()
	{
		if (FailedPickupSound != null)
		{
			PlayAudioClip(FailedPickupSound);
		}
	}

	protected void PlayAudioClip(AudioClip clip)
	{
		if (clip != null)
		{
			AudioManager.PlayClipAtPoint(clip, base.transform.position, Volume, Time.timeScale);
		}
	}

	protected void PlayPickupParticle()
	{
		ParticleManager instance = ParticleManager.Instance;
		if (instance != null && !string.IsNullOrEmpty(PickupParticle))
		{
			GameObject particle = instance.CreateParticle(PickupParticle, base.transform.position, Quaternion.identity, ParticleDestroyMethod.Duration);
			PlayParticle(particle);
		}
	}

	protected void PlayParticle(GameObject particle)
	{
		if (particle != null && particle.GetComponent<ParticleSystem>() != null)
		{
			particle.GetComponent<ParticleSystem>().Play();
		}
	}

	public void ForceDelete()
	{
		deleted = true;
	}

	protected PlayerGUILink GUILinkByObject(GameObject go)
	{
		if (go != null)
		{
			return go.GetComponent<PlayerGUILink>();
		}
		return null;
	}

	public void Rotate(Quaternion rotation)
	{
		if (base.GetComponent<Rigidbody>() != null)
		{
			base.GetComponent<Rigidbody>().MoveRotation(rotation);
		}
	}

	protected bool IsOnNavMesh(Vector3 position, float radius = 0.25f)
	{
		NavMeshHit hit;
		return NavMesh.SamplePosition(position, out hit, radius, 1);
	}
}
