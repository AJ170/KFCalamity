using System.Collections.Generic;
using UnityEngine;

public class ActionToggleUnitBar : TutorialSubStepBase
{
	public string UnitBarKey;

	public override void OnStart()
	{
		UnitBar unitBar = TutorialManager.GetUnitBar(UnitBarKey);
		unitBar.gameObject.SetActive(!unitBar.gameObject.activeSelf);
		Object.Destroy(unitBar.gameObject);
		UnitBarLaggingHack();
		SetComplete(true);
	}

	private void UnitBarLaggingHack()
	{
		List<Player> allPlayers = TutorialManager.GameModeRef().GameStateRef().Context.PlayerManager.GetAllPlayers();
		UIFollowTarget component = allPlayers[0].PlayerGUI.UnitBarObject.GetComponent<UIFollowTarget>();
		component.enabled = false;
		component.enabled = true;
	}
}
