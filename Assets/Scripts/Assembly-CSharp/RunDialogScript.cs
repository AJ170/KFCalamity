using UnityEngine;

public class RunDialogScript : MonoBehaviour
{
	public string SpeechLocalizerKey;

	public float SpeechTime;

	public bool CompletesOnClose;

	public TutorialSpeechLine SpeechLine = TutorialSpeechLine.None;

	private bool hasRun;

	private float timer;

	private bool isComplete;

	private TutorialManager tutorialManager;

	private void Start()
	{
		tutorialManager = base.transform.root.GetComponent<TutorialManager>();
	}

	private void OnMessageEnd()
	{
		isComplete = true;
	}

	public void StartDialog()
	{
		if (hasRun)
		{
			return;
		}
		string @string = Localizer.GetString(SpeechLocalizerKey);
		if (!string.IsNullOrEmpty(@string))
		{
			TutorialMessage tutorialMessage = tutorialManager.RunSpeech(@string, SpeechTime, OnMessageEnd);
			if (CompletesOnClose)
			{
				tutorialMessage.AddAlphaCompleteListener(OnMessageEnd);
			}
			else
			{
				isComplete = true;
			}
		}
		if (SpeechLine != TutorialSpeechLine.None)
		{
			TutorialSpeechManager.Play(SpeechLine);
		}
		timer = SpeechTime;
	}

	private void Update()
	{
		if (timer > 0f)
		{
			timer = Mathf.Max(0f, timer - Time.deltaTime);
		}
	}

	public bool CheckComplete()
	{
		return isComplete;
	}
}
