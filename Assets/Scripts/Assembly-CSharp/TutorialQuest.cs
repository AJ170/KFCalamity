using UnityEngine;

public class TutorialQuest : MonoBehaviour
{
	public string QuestTextKey;

	public string QuestSymbolEntry;

	public TutorialEventBase ExternalMission;

	private TutorialEventBase mission;

	private IMissionCounter missionCounter;

	private GameObject questID;

	public GameObject QuestID
	{
		get
		{
			return questID;
		}
		set
		{
			questID = value;
		}
	}

	private void Start()
	{
		if (ExternalMission != null)
		{
			mission = ExternalMission;
		}
		else
		{
			mission = GetComponent<TutorialEventBase>();
		}
		missionCounter = mission as IMissionCounter;
		if (mission == null)
		{
			Debug.LogError("TutorialQuest:Start() - " + base.name + " has no mission objective attached!");
		}
	}

	public bool IsComplete()
	{
		return !(mission == null) && mission.IsComplete;
	}

	public bool HasMissionCounter()
	{
		return missionCounter != null;
	}

	public IMissionCounter GetMissionCounter()
	{
		return missionCounter;
	}
}
