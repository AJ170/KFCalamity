using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SpawnBulletEffect : EffectBaseListener
{
	public GameObject Bullet;

	public float Offset = 0.3f;

	public bool TriggerBulletImpact;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (Bullet != null)
		{
			if (e.Id == TriggerId.Projectile)
			{
				Projectile projectile = (Projectile)e;
				SpawnBullet(projectile.Bone, projectile.Direction, projectile.Target, projectile.Collider);
			}
			else if (e.Id == TriggerId.Hit)
			{
				Hit hit = (Hit)e;
				Transform hitTransform = hit.HitTransform;
				Vector3 position = hitTransform.position;
				Vector3 position2 = base.transform.position;
				SpawnBullet(base.transform, Vector3.Normalize(position - position2), position, hit.HitTransform.GetComponent<Collider>());
			}
			else if (e.Id == TriggerId.Impact)
			{
				Impact impact = (Impact)e;
				Vector3 position3 = impact.Position;
				SpawnBullet(base.transform, Vector3.Normalize(position3 - base.transform.position), position3, impact.Collider);
			}
		}
		else
		{
			Debug.Log(string.Format("[SpawnBulletEffect:Execute] No bullet set in weapon {0}.", base.transform.parent.name));
		}
	}

	private void SpawnBullet(Transform bone, Vector3 direction, Vector3 targetPosition, Collider collider = null)
	{
		GameObject effect = EffectPool.GetEffect(Bullet);
		if (!(effect != null))
		{
			return;
		}
		Transform transform = effect.transform;
		Vector3 a = (transform.position = bone.position + direction * Offset);
		transform.rotation = Quaternion.LookRotation(direction);
		effect.SetActive(true);
		Bullet component = effect.GetComponent<Bullet>();
		if (component != null)
		{
			component.SetMaxDistance(Vector3.Distance(a, targetPosition));
			component.Fire();
			if (TriggerBulletImpact && effectSystem != null)
			{
				component.SetTriggerImpact(effectSystem, collider, targetPosition);
			}
		}
	}
}
