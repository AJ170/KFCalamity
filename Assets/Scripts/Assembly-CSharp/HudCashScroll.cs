using System.Collections.Generic;
using UnityEngine;

public class HudCashScroll : MonoBehaviour
{
	private enum ScrollSpeedMode
	{
		Normal = 0,
		Fast = 1
	}

	protected class Entry
	{
		public float time;

		public float offset;

		public int val;

		public string surfix;

		public UILabel label;

		public UILabel surfixLabel;

		public TweenPosition PositionTween;

		public List<TweenAlpha> AlphaTweens;

		public GameObject Object;
	}

	protected class QueuedEntry
	{
		public object obj;

		public object baseObj;

		public Color c;

		public string surfix;
	}

	[Range(3f, 10f)]
	public int MaxEntries = 3;

	[Range(0.1f, 10f)]
	public float NormalTweenDuration = 0.3f;

	[Range(0.1f, 10f)]
	public float FastTweenDuration = 0.1f;

	[Range(0.1f, 10f)]
	public float NormalStayDuration = 1.5f;

	[Range(0.1f, 10f)]
	public float FastStayDuration = 0.5f;

	[Range(0.1f, 10f)]
	public float NormalTweenResetTime = 0.8f;

	[Range(0.1f, 10f)]
	public float FastTweenResetTime = 0.4f;

	[Range(0.1f, 10f)]
	public float TweenResetCounter;

	public GameObject MoneyTextPrefab;

	public UILabel.Effect effect;

	public UIFont MainFont;

	public UIFont SurfixFont;

	private Queue<QueuedEntry> waitingEntries = new Queue<QueuedEntry>();

	private Queue<Entry> entries = new Queue<Entry>();

	private List<Entry> unusedEntries = new List<Entry>();

	private HudMoneyHandler moneyHandler;

	private ScrollSpeedMode speedMode;

	private bool tweeningList;

	private bool calculateTweenTime;

	private bool canSyncMoney;

	private int tweenCounter;

	public HudMoneyHandler MoneyHandler
	{
		get
		{
			return moneyHandler;
		}
		set
		{
			moneyHandler = value;
		}
	}

	public bool CanSyncMoney
	{
		get
		{
			return canSyncMoney;
		}
		set
		{
			canSyncMoney = value;
		}
	}

	public int TotalQueueMoney
	{
		get
		{
			int num = 0;
			foreach (Entry entry in entries)
			{
				if (entry != null)
				{
					num += entry.val;
				}
			}
			foreach (QueuedEntry waitingEntry in waitingEntries)
			{
				if (waitingEntry != null)
				{
					num += (int)waitingEntry.obj;
				}
			}
			return num;
		}
	}

	private float TweenDuration
	{
		get
		{
			switch (speedMode)
			{
			case ScrollSpeedMode.Normal:
				return NormalTweenDuration;
			case ScrollSpeedMode.Fast:
				return FastTweenDuration;
			default:
				return 1f;
			}
		}
	}

	private float StayDuration
	{
		get
		{
			switch (speedMode)
			{
			case ScrollSpeedMode.Normal:
				return NormalStayDuration;
			case ScrollSpeedMode.Fast:
				return FastStayDuration;
			default:
				return 1f;
			}
		}
	}

	private float TweenResetTime
	{
		get
		{
			switch (speedMode)
			{
			case ScrollSpeedMode.Normal:
				return NormalTweenResetTime;
			case ScrollSpeedMode.Fast:
				return FastTweenResetTime;
			default:
				return 1f;
			}
		}
	}

