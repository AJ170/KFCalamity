using UnityEngine;

public class AbilityButtonBase : MonoBehaviour
{
	protected string iconSpriteName;

	protected int charges;

	protected bool usingCharges;

	protected bool onCooldown;

	protected float timeLeft;

	protected float totalTime;

	public GameObject Icon;

	public GameObject Cooldown;

	public GameObject Charges;

	public GameObject Overlay;

	public UISprite Background;

	public TweenAlpha OnPressFlashAlphaTween;

	public TweenAlpha CoolDownFlashAlphaTween;

	public TweenScale CoolDownFlashScaleTween;

	public float Alpha = 1f;

	protected UIButton button;

	protected UILabel buttonCooldown;

	protected UILabel buttonChargesLabel;

	protected UISprite buttonOverlay;

	protected UISprite buttonIcon;

	protected BoxCollider buttonCollider;

	public int AbilityCharges
	{
		get
		{
			return charges;
		}
	}

	public float CurrentCooldown
	{
		get
		{
			return timeLeft;
		}
	}

	public string IconName
	{
		get
		{
			return buttonIcon.spriteName;
		}
	}

	public void ToggleButtonActive(bool isActive)
	{
		if (buttonCollider != null)
		{
			buttonCollider.enabled = isActive;
		}
	}

	private void Awake()
	{
		button = base.gameObject.GetComponent<UIButton>();
		buttonOverlay = Overlay.GetComponent<UISprite>();
		buttonCooldown = Cooldown.GetComponent<UILabel>();
		buttonChargesLabel = Charges.GetComponent<UILabel>();
		buttonIcon = Icon.GetComponent<UISprite>();
		buttonCollider = GetComponent<BoxCollider>();
		if (Background != null)
		{
			Background.alpha = Alpha;
			buttonIcon.alpha = Alpha;
		}
	}

	public virtual void Start()
	{
		if (!string.IsNullOrEmpty(iconSpriteName))
		{
			SetIconImage(iconSpriteName);
		}
		if (charges > 0)
		{
			SetCharges(charges);
		}
		if (onCooldown)
		{
			SetCooldown(timeLeft);
		}
	}

	public virtual void Update()
	{
		if (onCooldown)
		{
			OnCooldownTick();
		}
	}

	private void TriggerOffCooldownFlash()
	{
		if (CoolDownFlashAlphaTween != null && CoolDownFlashScaleTween != null)
		{
			CoolDownFlashAlphaTween.gameObject.SetActive(true);
			CoolDownFlashAlphaTween.enabled = true;
			CoolDownFlashAlphaTween.Reset();
			CoolDownFlashAlphaTween.callWhenFinished = "OffCooldownFlashComplete";
			CoolDownFlashAlphaTween.eventReceiver = base.gameObject;
			CoolDownFlashAlphaTween.Play(true);
			CoolDownFlashScaleTween.enabled = true;
			CoolDownFlashScaleTween.Reset();
			CoolDownFlashScaleTween.Play(true);
		}
	}

	public void OffCooldownFlashComplete()
	{
		if (CoolDownFlashAlphaTween != null)
		{
			CoolDownFlashAlphaTween.gameObject.SetActive(false);
		}
	}

	private void TriggerOnPressFlash()
	{
		if (OnPressFlashAlphaTween != null)
		{
			OnPressFlashAlphaTween.gameObject.SetActive(true);
			OnPressFlashAlphaTween.enabled = true;
			OnPressFlashAlphaTween.callWhenFinished = "OnPressFlashComplete";
			OnPressFlashAlphaTween.eventReceiver = base.gameObject;
			OnPressFlashAlphaTween.Reset();
			OnPressFlashAlphaTween.Play(true);
		}
	}

	public void OnPressFlashComplete()
	{
		if (OnPressFlashAlphaTween != null)
		{
			OnPressFlashAlphaTween.gameObject.SetActive(false);
		}
	}

	public virtual void OnCooldownTick()
	{
		timeLeft -= Time.deltaTime;
		buttonCooldown.text = Mathf.Ceil(timeLeft).ToString();
		buttonOverlay.fillAmount = timeLeft / totalTime;
		if (timeLeft <= 0f)
		{
			onCooldown = false;
			buttonCooldown.text = string.Empty;
			buttonCooldown.gameObject.SetActive(false);
			if (usingCharges)
			{
				if (charges <= 0)
				{
					buttonOverlay.gameObject.SetActive(true);
					buttonOverlay.fillAmount = 1f;
				}
				else
				{
					buttonOverlay.gameObject.SetActive(false);
				}
			}
			else
			{
				buttonOverlay.gameObject.SetActive(false);
			}
		}
		else
		{
			buttonCooldown.gameObject.SetActive(true);
		}
	}

	public virtual void SetCooldown(float time)
	{
		timeLeft = time;
		totalTime = time;
		onCooldown = true;
		if (buttonCooldown != null)
		{
			TriggerOnPressFlash();
			buttonCooldown.text = Mathf.Ceil(timeLeft).ToString();
			buttonOverlay.gameObject.SetActive(true);
			buttonOverlay.fillAmount = 1f;
		}
	}

	public virtual void StopCooldown()
	{
		buttonCooldown.text = string.Empty;
		if (usingCharges)
		{
			if (charges <= 0)
			{
				buttonOverlay.gameObject.SetActive(true);
				buttonChargesLabel.gameObject.SetActive(false);
				buttonOverlay.fillAmount = 1f;
			}
			else
			{
				buttonOverlay.gameObject.SetActive(false);
				buttonChargesLabel.gameObject.SetActive(true);
				TriggerOffCooldownFlash();
			}
		}
		else
		{
			buttonOverlay.gameObject.SetActive(false);
			TriggerOffCooldownFlash();
		}
		onCooldown = false;
	}

	public virtual void SetCharges(int amount)
	{
		usingCharges = true;
		if ((bool)buttonChargesLabel)
		{
			buttonChargesLabel.text = amount.ToString();
			if (amount <= 0)
			{
				if (buttonCollider != null)
				{
					buttonCollider.enabled = false;
				}
				buttonChargesLabel.gameObject.SetActive(false);
			}
			else
			{
				if (buttonCollider != null && !buttonCollider.enabled)
				{
					buttonCollider.enabled = true;
				}
				buttonChargesLabel.gameObject.SetActive(true);
			}
			if (!onCooldown)
			{
				buttonOverlay.gameObject.SetActive(false);
				if (charges < amount)
				{
					TriggerOffCooldownFlash();
				}
			}
		}
		charges = amount;
	}

	public virtual void SetIconImage(string icon)
	{
		iconSpriteName = icon;
		if ((bool)buttonIcon)
		{
			buttonIcon.spriteName = icon;
			buttonIcon.MakePixelPerfect();
		}
	}
}
