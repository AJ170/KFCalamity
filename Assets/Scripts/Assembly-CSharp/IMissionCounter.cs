public interface IMissionCounter
{
	bool IsComplete { get; }

	bool NeedsUpdate { get; set; }

	bool ShowCounter { get; }

	QuestEntry.EntryType DisplayType { get; }

	int GetCurrentCount();

	int GetMaxCount();
}
