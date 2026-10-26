using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class EffectBaseTarget : IFactoryItem
{
	protected EffectType effectType;

	protected BuffType buffType;

	protected BuffClass buffClass;

	protected ReceiveTriggerHandler receiveTriggerHandler;

	private GameObject source;

	protected GameObject gameObject;

	private EffectSystem creator;

	private EffectSystem owner;

	protected float repeatInterval;

	protected int numRepeats;

	protected int maxRepeats;

	protected float timeSinceLastInterval;

	protected bool overridable = true;

	protected bool stackable;

	protected int numStacks;

	protected int maxStacks;

	protected bool deleted;

	protected bool isShutdown;

	protected bool receiveTriggers;

	private bool activated;

	public EffectType EffectType
	{
		get
		{
			return effectType;
		}
	}

	public BuffType BuffType
	{
		get
		{
			return buffType;
		}
	}

	public BuffClass BuffClass
	{
		get
		{
			return buffClass;
		}
	}

	public bool IsOverridable
	{
		get
		{
			return overridable;
		}
	}

	public bool IsStackable
	{
		get
		{
			return stackable;
		}
	}

	public int NumStacks
	{
		get
		{
			return numStacks;
		}
	}

	public int MaxStacks
	{
		get
		{
			return maxStacks;
		}
	}

	public bool IsDeleted
	{
		get
		{
			return deleted;
		}
	}

	public bool IsActivated
	{
		get
		{
			return activated;
		}
	}

	public GameObject Source
	{
		get
		{
			return source;
		}
		set
		{
			if (source == null)
			{
				source = value;
			}
		}
	}

	public GameObject GameObject
	{
		get
		{
			return gameObject;
		}
		set
		{
			gameObject = value;
		}
	}

	public EffectSystem Creator
	{
		get
		{
			return creator;
		}
		set
		{
			if (creator == null)
			{
				creator = value;
			}
		}
	}

	public EffectSystem Owner
	{
		get
		{
			return owner;
		}
		set
		{
			if (owner == null)
			{
				owner = value;
			}
		}
	}

	void IFactoryItem.Init()
	{
		Awake();
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		if (receiveTriggers)
		{
			SetupReceiveTriggerHandler();
		}
		if (stackable)
		{
			Stack();
		}
	}

	public bool ShouldShutdown()
	{
		return activated && !isShutdown;
	}

	public virtual void Shutdown()
	{
		if (receiveTriggerHandler != null)
		{
			receiveTriggerHandler.Shutdown();
		}
		deleted = true;
		isShutdown = true;
	}

	public virtual void Update()
	{
		if (!deleted)
		{
			if (!IsActivated)
			{
				Start();
				ActivateEffect();
			}
			if (effectType == EffectType.Continuous)
			{
				UpdateContinuous();
			}
			else if (effectType == EffectType.Infinite)
			{
				UpdateInfinite();
			}
			else if (effectType == EffectType.Instant)
			{
				TriggerInstant();
			}
		}
	}

	private void SetupReceiveTriggerHandler()
	{
		receiveTriggerHandler = new ReceiveTriggerHandler(this);
	}

	public virtual void OnReceivedTrigger(TriggerBase e)
	{
	}

	public virtual bool Evaluator()
	{
		return true;
	}

	protected virtual void OnActivated()
	{
	}

	private void ActivateEffect()
	{
		activated = true;
		OnActivated();
	}

	protected virtual void Execute()
	{
	}

	private void TriggerInstant()
	{
		Execute();
		deleted = true;
	}

	protected virtual void OnUpdate()
	{
	}

	private void UpdateContinuous()
	{
		UpdateInterval();
		if (TimeToUpdate())
		{
			numRepeats = Mathf.Min(numRepeats + 1, maxRepeats);
			timeSinceLastInterval = 0f;
			OnUpdate();
			if (numRepeats == maxRepeats)
			{
				deleted = true;
			}
		}
	}

	private void UpdateInfinite()
	{
		UpdateInterval();
		if (TimeToUpdate())
		{
			timeSinceLastInterval = 0f;
			OnUpdate();
		}
	}

	private void UpdateInterval()
	{
		if (repeatInterval > 0f)
		{
			timeSinceLastInterval = Mathf.Min(timeSinceLastInterval + Time.deltaTime, repeatInterval);
		}
	}

	private bool TimeToUpdate()
	{
		if (repeatInterval > 0f && !deleted)
		{
			return timeSinceLastInterval == repeatInterval;
		}
		return false;
	}

	private void ResetRepeats()
	{
		numRepeats = 0;
	}

	private void ModifyDuration(float multiplier)
	{
		float a = multiplier * (float)maxRepeats + 0.5f;
		a = Mathf.Max(a, 1f);
		maxRepeats = (int)a;
	}

	public bool Stack()
	{
		if (stackable)
		{
			ResetRepeats();
			if (numStacks < maxStacks)
			{
				numStacks++;
				OnAddStack();
				return true;
			}
		}
		return false;
	}

	protected virtual void OnAddStack()
	{
	}

	public bool Refresh(EffectBaseTarget newEffect)
	{
		if (overridable)
		{
			if (newEffect.Source != null)
			{
				source = newEffect.Source;
			}
			if (newEffect.Creator != null)
			{
				creator = newEffect.Creator;
			}
			ResetRepeats();
			OnRefreshed(newEffect);
			return true;
		}
		return false;
	}

	protected virtual void OnRefreshed(EffectBaseTarget newEffect)
	{
	}

	public void Resistance(EffectResistance resistance, out bool immune)
	{
		immune = false;
		if (resistance != null)
		{
			if (resistance.IsImmune)
			{
				immune = true;
			}
			else if (effectType == EffectType.Continuous && resistance.Name != 0)
			{
				ModifyDuration(resistance.Multiplier);
			}
		}
	}

	public void ForceDelete()
	{
		deleted = true;
	}
}
