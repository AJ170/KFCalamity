using EffectEvents;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class CombatStatusController : MonoBehaviour, IEffectTriggerListener
{
	private EffectEventDelegator eventDelegator;

	private bool inCombat;

	private bool isSafe;

	private float leaveCombatTime = 6f;

	private float untilSafeTime = 5f;

	private float combatCounter;

	private float safeCounter;

	public bool InCombat
	{
		get
		{
			return inCombat;
		}
	}

	public bool IsSafe
	{
		get
		{
			return isSafe;
		}
	}

	private void Start()
	{
		eventDelegator = base.transform.root.GetComponent<EffectEventDelegator>();
		if (eventDelegator != null)
		{
			eventDelegator.RegisterListener(this);
		}
		else
		{
			Debug.LogError(string.Format("CombatStatusController:Start() -> Didn't find 'EffectEventDelegator' in {0}", base.transform.root.name));
		}
	}

	private void Update()
	{
		if (InCombat)
		{
			combatCounter = Mathf.Min(combatCounter + Time.deltaTime, leaveCombatTime);
			if (combatCounter == leaveCombatTime)
			{
				LeaveCombat();
			}
		}
		if (!IsSafe)
		{
			safeCounter = Mathf.Min(safeCounter + Time.deltaTime, untilSafeTime);
			if (safeCounter == untilSafeTime)
			{
				Safe();
			}
		}
	}

	public void OnEffectTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.DamageReceived)
		{
			AttackedByEnemy(e as DamageReceived);
		}
		else if (e.Id == TriggerId.DamageDealt)
		{
			AttackedEnemy(e as DamageDealt);
		}
	}

	private void AttackedByEnemy(DamageReceived damageReceived)
	{
		if (!InCombat)
		{
			EnterCombat();
		}
		if (IsSafe)
		{
			NotSafe();
		}
		combatCounter = 0f;
		safeCounter = 0f;
	}

	private void AttackedEnemy(DamageDealt damageDealt)
	{
		if (!InCombat)
		{
			EnterCombat();
		}
		combatCounter = 0f;
	}

	private void EnterCombat()
	{
		inCombat = true;
		eventDelegator.TriggerInternal(new CombatEnter());
	}

	private void LeaveCombat()
	{
		inCombat = false;
		eventDelegator.TriggerInternal(new CombatLeave());
	}

	private void Safe()
	{
		isSafe = true;
		eventDelegator.TriggerInternal(new Safe());
	}

	private void NotSafe()
	{
		isSafe = false;
	}
}
