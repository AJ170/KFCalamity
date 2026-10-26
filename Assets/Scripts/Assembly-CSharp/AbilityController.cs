using System.Collections.Generic;
using EffectEvents;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
	public GameObject[] AbilitiyPrefabs = new GameObject[0];

	private GameObject abilityFolder;

	protected List<Ability> abilities = new List<Ability>();

	private bool firstUpdate = true;

	public int NumAbilities
	{
		get
		{
			return abilities.Count;
		}
	}

	protected virtual void Start()
	{
		CreateAbilityFolder();
		SetupAbilities();
	}

	private void Update()
	{
		if (firstUpdate)
		{
			StartPassiveAbilities();
			firstUpdate = false;
		}
	}

	private void OnDestroy()
	{
		for (int num = abilities.Count - 1; num >= 0; num--)
		{
			Object.Destroy(abilities[num]);
		}
		abilities.Clear();
	}

	private void SetupAbilities()
	{
		GameObject[] abilitiyPrefabs = AbilitiyPrefabs;
		foreach (GameObject abilityPrefab in abilitiyPrefabs)
		{
			SetupAbility(abilityPrefab);
		}
	}

	protected virtual Ability SetupAbility(GameObject abilityPrefab)
	{
		if (abilityPrefab != null)
		{
			GameObject gameObject = InstantiateAbility(abilityPrefab);
			Ability component = gameObject.GetComponent<Ability>();
			if (component != null)
			{
				abilities.Add(component);
				return component;
			}
			Debug.LogError("AbilityController:SetupAbility() -> Missing ability component in " + gameObject.name);
		}
		return null;
	}

	private void CreateAbilityFolder()
	{
		abilityFolder = new GameObject("Abilities");
		abilityFolder.transform.parent = base.transform;
		abilityFolder.transform.localPosition = Vector3.zero;
		abilityFolder.transform.localRotation = Quaternion.identity;
	}

	private GameObject InstantiateAbility(GameObject abilityPrefab)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(abilityPrefab);
		gameObject.transform.parent = abilityFolder.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.BroadcastMessage("OnMounted", base.gameObject, SendMessageOptions.DontRequireReceiver);
		return gameObject;
	}

	private void StartPassiveAbilities()
	{
		foreach (Ability ability in abilities)
		{
			if (ability.Type == AbilityType.Passive)
			{
				ability.TriggerInternal(new Cast(ability.Name));
			}
		}
	}

	public virtual bool CanCast(int abilityIndex)
	{
		return CanCast(AbilityByIndex(abilityIndex));
	}

	public virtual bool CanCast(Ability ability, bool ignoreCriterion = false)
	{
		if (ability != null)
		{
			return ability.CanCast(ignoreCriterion);
		}
		return false;
	}

	public virtual void Cast(int abilityIndex)
	{
		Ability ability = AbilityByIndex(abilityIndex);
		if (ability != null)
		{
			ability.Cast();
		}
	}

	public virtual void OnCastCompleted(Ability ability)
	{
	}

	public virtual void OnChargeChange(Ability ability)
	{
	}

	public virtual void OnTriggerCooldown(Ability ability)
	{
	}

	public virtual void OnCooldownOver(Ability ability)
	{
	}

	public Ability AbilityByIndex(int index)
	{
		if (index >= 0 && index < NumAbilities)
		{
			return abilities[index];
		}
		return null;
	}

	public Ability AbilityByName(string name)
	{
		foreach (Ability ability in abilities)
		{
			if (ability.Name == name)
			{
				return ability;
			}
		}
		return null;
	}
}
