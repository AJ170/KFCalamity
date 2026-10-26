using EffectEvents;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
	public float Duration = 5f;

	public bool DestroyOnExpire = true;

	public LifetimeSignalSender OnExpireSignal;

	public bool ResetOnActivated;

	public bool TriggerDeathOnExpire;

	private float counter;

	private void Start()
	{
		counter = Duration;
	}

	private void Update()
	{
		if (!(counter > 0f))
		{
			return;
		}
		counter -= Time.deltaTime;
		if (!(counter <= 0f))
		{
			return;
		}
		OnExpireSignal.Send(base.gameObject);
		if (TriggerDeathOnExpire)
		{
			EffectEventDelegator component = base.transform.root.GetComponent<EffectEventDelegator>();
			if (component != null)
			{
				component.TriggerInternal(new Death());
			}
		}
		if (DestroyOnExpire)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void Reset()
	{
		counter = Duration;
	}

	private void OnEnable()
	{
		if (ResetOnActivated)
		{
			Reset();
		}
	}
}
