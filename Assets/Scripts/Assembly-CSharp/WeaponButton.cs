using UnityEngine;

public class WeaponButton : MonoBehaviour
{
	public UISprite IconSprite;

	public UISprite BackgroundSprite;

	public GameObject SelectedSprite;

	public UISprite Trashcan;

	public UILabel Ammo;

	public void SetSelected(bool selected)
	{
		if (selected)
		{
			SelectedSprite.SetActive(true);
		}
		else
		{
			SelectedSprite.SetActive(false);
		}
	}

	public void OnEnabled()
	{
	}

	public void OnDisabled()
	{
	}

	public void UpdateAmmo(int currentAmmo, int maxAmmo)
	{
		if (Ammo != null)
		{
			Ammo.enabled = true;
			Ammo.text = currentAmmo + " | " + maxAmmo;
		}
	}

	public void ToggleAmmoVisible(bool visible)
	{
		if (Ammo != null)
		{
			Ammo.enabled = visible;
		}
	}

	public void SetAmmoColor(Color color)
	{
		if (Ammo != null)
		{
			Ammo.color = color;
		}
	}

	public void SetSlotActive(bool active)
	{
		if (IconSprite != null)
		{
			IconSprite.enabled = active;
		}
		if (Trashcan != null)
		{
			Trashcan.enabled = active;
		}
		if (Ammo != null)
		{
			Ammo.enabled = active;
		}
	}
}
