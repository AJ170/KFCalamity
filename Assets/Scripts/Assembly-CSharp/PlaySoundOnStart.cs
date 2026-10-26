public class PlaySoundOnStart : PlaySoundBase
{
	public bool Loop;

	private void Start()
	{
		PlayRandomClip(Loop);
	}
}
