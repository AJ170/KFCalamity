using UnityEngine;

public class WeaponUnlock : MonoBehaviour
{
	public Transform Text;

	public Transform Image;

	private bool initialized;

	private UILabel label;

	private UISprite sprite;

	private TweenAlpha labelAlphaTween;

	private TweenAlpha spriteAlphaTween;

	private void Start()
	{
		initialized = true;
		if (Image != null)
		{
			spriteAlphaTween = Image.GetComponent<TweenAlpha>();
			sprite = Image.GetComponent<UISprite>();
		}
		else
		{
			initialized = false;
		}
		if (Text != null)
		{
			labelAlphaTween = Text.GetComponent<TweenAlpha>();
			label = Text.GetComponent<UILabel>();
		}
		else
		{
			initialized = false;
		}
		if (initialized)
		{
			OnUnlockDone();
		}
		else
		{
			Debug.Log("<WeaponUnlock> Could not find components on WeaponUnlock prefab.");
		}
	}

	public void OnWeaponUnlocked(Weapon weapon)
	{
		if (initialized && weapon != null)
		{
			spriteAlphaTween.Reset();
			labelAlphaTween.Reset();
			Image.gameObject.SetActive(true);
			sprite.spriteName = weapon.Image;
			label.text = weapon.Name + " " + Localizer.GetString("gui_unlocked");
			spriteAlphaTween.Play(true);
			labelAlphaTween.Play(true);
		}
	}

	public void OnUnlockDone()
	{
		label.text = string.Empty;
		Image.gameObject.SetActive(false);
	}
}
