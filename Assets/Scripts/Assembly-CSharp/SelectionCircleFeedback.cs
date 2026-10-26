using ActorEvents;
using UnityEngine;

public class SelectionCircleFeedback : TakeDamageMaterialTint
{
	private bool obscured;

	private Color playerColor;

	private Shader seeThroughShader;

	protected override void Start()
	{
		base.Start();
		seeThroughShader = Shader.Find("Test: Selection Circle");
		if (seeThroughShader != null)
		{
			ActorEventController componentInRoot = base.gameObject.GetComponentInRoot<ActorEventController>();
			if (componentInRoot != null)
			{
				componentInRoot.Register(ActorEventID.ObscurityChanged, OnObscurityChanged);
			}
		}
		else
		{
			Debug.LogWarning("[SelectionCircleFeedback] Did not find SeeThrough-Shader");
		}
	}

	public void SetPlayerColorByID(int playerID)
	{
		playerColor = GlobalHelpers.PlayerColorByID(playerID);
	}

	private void OnObscurityChanged(ActorEventBase e)
	{
		ObscurityChanged obscurityChanged = e as ObscurityChanged;
		obscured = obscurityChanged.Obscured;
		if (obscured)
		{
			StopPulse(true);
			SetSeeThroughShader();
		}
		else
		{
			SetDefaultShader();
		}
	}

	protected override void OnDamageTaken(Health health, int damageTaken, int rawDamage, float armorTypeMultiplier)
	{
		if (!obscured)
		{
			Pulse();
		}
	}

	private void SetDefaultShader()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.shader = material.DefaultShader;
		}
	}

	private void SetSeeThroughShader()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.shader = seeThroughShader;
			material.Material.color = playerColor;
		}
	}
}
