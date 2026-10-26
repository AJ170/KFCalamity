using System.Collections.Generic;
using UnityEngine;

public class GuiUnlockController : MonoBehaviour
{
	public class DisplayData
	{
		public string Image;

		public string Text;
	}

	public GameObject DisplayPrefab;

	private Queue<DisplayData> displayQueue = new Queue<DisplayData>();

	private MetaManager metaManager;

	private GuiInGameManager guiManager;

	private string levelUp;

	private string unlocked;

	private bool isReadyToSpawn = true;

	private bool isPaused;

	private GameObject activeDisplayObject;

	private void Start()
	{
		metaManager = MetaManager.Instance;
		unlocked = Localizer.GetString("gui_unlocked");
		levelUp = Localizer.GetString("gui_level");
	}

	public void Pause()
	{
		isPaused = true;
	}

	public void UnPause()
	{
		isPaused = false;
		if (displayQueue.Count > 0)
		{
			isReadyToSpawn = true;
			SpawnDisplayObject();
		}
	}

	public void SetManagerReference(GuiInGameManager manager)
	{
		guiManager = manager;
	}

	public void AddWeaponUnlock(Weapon weapon)
	{
		DisplayData displayData = new DisplayData();
		displayData.Image = weapon.Image;
		displayData.Text = Localizer.GetString(weapon.Image) + "\n" + unlocked;
		displayQueue.Enqueue(displayData);
		SpawnDisplayObject();
	}

	public void AddWeaponLevelUp(Weapon weapon)
	{
		int num = metaManager.WeaponLevel(weapon.Name);
		DisplayData displayData = new DisplayData();
		displayData.Image = weapon.Image;
		displayData.Text = Localizer.GetString(weapon.Image) + "\n" + levelUp + " " + num;
		displayQueue.Enqueue(displayData);
		SpawnDisplayObject();
	}

	public void AddPerkLevelUp(Perk perk)
	{
		int num = metaManager.PerkLevel(perk);
		DisplayData displayData = new DisplayData();
		displayData.Image = GetPerkLocalizerKey(perk);
		displayData.Text = Localizer.GetString(GetPerkLocalizerKey(perk)) + "\n" + levelUp + " " + num;
		displayQueue.Enqueue(displayData);
		SpawnDisplayObject();
	}

	public void OnDisplayObjectComplete()
	{
		Object.Destroy(activeDisplayObject);
		if (!isPaused)
		{
			if (displayQueue.Count > 0)
			{
				CreateDisplayObject();
			}
			else
			{
				isReadyToSpawn = true;
			}
		}
	}

	private void SpawnDisplayObject()
	{
		if (isReadyToSpawn && !isPaused)
		{
			CreateDisplayObject();
		}
	}

	private void CreateDisplayObject()
	{
		isReadyToSpawn = false;
		DisplayData displayData = displayQueue.Dequeue();
		activeDisplayObject = NGUITools.AddChild(base.gameObject, DisplayPrefab);
		WeaponAndPerkDisplay component = activeDisplayObject.GetComponent<WeaponAndPerkDisplay>();
		component.SetData(base.gameObject, "OnDisplayObjectComplete", displayData.Image, displayData.Text);
		guiManager.PlaySound(GuiInGameManager.Sound.HudUnlock);
	}

	private string GetPerkLocalizerKey(Perk perk)
	{
		string result = string.Empty;
		switch (perk)
		{
		case Perk.Berserker:
			result = "gui_berserker";
			break;
		case Perk.Commando:
			result = "gui_commando";
			break;
		case Perk.Demolition:
			result = "gui_demolition";
			break;
		case Perk.Firebug:
			result = "gui_firebug";
			break;
		case Perk.Medic:
			result = "gui_medic";
			break;
		case Perk.Sharpshooter:
			result = "gui_sharpshooter";
			break;
		case Perk.SupportSpecialist:
			result = "gui_support_specialist";
			break;
		case Perk.None:
			result = string.Empty;
			break;
		}
		return result;
	}
}
