using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
	public TweenPosition PositionTween;

	public UILabel LabelBoss;

	private UILabel labelWave;

	private UILabel labelEnemy;

	private UILabel labelWaveCounter;

	private UILabel labelEnemyCounter;

	public Transform WaveText;

	public Transform EnemyText;

	public Transform WaveCounter;

	public Transform EnemyCounter;

	public Transform Background;

	public TweenAlpha Alpha;

	private List<GameObject> colliderList = new List<GameObject>();

	private bool isBossWave;

	private int enemiesLeft;

	public int EnemiesLeft
	{
		get
		{
			return enemiesLeft;
		}
	}

	public void Awake()
	{
		labelEnemyCounter = EnemyCounter.GetComponent<UILabel>();
		labelEnemy = EnemyText.GetComponent<UILabel>();
		labelWave = WaveText.GetComponent<UILabel>();
		labelWaveCounter = WaveCounter.GetComponent<UILabel>();
		labelWave.text = Localizer.GetString("gui_wave");
		labelEnemy.text = Localizer.GetString("gui_enemies_left");
	}

	public virtual void Start()
	{
	}

	public virtual void TweenIn()
	{
		PositionTween.enabled = true;
		PositionTween.Play(true);
	}

	public virtual void TweenOut()
	{
		PositionTween.Play(false);
	}

	private void FadeOut()
	{
		Alpha.Reset2();
		Alpha.to = 0.5f;
		Alpha.from = Alpha.alpha;
		Alpha.enabled = true;
	}

	private void FadeIn()
	{
		Alpha.Reset2();
		Alpha.to = 1f;
		Alpha.from = Alpha.alpha;
		Alpha.enabled = true;
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (!colliderList.Contains(collider.gameObject))
		{
			colliderList.Add(collider.gameObject);
			if (colliderList.Count == 1)
			{
				FadeOut();
			}
		}
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		if (colliderList.Contains(collider.gameObject))
		{
			colliderList.Remove(collider.gameObject);
			if (colliderList.Count == 0)
			{
				FadeIn();
			}
		}
	}

	public virtual void CheckForNullColliders()
	{
		if (colliderList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < colliderList.Count; i++)
		{
			if (colliderList[i] == null || !colliderList[i].activeSelf)
			{
				colliderList.RemoveAt(i);
				break;
			}
		}
		if (colliderList.Count == 0)
		{
			FadeIn();
		}
	}

	public virtual void Update()
	{
		CheckForNullColliders();
	}

	public virtual void SetWave(int wave, int maxWaves)
	{
		wave = Mathf.Max(wave, 0);
		maxWaves = Mathf.Max(maxWaves, 1);
		if (!isBossWave)
		{
			labelWaveCounter.text = wave + "/" + (maxWaves - 1);
			TweenIn();
		}
	}

	public virtual void SetEnemiesLeft(int enemies)
	{
		if (!isBossWave)
		{
			labelEnemyCounter.text = enemies.ToString();
			enemiesLeft = enemies;
		}
	}

	public virtual void SetBossWave()
	{
		LabelBoss.gameObject.SetActive(true);
		labelWaveCounter.gameObject.SetActive(false);
		labelEnemyCounter.gameObject.SetActive(false);
		labelWave.gameObject.SetActive(false);
		labelEnemy.gameObject.SetActive(false);
		isBossWave = true;
		LabelBoss.text = Localizer.GetString("gui_wave_boss");
		labelWaveCounter.text = string.Empty;
		labelEnemyCounter.text = string.Empty;
		labelWave.text = string.Empty;
		labelEnemy.text = string.Empty;
	}

	public virtual void SetGameInfoVisibility(bool visible)
	{
		if (visible)
		{
			TweenIn();
		}
		else
		{
			TweenOut();
		}
	}
}
