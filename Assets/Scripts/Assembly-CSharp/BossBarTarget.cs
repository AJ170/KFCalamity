using UnityEngine;

public class BossBarTarget : MonoBehaviour
{
	private GuiInGameManager guiManager;

	private void Start()
	{
		guiManager = GuiInGameManager.Instance;
		if ((bool)guiManager)
		{
			guiManager.ShowBossBar();
			guiManager.SetBossBarTarget(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (guiManager != null)
		{
			guiManager.HideBossBar();
		}
	}
}