	private void Start()
	{
		for (int i = 0; i < MaxEntries; i++)
		{
			Entry entry = new Entry();
			entry.Object = NGUITools.AddChild(base.gameObject, MoneyTextPrefab);
			HudCashScrollEntry component = entry.Object.GetComponent<HudCashScrollEntry>();
			entry.label = component.Label;
			entry.label.gameObject.layer = base.gameObject.layer;
			entry.label.name = (entries.Count + 1).ToString();
			entry.label.effectStyle = effect;
			entry.surfixLabel = component.SurfixLabel;
			entry.surfixLabel.gameObject.layer = base.gameObject.layer;
			entry.surfixLabel.name = entries.Count + 1 + " surfix";
			entry.surfixLabel.effectStyle = effect;
			entry.PositionTween = component.PositionTween;
			entry.AlphaTweens = component.AlphaTweens;
			NGUITools.SetActive(entry.Object, false);
			entry.PositionTween.enabled = false;
			for (int j = 0; j < 2; j++)
			{
				entry.AlphaTweens[j].enabled = false;
			}
			entry.label.MakePixelPerfect();
			entry.Object.transform.localPosition = new Vector3(0f, MainFont.size * (unusedEntries.Count + 1), 0f);
			unusedEntries.Add(entry);
			base.gameObject.SetActive(false);
		}
	}

	private Entry Create()
	{
		if (unusedEntries.Count > 0)
		{
			Entry entry = unusedEntries[unusedEntries.Count - 1];
			unusedEntries.RemoveAt(unusedEntries.Count - 1);
			entry.label.depth = NGUITools.CalculateNextDepth(base.gameObject);
			entry.surfixLabel.text = string.Empty;
			entry.PositionTween.Reset();
			entry.PositionTween.callWhenFinished = string.Empty;
			entry.PositionTween.enabled = false;
			entry.Object.transform.localPosition = new Vector3(0f, MainFont.size * (entries.Count + 1), 0f);
			entry.time = 0f;
			NGUITools.SetActive(entry.Object, true);
			entry.offset = 0f;
			entries.Enqueue(entry);
			return entry;
		}
		return null;
	}

	public void Add(object baseObj, object obj, Color c, string surfix)
	{
		base.gameObject.SetActive(true);
		CanSyncMoney = false;
		if (tweeningList)
		{
			QueuedEntry queuedEntry = new QueuedEntry();
			queuedEntry.obj = obj;
			queuedEntry.baseObj = baseObj;
			queuedEntry.c = c;
			queuedEntry.surfix = surfix;
			waitingEntries.Enqueue(queuedEntry);
		}
		else if (entries.Count < MaxEntries && waitingEntries.Count == 0)
		{
			speedMode = ScrollSpeedMode.Normal;
			AddEntry(baseObj, obj, c, surfix);
		}
		else
		{
			speedMode = ScrollSpeedMode.Fast;
			QueuedEntry queuedEntry2 = new QueuedEntry();
			queuedEntry2.obj = obj;
			queuedEntry2.baseObj = baseObj;
			queuedEntry2.c = c;
			queuedEntry2.surfix = surfix;
			waitingEntries.Enqueue(queuedEntry2);
		}
	}

	private void AddEntry(object baseObj, object obj, Color c, string surfix)
	{
		if (!base.enabled)
		{
			return;
		}
		bool flag = false;
		int val = 0;
		int num = (int)baseObj;
		if (obj is int)
		{
			flag = true;
			val = (int)obj;
		}
		string text = ((!flag) ? obj.ToString() : ((!((float)num < 0f)) ? ("+ " + Mathf.RoundToInt(num)) : Mathf.RoundToInt(num).ToString()));
		if (entries.Count < MaxEntries)
		{
			Entry entry = Create();
			entry.label.color = c;
			entry.val = val;
			entry.label.text = text;
			entry.surfixLabel.text = surfix;
			Vector2 vector = entry.label.font.CalculatePrintedSize(text, false, UIFont.SymbolStyle.None) * MainFont.size;
			entry.surfixLabel.transform.localPosition = new Vector3(vector.x, 0f, 0f);
			entry.time = 0f;
			float duration = NormalTweenDuration;
			if (HasWaitingEntries())
			{
				duration = FastTweenDuration;
			}
			entry.label.alpha = 0f;
			entry.surfixLabel.alpha = 0f;
			for (int i = 0; i < entry.AlphaTweens.Count; i++)
			{
				entry.AlphaTweens[i].duration = duration;
				entry.AlphaTweens[i].from = 0f;
				entry.AlphaTweens[i].to = 1f;
				entry.AlphaTweens[i].enabled = true;
				entry.AlphaTweens[i].Play(true);
			}
		}
	}

