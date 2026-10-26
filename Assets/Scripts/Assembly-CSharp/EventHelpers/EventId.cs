namespace EventHelpers
{
	public enum EventId
	{
		None = 0,
		WaveCreate = 1,
		WaveStart = 2,
		WaveEnd = 3,
		WaveUnitsKilled = 4,
		AllWavesCleared = 5,
		CreateUnit = 6,
		CreateSoldier = 7,
		DestroyUnit = 8,
		DestroySoldier = 9,
		PlayerAdd = 10,
		PlayerRemove = 11,
		AllSoldiersKilled = 12,
		PerkLevelUp = 13,
		WeaponUnlocked = 14,
		WeaponLevelUp = 15,
		ZedTimeStart = 16,
		ZedTimeEnd = 17,
		PauseToggled = 18
	}
}
