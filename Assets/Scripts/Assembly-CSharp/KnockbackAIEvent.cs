using UnityEngine;

internal class KnockbackAIEvent : AIEvent
{
	private Vector3 direction;

	private float force;

	private float duration;

	public Vector3 Direction
	{
		get
		{
			return direction;
		}
	}

	public float Force
	{
		get
		{
			return force;
		}
	}

	public float Duration
	{
		get
		{
			return duration;
		}
	}

	public KnockbackAIEvent(Vector3 direction, float force, float duration)
		: base(AIEventID.Knockback)
	{
		this.direction = direction;
		this.force = force;
		this.duration = duration;
	}
}
