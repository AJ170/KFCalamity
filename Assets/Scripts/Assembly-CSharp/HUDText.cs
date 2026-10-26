using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/HUD Text")]
public class HUDText : MonoBehaviour
{
	protected class Entry
	{
		public float time;

		public float stay;

		public float offset;

		public float val;

		public UILabel label;

		public float movementStart
		{
			get
			{
				return time + stay;
			}
		}
	}

	public int EntryCapacity = 5;

	private int nextRemoveEntryIndex;

	public UIFont font;

	public UILabel.Effect effect;

	public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(3f, 40f));

	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(1f, 1f), new Keyframe(3f, 0f));

	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f));

	private List<Entry> mList = new List<Entry>();

	private List<Entry> mUnused = new List<Entry>();

	private Keyframe[] offsets;

	private Keyframe[] alphas;

	private Keyframe[] scales;

	public bool UniqueEntries;

	public bool isVisible
	{
		get
		{
			return mList.Count != 0;
		}
	}

	private static int Comparison(Entry a, Entry b)
	{
		if (a.movementStart < b.movementStart)
		{
			return -1;
		}
		if (a.movementStart > b.movementStart)
		{
			return 1;
		}
		return 0;
	}

	private void Start()
	{
		mList = new List<Entry>(EntryCapacity);
		mUnused = new List<Entry>(EntryCapacity);
		for (int i = 0; i < EntryCapacity; i++)
		{
			Entry entry = new Entry();
			entry.time = Time.unscaledTime;
			entry.label = NGUITools.AddWidget<UILabel>(base.gameObject);
			entry.label.name = i.ToString();
			entry.label.effectStyle = effect;
			entry.label.font = font;
			entry.label.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			NGUITools.SetActive(entry.label.gameObject, false);
			mUnused.Add(entry);
		}
		offsets = offsetCurve.keys;
		alphas = alphaCurve.keys;
		scales = scaleCurve.keys;
	}

	private Entry Create()
	{
		if (mUnused.Count > 0)
		{
			nextRemoveEntryIndex = 0;
			Entry entry = mUnused[mUnused.Count - 1];
			mUnused.RemoveAt(mUnused.Count - 1);
			entry.time = Time.unscaledTime;
			entry.label.depth = NGUITools.CalculateNextDepth(base.gameObject);
			NGUITools.SetActive(entry.label.gameObject, true);
			entry.offset = 0f;
			mList.Add(entry);
			return entry;
		}
		Entry entry2 = mList[nextRemoveEntryIndex];
		if (nextRemoveEntryIndex + 1 == mList.Count)
		{
			nextRemoveEntryIndex = 0;
		}
		else
		{
			nextRemoveEntryIndex++;
		}
		entry2.time = Time.unscaledTime;
		entry2.label.depth = NGUITools.CalculateNextDepth(base.gameObject);
		NGUITools.SetActive(entry2.label.gameObject, true);
		entry2.offset = 0f;
		return entry2;
	}

	private bool EntryExists(string val)
	{
		int num = mList.Count;
		while (num > 0)
		{
			Entry entry = mList[--num];
			if (entry.label.text == val)
			{
				return true;
			}
		}
		return false;
	}

	private void Delete(Entry ent)
	{
		mList.Remove(ent);
		mUnused.Add(ent);
		NGUITools.SetActive(ent.label.gameObject, false);
	}

	public void Add(object obj, Color c, float stayDuration)
	{
		if (!base.enabled)
		{
			return;
		}
		float unscaledTime = Time.unscaledTime;
		bool flag = false;
		float num = 0f;
		if (obj is float)
		{
			flag = true;
			num = (float)obj;
		}
		else if (obj is int)
		{
			flag = true;
			num = (int)obj;
		}
		if (flag)
		{
			if (num == 0f)
			{
				return;
			}
			int num2 = mList.Count;
			while (num2 > 0)
			{
				Entry entry = mList[--num2];
				if (!(entry.time + 1f < unscaledTime) && entry.val != 0f)
				{
					if (entry.val < 0f && num < 0f)
					{
						entry.val += num;
						entry.label.text = Mathf.RoundToInt(entry.val).ToString();
						return;
					}
					if (entry.val > 0f && num > 0f)
					{
						entry.val += num;
						entry.label.text = "+" + Mathf.RoundToInt(entry.val);
						return;
					}
				}
			}
		}
		string text = ((!flag) ? obj.ToString() : ((!(num < 0f)) ? ("+" + Mathf.RoundToInt(num)) : Mathf.RoundToInt(num).ToString()));
		if (flag || !UniqueEntries || !EntryExists(text))
		{
			Entry entry2 = Create();
			entry2.stay = stayDuration;
			entry2.label.color = c;
			entry2.val = num;
			entry2.label.text = text;
			mList.Sort(Comparison);
		}
	}

	public void Add(object obj, Color c, float stayDuration, string textPrefix)
	{
		if (!base.enabled)
		{
			return;
		}
		float unscaledTime = Time.unscaledTime;
		bool flag = false;
		float num = 0f;
		if (obj is float)
		{
			flag = true;
			num = (float)obj;
		}
		else if (obj is int)
		{
			flag = true;
			num = (int)obj;
		}
		if (flag)
		{
			if (num == 0f)
			{
				return;
			}
			int num2 = mList.Count;
			while (num2 > 0)
			{
				Entry entry = mList[--num2];
				if (!(entry.time + 1f < unscaledTime) && entry.val != 0f)
				{
					if (entry.val < 0f && num < 0f)
					{
						entry.val += num;
						entry.label.text = textPrefix + Mathf.RoundToInt(entry.val);
						entry.time = Time.unscaledTime;
						mList.Sort(Comparison);
						return;
					}
					if (entry.val > 0f && num > 0f)
					{
						entry.val += num;
						entry.label.text = textPrefix + Mathf.RoundToInt(entry.val);
						entry.time = Time.unscaledTime;
						mList.Sort(Comparison);
						return;
					}
				}
			}
		}
		Entry entry2 = Create();
		entry2.stay = stayDuration;
		entry2.label.color = c;
		entry2.val = num;
		if (flag)
		{
			entry2.label.text = textPrefix + ((!(num < 0f)) ? (string.Empty + Mathf.RoundToInt(entry2.val)) : Mathf.RoundToInt(entry2.val).ToString());
		}
		else
		{
			entry2.label.text = textPrefix + obj.ToString();
		}
		mList.Sort(Comparison);
	}

	private void OnDisable()
	{
		int num = mList.Count;
		while (num > 0)
		{
			Entry entry = mList[--num];
			if (entry.label != null)
			{
				entry.label.enabled = false;
			}
			else
			{
				mList.RemoveAt(num);
			}
		}
	}

	private void Update()
	{
		float unscaledTime = Time.unscaledTime;
		float time = offsets[offsets.Length - 1].time;
		float time2 = alphas[alphas.Length - 1].time;
		float time3 = scales[scales.Length - 1].time;
		float num = Mathf.Max(time3, Mathf.Max(time, time2));
		int num2 = mList.Count;
		while (num2 > 0)
		{
			Entry entry = mList[--num2];
			float num3 = unscaledTime - entry.movementStart;
			entry.offset = offsetCurve.Evaluate(num3);
			entry.label.alpha = alphaCurve.Evaluate(num3 + 0.5f);
			float num4 = scaleCurve.Evaluate(unscaledTime - entry.time) * (float)entry.label.font.size;
			if (num4 < 0.001f)
			{
				num4 = 0.001f;
			}
			entry.label.cachedTransform.localScale = new Vector3(num4, num4, num4);
			if (num3 > num)
			{
				Delete(entry);
			}
			else
			{
				entry.label.enabled = true;
			}
		}
		float a = 0f;
		int num5 = mList.Count;
		while (num5 > 0)
		{
			Entry entry2 = mList[--num5];
			a = Mathf.Max(a, entry2.offset);
			entry2.label.cachedTransform.localPosition = new Vector3(0f, a, 0f);
			a += Mathf.Round(entry2.label.cachedTransform.localScale.y);
		}
	}
}
