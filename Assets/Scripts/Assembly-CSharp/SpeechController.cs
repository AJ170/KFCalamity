using UnityEngine;

public class SpeechController : MonoBehaviour
{
	public SpeechManager.SpeechSetType Type;

	public GameObject FeedbackPrefab;

	public Transform FeedbackMount;

	public Vector3 FeedbackOffset = Vector3.zero;

	private GameObject feedbackInstance;

	private ParticleSystem feedbackSystem;

	private void Start()
	{
		if (FeedbackMount == null)
		{
			FeedbackMount = base.transform;
		}
	}

	public void DisplayFeedback()
	{
		if (feedbackInstance == null && FeedbackPrefab != null)
		{
			CreateFeedback();
		}
		if (feedbackInstance != null)
		{
			feedbackInstance.SetActive(true);
			if (feedbackSystem != null)
			{
				feedbackSystem.Clear();
				feedbackSystem.Play();
			}
		}
	}

	public void HideFeedback()
	{
		if (feedbackInstance != null)
		{
			feedbackInstance.SetActive(false);
		}
	}

	private void CreateFeedback()
	{
		if (FeedbackPrefab != null)
		{
			feedbackInstance = (GameObject)Object.Instantiate(FeedbackPrefab);
			feedbackSystem = feedbackInstance.GetComponent<ParticleSystem>();
			feedbackInstance.SetActive(false);
			Transform transform = feedbackInstance.transform;
			transform.parent = FeedbackMount;
			transform.localPosition = FeedbackOffset;
		}
	}

	public void OnWeaponSwitch(Weapon weapon)
	{
		string text = weapon.Name;
		SpeechManager.SpeechLine? speechLine = null;
		switch (text)
		{
		case "LAW":
			speechLine = SpeechManager.SpeechLine.LAWEquip;
			break;
		case "HuntingShotgun":
			speechLine = SpeechManager.SpeechLine.HuntingShotgunEquip;
			break;
		case "HandcannonDual":
			speechLine = SpeechManager.SpeechLine.DualHandcannonEquip;
			break;
		case "Axe":
			speechLine = SpeechManager.SpeechLine.FireAxeEquip;
			break;
		}
		if (speechLine.HasValue && Random.value < 0.2f)
		{
			SpeechManager.PlaySpeech(speechLine.Value, base.gameObject);
		}
	}
}