	private void SpawnWaitingEntries()
	{
		if (entries.Count < MaxEntries && waitingEntries.Count > 0)
		{
			QueuedEntry queuedEntry = waitingEntries.Dequeue();
			AddEntry(queuedEntry.baseObj, queuedEntry.obj, queuedEntry.c, queuedEntry.surfix);
			speedMode = ScrollSpeedMode.Fast;
			SpawnWaitingEntries();
		}
		else if (waitingEntries.Count == 0)
		{
			speedMode = ScrollSpeedMode.Normal;
		}
	}

	private bool HasWaitingEntries()
	{
		if (waitingEntries.Count > 0)
		{
			return true;
		}
		return false;
	}

	private void TweenEntries()
	{
		float tweenDuration = TweenDuration;
		int num = 0;
		foreach (Entry entry in entries)
		{
			Vector3 localPosition = entry.Object.transform.localPosition;
			float num2 = entry.label.font.size;
			if (num == 0)
			{
				for (int i = 0; i < entry.AlphaTweens.Count; i++)
				{
					entry.AlphaTweens[i].duration = tweenDuration;
					entry.AlphaTweens[i].Play(false);
				}
				Tween(0, entry, localPosition, Vector3.zero, tweenDuration, base.gameObject, "TweenCallBack");
				tweenCounter++;
			}
			else
			{
				Tween(0, entry, localPosition, new Vector3(0f, localPosition.y - num2, 0f), tweenDuration, base.gameObject, "TweenCallBack");
				tweenCounter++;
			}
			num++;
		}
		tweeningList = true;
	}

	private void Tween(int index, Entry en, Vector3 from, Vector3 to, float duration)
	{
		if (en != null)
		{
			en.PositionTween.Reset2();
			en.PositionTween.from = from;
			en.PositionTween.to = to;
			en.PositionTween.duration = duration;
			en.PositionTween.method = UITweener.Method.EaseOut;
			en.PositionTween.enabled = true;
		}
	}

	private void Tween(int index, Entry en, Vector3 from, Vector3 to, float duration, GameObject callbackReciever, string callback)
	{
		if (en != null)
		{
			en.PositionTween.Reset2();
			en.PositionTween.from = from;
			en.PositionTween.to = to;
			en.PositionTween.duration = duration;
			en.PositionTween.method = UITweener.Method.EaseOut;
			en.PositionTween.callWhenFinished = callback;
			en.PositionTween.eventReceiver = callbackReciever;
			en.PositionTween.enabled = true;
		}
	}

	public void TweenCallBack()
	{
		tweenCounter--;
		if (tweenCounter == 0)
		{
			tweenCounter = 0;
			DeleteAndContinue();
		}
	}

	private void StartTweenResetTimer()
	{
		TweenResetCounter = 0f;
		calculateTweenTime = true;
	}

	private void OnTweenResetTick()
	{
		TweenResetCounter += Time.deltaTime;
		if (TweenResetCounter >= TweenResetTime)
		{
			tweeningList = false;
			calculateTweenTime = false;
		}
	}

	private void DeleteAndContinue()
	{
		Entry entry = entries.Dequeue();
		MoneyHandler.SyncMoney();
		NGUITools.SetActive(entry.Object, false);
		entry.surfixLabel.text = string.Empty;
		unusedEntries.Add(entry);
		if (HasWaitingEntries())
		{
			SpawnWaitingEntries();
			StartTweenResetTimer();
			return;
		}
		CanSyncMoney = true;
		MoneyHandler.SyncMoney();
		tweeningList = false;
		if (entries.Count == 0)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void HandleEntryLifeTimes()
	{
		if (entries.Count <= 0)
		{
			return;
		}
		int num = 0;
		bool flag = false;
		foreach (Entry entry in entries)
		{
			entry.time += Time.deltaTime;
			if (!tweeningList && num == 0 && entry.time >= StayDuration)
			{
				flag = true;
			}
			num++;
		}
		if (flag)
		{
			TweenEntries();
		}
	}

	private void Update()
	{
		if (calculateTweenTime)
		{
			OnTweenResetTick();
		}
		HandleEntryLifeTimes();
	}
}
