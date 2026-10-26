public class PlayerStatusText
{
	private enum StatusTexts
	{
		None = 0,
		Reloading = 1,
		No_ammo = 2
	}

	private StatusTexts currentStatus;

	private void Start()
	{
	}

	public bool HasStatus()
	{
		if (currentStatus != 0)
		{
			return true;
		}
		return false;
	}

	public string GetCurrentStatus()
	{
		if (currentStatus == StatusTexts.No_ammo)
		{
			return Localizer.GetString("gui_player_status_empty_ammo");
		}
		if (currentStatus == StatusTexts.Reloading)
		{
			return Localizer.GetString("gui_player_status_reload");
		}
		return null;
	}

	public void SetNoAmmoStatus(bool isVisible)
	{
		if (currentStatus == StatusTexts.No_ammo)
		{
			if (!isVisible)
			{
				currentStatus = StatusTexts.None;
			}
		}
		else if (currentStatus == StatusTexts.None && isVisible)
		{
			currentStatus = StatusTexts.No_ammo;
		}
	}

	public void SetReloadStatus(bool isVisible)
	{
		if (isVisible)
		{
			currentStatus = StatusTexts.Reloading;
		}
		else
		{
			currentStatus = StatusTexts.None;
		}
	}
}
