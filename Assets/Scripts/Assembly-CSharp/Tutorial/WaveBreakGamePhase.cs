using UnityEngine;

namespace Tutorial
{
	public class WaveBreakGamePhase : global::WaveBreakGamePhase
	{
		public WaveBreakGamePhase(GameModeTutorial tutorial, bool firstStart)
			: base(tutorial)
		{
		}

		protected override void GotoBossPhase()
		{
			Debug.LogWarning("Tutorial.WaveBreakGamePhase:GotoBossPhase() No support for boss phase");
		}

		protected override void GotoWavePhase()
		{
			gamePhaseManager.SetPhase(new WaveGamePhase(gameMode as GameModeTutorial));
			ToggleTutorialGui(true);
		}

		private void HideTutorialGui()
		{
			ToggleTutorialGui(false);
		}

		public void ToggleTutorialGui(bool visible)
		{
			GameModeTutorial gameModeTutorial = gameMode as GameModeTutorial;
			gameModeTutorial.TutorialManagerRef().ToggleQuestLog(visible);
			gameModeTutorial.TutorialManagerRef().ToggleTutorialMessage(visible);
		}

		public override void OnEnter()
		{
			base.OnEnter();
			commandQueue.Add(HideTutorialGui);
		}

		protected override void OpenShop()
		{
			GameModeTutorial gameModeTutorial = gameMode as GameModeTutorial;
			gameModeTutorial.TutorialManagerRef().DestroyTutorialMessage();
			base.OpenShop();
		}
	}
}
