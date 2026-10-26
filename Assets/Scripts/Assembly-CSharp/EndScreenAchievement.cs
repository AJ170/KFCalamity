using UnityEngine;

public class EndScreenAchievement : MonoBehaviour
{
	public UISprite Sprite;

	public UISprite BoundingBox;

	public UILabel Label;

	public UILabel CountLabel;

	public TweenScale Scale;

	public TweenScale LabelScale;

	public TweenScale CountScale;

	public TweenAlpha Alpha;

	public TweenAlpha SpriteAlpha;

	public string Name;

	public Perk Perk = Perk.None;

	public bool IsUnlock;

	public EndScreen.AchievementType Type;

	private int updateCount = 1;

	private void Start()
	{
	}

	public void Hide()
	{
		base.enabled = false;
	}

	public void Show()
	{
		base.enabled = true;
	}

	public void StartTween()
	{
		Alpha.Reset();
		Alpha.Play(true);
		Alpha.enabled = true;
		SpriteAlpha.Reset();
		SpriteAlpha.Play(true);
		SpriteAlpha.enabled = true;
		Scale.Reset();
		Scale.Play(true);
		Scale.enabled = true;
		LabelScale.Reset();
		LabelScale.Play(true);
		LabelScale.enabled = true;
		base.enabled = true;
		Sprite.alpha = 1f;
		Label.alpha = 1f;
		CountLabel.alpha = 1f;
	}

	public void FadeIn()
	{
		SpriteAlpha.Reset2();
		SpriteAlpha.duration = 0.2f;
		SpriteAlpha.Play(true);
		SpriteAlpha.enabled = true;
		Alpha.Reset2();
		Alpha.duration = 0.2f;
		Alpha.Play(true);
		Alpha.enabled = true;
	}

	public void FadeOut()
	{
		SpriteAlpha.Reset2();
		SpriteAlpha.duration = 0.2f;
		SpriteAlpha.Play(false);
		SpriteAlpha.enabled = true;
		Alpha.Reset2();
		Alpha.duration = 0.2f;
		Alpha.Play(false);
		Alpha.enabled = true;
	}

	public void SetType(EndScreen.AchievementType type)
	{
		Type = type;
	}

	public void SetPerk(Perk perk)
	{
		Perk = perk;
	}

	public void SetName(string name)
	{
		Name = name;
	}

	public void SetImage(string image)
	{
		Sprite.spriteName = image;
		Sprite.MakePixelPerfect();
	}

	public void SetText(string text)
	{
		Label.text = text;
	}

	public void IncreaseUpdateCount()
	{
		updateCount++;
		if (updateCount > 1)
		{
			CountScale.Reset2();
			CountScale.Play(true);
			CountLabel.enabled = true;
			CountLabel.text = "x" + updateCount;
		}
	}

	public void SetDepth(int depth)
	{
		BoundingBox.depth = depth;
		Sprite.depth = depth + 1;
		Label.depth = depth + 2;
	}
}
