public interface IMetaDamageBonuses
{
	float GetDamageMultiplier();

	float GetDamageMultiplier(Perk perk);

	float GetHeadshotMultiplier(Perk perk);
}
