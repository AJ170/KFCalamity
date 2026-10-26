public class TutorialPlayerHasItem : TutorialEventBase
{
	public enum ItemType
	{
		Grenade = 0,
		Syringes = 1
	}

	private PlayerAbilityController abilityController;

	private Ability ability;

	public ItemType Item;

	public int Amount;

	private void Start()
	{
	}

	private void Update()
	{
		if (ability.ChargesLeft() >= Amount)
		{
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		abilityController = Player.GetComponent<PlayerAbilityController>();
		if (Item == ItemType.Grenade)
		{
			ability = abilityController.Grenade.GetComponent<Ability>();
		}
		else if (Item == ItemType.Syringes)
		{
			ability = abilityController.AbilityBySlot(AbilitySlot.SyringeSelf);
		}
	}
}
