using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
	private class ListItemObject
	{
		public TweenPosition PosTween;

		public UISprite ItemSprite;

		public Transform ItemTransform;

		public int ListIndex;

		public float CurrentX;

		public ListItemObject(GameObject listItem, int listIndex)
		{
			ItemSprite = listItem.GetComponent<UISprite>();
			PosTween = listItem.GetComponent<TweenPosition>();
			ItemTransform = listItem.GetComponent<Transform>();
			ListIndex = listIndex;
		}
	}

	private class ListDataItem
	{
		public string SpriteName;

		public InventoryWeaponSlot Slot;

		public Weapon Weapon;

		public bool HasAmmo;

		public ListDataItem(string spriteName, InventoryWeaponSlot slot, Weapon weapon)
		{
			SpriteName = spriteName;
			Slot = slot;
			Weapon = weapon;
			HasAmmo = true;
		}
	}

	public GameObject ListItemPrefab;

	public float ItemOffset = 160f;

	public float DragTweenStartOffsetX = 20f;

	public float DragTWeenStartOfsetY = 15f;

	public UILabel CurrentAmmo;

	public UILabel MaxAmmo;

	public UILabel AmmoDivider;

	public UISprite CurrentAmmoBar;

	public UISprite BackgroundAmmoBar;

	public Action<InventoryWeaponSlot> OnWeaponSwitch;

	public Action<InventoryWeaponSlot> OnWeaponRemoved;

	private Vector2 pressPosition = Vector2.zero;

	private bool isTweening;

	private int currentIndex;

	private float horizontalDragLength;

	private float verticalDragLength;

	private Vector2 multiplier;

	private Dictionary<InventoryWeaponSlot, PlayerHudBase.InventoryWeapon> inventoryWeapons;

	private PlayerWeaponController playerWeaponController;

	private BoxCollider boxCollider;

	private Transform dragTransform;

	private List<ListDataItem> listItemData = new List<ListDataItem>();

	private List<ListItemObject> listItemObjects = new List<ListItemObject>();

	public int Index
	{
		get
		{
			return currentIndex;
		}
	}

	public InventoryWeaponSlot CurrentSlot
	{
		get
		{
			return listItemData[listItemObjects[1].ListIndex].Slot;
		}
	}

	public void SetInventoryWeapons(Dictionary<InventoryWeaponSlot, PlayerHudBase.InventoryWeapon> inventoryWeapons)
	{
		this.inventoryWeapons = inventoryWeapons;
	}

	public void SyncGUI(InventoryWeaponSlot equippedSlot)
	{
		foreach (KeyValuePair<InventoryWeaponSlot, PlayerHudBase.InventoryWeapon> inventoryWeapon in inventoryWeapons)
		{
			if (inventoryWeapon.Value.WeaponState != PlayerHudBase.WeaponSlotState.None)
			{
				Weapon weapon = inventoryWeapon.Value.Weapon;
				string spriteName = weapon.TouchImage + "_white";
				AddItem(spriteName, weapon, inventoryWeapon.Key);
			}
		}
	}

	public void SetCurrentSlotState(bool hasAmmo)
	{
		Weapon weapon = inventoryWeapons[CurrentSlot].Weapon;
		UISprite itemSprite = listItemObjects[1].ItemSprite;
		if (!hasAmmo)
		{
			itemSprite.spriteName = weapon.TouchImage + "_red";
		}
		else
		{
			itemSprite.spriteName = weapon.TouchImage + "_white";
		}
		itemSprite.MakePixelPerfect();
	}

	private PlayerWeaponController GetPlayerWeaponController()
	{
		if (playerWeaponController != null)
		{
			return playerWeaponController;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("Soldier");
		playerWeaponController = gameObject.GetComponent<PlayerWeaponController>();
		return playerWeaponController;
	}

	private void ToggleAmmoVisible(bool toggle)
	{
		MaxAmmo.enabled = toggle;
		CurrentAmmo.enabled = toggle;
		AmmoDivider.enabled = toggle;
		CurrentAmmoBar.enabled = toggle;
		BackgroundAmmoBar.enabled = toggle;
	}

	public void UpdateAmmo()
	{
		ListDataItem listDataItem = listItemData[listItemObjects[1].ListIndex];
		PlayerHudBase.InventoryWeapon inventoryWeapon = inventoryWeapons[listDataItem.Slot];
		if (inventoryWeapon.UseAmmunition)
		{
			if (!MaxAmmo.enabled)
			{
				ToggleAmmoVisible(true);
			}
			int magazineSize = inventoryWeapon.Data.Weapon.MagazineSize;
			int loadedAmmunition = inventoryWeapon.Data.LoadedAmmunition;
			int spareAmmunition = inventoryWeapon.Data.SpareAmmunition;
			MaxAmmo.text = spareAmmunition.ToString();
			CurrentAmmo.text = loadedAmmunition.ToString();
			float x = CurrentAmmoBar.transform.localScale.x;
			float x2 = BackgroundAmmoBar.transform.localScale.x;
			Vector3 localScale = CurrentAmmoBar.transform.localScale;
			float b = x2 * ((float)loadedAmmunition / (float)magazineSize);
			CurrentAmmoBar.transform.localScale = new Vector3(Mathf.Max(0.01f, b), localScale.y, localScale.z);
			bool flag = loadedAmmunition != 0 || spareAmmunition != 0;
			listDataItem.HasAmmo = flag || !listDataItem.Weapon.UseAmmunition;
			SetCurrentSlotState(flag);
		}
		else if (MaxAmmo.enabled)
		{
			ToggleAmmoVisible(false);
		}
	}

	public void OnWeaponEquipped(InventoryWeaponSlot slot)
	{
		if (slot != CurrentSlot)
		{
			int num = listItemData.FindIndex((ListDataItem a) => a.Slot == slot);
			currentIndex = num;
			InvalidateList();
		}
		UpdateAmmo();
	}

	public void AddItem(string spriteName, Weapon weapon, InventoryWeaponSlot slot)
	{
		ListDataItem listDataItem = listItemData.Find((ListDataItem a) => a.Slot == slot);
		InventoryWeaponSlot currentSlot = InventoryWeaponSlot.None;
		if (listItemData.Count > 0)
		{
			currentSlot = CurrentSlot;
		}
		if (listDataItem == null)
		{
			listDataItem = new ListDataItem(spriteName, slot, weapon);
			listItemData.Add(listDataItem);
			listItemData.Sort((ListDataItem a, ListDataItem b) => a.Slot.CompareTo(b.Slot));
		}
		else
		{
			listDataItem.SpriteName = spriteName;
		}
		if (currentSlot != InventoryWeaponSlot.None)
		{
			currentIndex = listItemData.FindIndex((ListDataItem a) => a.Slot == currentSlot);
		}
		InvalidateList();
	}

	public void RemoveWeapon(InventoryWeaponSlot slot)
	{
		int num = listItemData.FindIndex((ListDataItem a) => a.Slot == slot);
		if (num > -1)
		{
			listItemData.RemoveAt(num);
			InvalidateList();
			if (OnWeaponRemoved != null)
			{
				OnWeaponRemoved(slot);
			}
		}
	}

	private int NextWrappedIndex(int index, int direction)
	{
		if (listItemData.Count == 0)
		{
			return 0;
		}
		int num = index + direction;
		if (num < 0)
		{
			return listItemData.Count - 1;
		}
		return num % listItemData.Count;
	}

	private void InvalidateList()
	{
		if (currentIndex >= listItemData.Count)
		{
			currentIndex = Mathf.Max(0, listItemData.Count - 1);
		}
		listItemObjects[0].ListIndex = NextWrappedIndex(currentIndex, -1);
		listItemObjects[1].ListIndex = currentIndex;
		listItemObjects[2].ListIndex = NextWrappedIndex(currentIndex, 1);
		if (listItemData.Count > 0)
		{
			foreach (ListItemObject listItemObject in listItemObjects)
			{
				ListDataItem listDataItem = listItemData[listItemObject.ListIndex];
				string touchImage = listDataItem.Weapon.TouchImage;
				touchImage = (listDataItem.HasAmmo ? (touchImage + "_white") : (touchImage + "_red"));
				listItemObject.ItemSprite.enabled = true;
				listItemObject.ItemSprite.spriteName = touchImage;
				listItemObject.ItemSprite.MakePixelPerfect();
			}
			return;
		}
		listItemObjects[0].ItemSprite.enabled = false;
		listItemObjects[1].ItemSprite.enabled = false;
		listItemObjects[2].ItemSprite.enabled = false;
		ToggleAmmoVisible(false);
	}

	private void OnListItemTweenFinished(UITweener tweener)
	{
		ListItemObject listItemObject = listItemObjects.Find((ListItemObject a) => a.PosTween == tweener);
	}

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider>();
		for (int i = 0; i < 3; i++)
		{
			int num = i - 1;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ListItemPrefab);
			ListItemObject listItemObject = new ListItemObject(gameObject, num);
			UISprite component = gameObject.GetComponent<UISprite>();
			component.panel = GetComponentInParent<UIPanel>();
			component.enabled = false;
			listItemObject.ItemTransform.parent = base.transform;
			listItemObject.CurrentX = (float)num * ItemOffset;
			listItemObject.ItemTransform.localPosition = new Vector3((float)num * ItemOffset, 0f, 0f);
			listItemObjects.Add(listItemObject);
			listItemObject.ListIndex = 0;
		}
		MaxAmmo.text = string.Empty;
		CurrentAmmo.text = string.Empty;
		ToggleAmmoVisible(false);
	}

	private void Start()
	{
	}

	private void OnVerticalTweenFinished(UITweener tweener)
	{
		InventoryWeaponSlot slot = listItemData[listItemObjects[1].ListIndex].Slot;
		listItemData.RemoveAt(listItemObjects[1].ListIndex);
		InvalidateList();
		playerWeaponController.DropWeaponBySlot(slot);
		listItemObjects[1].ItemTransform.localPosition = Vector3.zero;
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			multiplier = Vector2.one;
			return;
		}
		isTweening = false;
		multiplier = Vector2.one;
		Vector3 localPosition = listItemObjects[1].ItemTransform.localPosition;
		if (localPosition.y < DragTWeenStartOfsetY)
		{
			localPosition.y = 0f;
		}
		if (localPosition.x < DragTweenStartOffsetX && localPosition.x > 0f - DragTweenStartOffsetX)
		{
			localPosition.x = 0f;
		}
		listItemObjects[1].ItemTransform.localPosition = localPosition;
	}

	private void MoveList(bool forward)
	{
		int num = 0;
		foreach (ListItemObject listItemObject2 in listItemObjects)
		{
			TweenPosition posTween = listItemObject2.PosTween;
			Vector3 localPosition = listItemObject2.ItemTransform.localPosition;
			posTween.Reset2();
			posTween.onFinished = null;
			posTween.from = localPosition;
			float num2 = listItemObject2.CurrentX + ItemOffset * (float)((!forward) ? 1 : (-1));
			posTween.to = new Vector3(num2, 0f, 0f);
			listItemObject2.CurrentX = num2;
			posTween.Play(true);
			num++;
		}
		int index = ((!forward) ? (listItemObjects.Count - 1) : 0);
		ListItemObject listItemObject = listItemObjects[index];
		listItemObjects.RemoveAt(index);
		if (forward)
		{
			listItemObjects.Add(listItemObject);
		}
		else
		{
			listItemObjects.Insert(0, listItemObject);
		}
		listItemObject.PosTween.enabled = false;
		listItemObject.CurrentX = ((!forward) ? (0f - ItemOffset) : ItemOffset);
		listItemObject.ItemTransform.localPosition = new Vector3(listItemObject.CurrentX, 0f, 0f);
		currentIndex = NextWrappedIndex(currentIndex, forward ? 1 : (-1));
		InvalidateList();
		if (OnWeaponSwitch != null)
		{
			InventoryWeaponSlot slot = listItemData[listItemObjects[1].ListIndex].Slot;
			OnWeaponSwitch(slot);
			GetPlayerWeaponController().SetActiveWeapon(slot);
			UpdateAmmo();
		}
		isTweening = true;
	}

	private void HandleHorizontalDrag(Vector2 delta)
	{
		foreach (ListItemObject listItemObject in listItemObjects)
		{
			listItemObject.ItemTransform.localPosition += new Vector3(delta.x * multiplier.x, 0f, 0f);
		}
		horizontalDragLength += delta.x * multiplier.x;
		if (Mathf.Abs(horizontalDragLength) > DragTweenStartOffsetX)
		{
			MoveList(Mathf.Sign(horizontalDragLength) < 0f);
			horizontalDragLength = 0f;
		}
	}

	private void HandleVerticalDrag(Vector2 delta)
	{
		Vector2 normalized = delta.normalized;
		if ((normalized.y > 0.75f || normalized.y < -0.75f) && GetPlayerWeaponController().CanDropWeapon(CurrentSlot))
		{
			Transform itemTransform = listItemObjects[1].ItemTransform;
			itemTransform.localPosition += new Vector3(0f, delta.y, 0f);
			multiplier.x = 0f;
			verticalDragLength += delta.y;
			if (verticalDragLength > DragTWeenStartOfsetY || verticalDragLength < 0f - DragTWeenStartOfsetY)
			{
				TweenPosition posTween = listItemObjects[1].PosTween;
				posTween.from = itemTransform.localPosition;
				posTween.to = itemTransform.localPosition + new Vector3(0f, 100f * Mathf.Sign(verticalDragLength), 0f);
				posTween.onFinished = OnVerticalTweenFinished;
				posTween.Play(true);
				isTweening = true;
				verticalDragLength = 0f;
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!isTweening && listItemData.Count > 1)
		{
			HandleHorizontalDrag(delta);
		}
		HandleVerticalDrag(delta);
	}

	private void Update()
	{
	}
}
