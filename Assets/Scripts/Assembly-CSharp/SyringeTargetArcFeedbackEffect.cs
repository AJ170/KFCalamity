using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SyringeTargetArcFeedbackEffect : EffectBaseListener
{
	private enum FeedbackState
	{
		Inactive = 0,
		Active = 1,
		CastSuccess = 2,
		CastFailed = 3
	}

	public GameObject ArcOutlinePrefab;

	public GameObject ArcFillerPrefab;

	public Color CastSuccessColor = Color.green;

	public Color CastFailedColor = Color.red;

	private FeedbackState state;

	private LerpColor outlineColorLerp = new LerpColor();

	private LerpColor fillerColorLerp = new LerpColor();

	private AlphaLerp arcsAlphaLerp = new AlphaLerp(0f);

	private AlphaLerp fillerScaleLerp = new AlphaLerp(0f);

	private GameObject parent;

	private GameObject arcOutline;

	private GameObject arcFiller;

	private MaterialData arcOutlineMaterial;

	private MaterialData arcFillerMaterial;

	private float chargeUpTime;

	private float timer;

	protected override void Start()
	{
		base.Start();
		if (effectSystem != null)
		{
			EffectEventSystem eventSystem = effectSystem.EventSystem;
			if (Trigger != 0)
			{
				eventSystem.Unsubscribe(Trigger, this);
			}
			eventSystem.Subscribe(TriggerId.ChargeUpStart, Execute, this);
			eventSystem.Subscribe(TriggerId.ChargeUpRelease, Execute, this);
			eventSystem.Subscribe(TriggerId.ChargeUpMax, Execute, this);
		}
		if (ArcOutlinePrefab != null && ArcFillerPrefab != null)
		{
			arcOutline = Object.Instantiate(ArcOutlinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			arcFiller = Object.Instantiate(ArcFillerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			arcOutlineMaterial = new MaterialData(arcOutline.GetComponent<Renderer>().material);
			arcFillerMaterial = new MaterialData(arcFiller.GetComponent<Renderer>().material);
			Color defaultColor = arcOutlineMaterial.DefaultColor;
			defaultColor.a = 0f;
			outlineColorLerp.CurrentColor = defaultColor;
			arcOutlineMaterial.DefaultColor = defaultColor;
			arcOutlineMaterial.color = defaultColor;
			Color defaultColor2 = arcFillerMaterial.DefaultColor;
			defaultColor2.a = 0f;
			fillerColorLerp.CurrentColor = defaultColor2;
			arcFillerMaterial.DefaultColor = defaultColor2;
			arcFillerMaterial.color = defaultColor2;
			MountArcs(null);
			SetArcsActive(false);
		}
		else
		{
			Debug.LogError("SyringeTargetArcFeedbackEffect:Start() Missing Outline or Filler arc prefab");
		}
	}

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	protected override void Execute(TriggerBase e)
	{
		if (!(ArcOutlinePrefab != null) || !(ArcOutlinePrefab != null) || (!NoCustomEvents() && !MatchCustomEvent(e.CustomName)))
		{
			return;
		}
		if (e.Id == TriggerId.ChargeUpStart)
		{
			ChargeUpStart chargeUpStart = e as ChargeUpStart;
			state = FeedbackState.Inactive;
			chargeUpTime = chargeUpStart.ChargeUpTime;
			timer = 0.1f;
		}
		else if (e.Id == TriggerId.ChargeUpRelease)
		{
			if (state == FeedbackState.Inactive)
			{
				timer = 0f;
			}
			if (state == FeedbackState.Active)
			{
				timer = 0f;
				state = FeedbackState.CastFailed;
				StopScale();
				StartFadeOut();
			}
		}
		else if (e.Id == TriggerId.ChargeUpMax)
		{
			ChargeUpMax chargeUpMax = e as ChargeUpMax;
			bool success = chargeUpMax.Success;
			state = ((!success) ? FeedbackState.CastFailed : FeedbackState.CastSuccess);
			float time = 0.2f;
			StartColorBlend(success, time);
			timer = time;
		}
	}

	private void Update()
	{
		if (timer > 0f)
		{
			timer = Mathf.Max(timer - Time.deltaTime, 0f);
			if (timer == 0f)
			{
				if (state == FeedbackState.Inactive)
				{
					state = FeedbackState.Active;
					SetArcsActive(true);
					MountArcs(parent.transform);
					RestoreColorBlend();
					StartFadeIn();
					StartScale();
				}
				else if (state == FeedbackState.CastSuccess || state == FeedbackState.CastFailed)
				{
					StartFadeOut();
				}
			}
		}
		UpdateColor();
		UpdateScale();
	}

	private void UpdateColor()
	{
		if (outlineColorLerp.Lerping || fillerColorLerp.Lerping || arcsAlphaLerp.Lerping)
		{
			outlineColorLerp.Update();
			fillerColorLerp.Update();
			arcsAlphaLerp.Update();
			Color currentColor = outlineColorLerp.CurrentColor;
			Color currentColor2 = fillerColorLerp.CurrentColor;
			currentColor2.a = (currentColor.a = arcsAlphaLerp.Alpha);
			arcOutlineMaterial.color = currentColor;
			arcFillerMaterial.color = currentColor2;
		}
	}

	private void UpdateScale()
	{
		if (fillerScaleLerp.Lerping)
		{
			fillerScaleLerp.Update();
			float alpha = fillerScaleLerp.Alpha;
			arcFiller.transform.localScale = new Vector3(alpha, alpha, alpha);
		}
	}

	private void StartFadeIn(float fadeTime = 0.15f)
	{
		arcsAlphaLerp.Lerp(1f, fadeTime, null, true);
	}

	private void StartFadeOut(float fadeTime = 0.3f)
	{
		state = FeedbackState.Inactive;
		arcsAlphaLerp.Lerp(0f, fadeTime, OnFadeOutEnd, true);
	}

	private void OnFadeOutEnd()
	{
		SetArcsActive(false);
		MountArcs(null);
	}

	private void StartScale()
	{
		if (chargeUpTime > 0f)
		{
			fillerScaleLerp.Alpha = 0f;
			fillerScaleLerp.Lerp(1f, chargeUpTime, null, true);
		}
	}

	private void StopScale()
	{
		fillerScaleLerp.Stop();
	}

	private void StartColorBlend(bool castSuccess, float time)
	{
		Color goal = ((!castSuccess) ? CastFailedColor : CastSuccessColor);
		outlineColorLerp.Lerp(outlineColorLerp.CurrentColor, goal, 1f / time, null, true);
		fillerColorLerp.Lerp(fillerColorLerp.CurrentColor, goal, 1f / time, null, true);
	}

	private void RestoreColorBlend()
	{
		outlineColorLerp.Stop();
		fillerColorLerp.Stop();
		outlineColorLerp.CurrentColor = arcOutlineMaterial.DefaultColor;
		fillerColorLerp.CurrentColor = arcFillerMaterial.DefaultColor;
	}

	private void SetArcsActive(bool active)
	{
		arcOutline.SetActive(active);
		arcFiller.SetActive(active);
	}

	private void MountArcs(Transform transform)
	{
		if (transform == null)
		{
			transform = base.transform;
		}
		arcOutline.transform.parent = transform;
		arcFiller.transform.parent = transform;
		arcOutline.transform.localPosition = Vector3.zero;
		arcFiller.transform.localPosition = Vector3.zero;
		arcOutline.transform.localRotation = ArcOutlinePrefab.transform.rotation;
		arcFiller.transform.localRotation = ArcFillerPrefab.transform.rotation;
		arcFiller.transform.localScale = Vector3.zero;
	}
}
