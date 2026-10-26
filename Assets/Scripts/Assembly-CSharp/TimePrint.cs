using UnityEngine;

public class TimePrint
{
	private float totalTime;

	private float timeLeft;

	private float letterDelay;

	private float letterDelayCount;

	private int letterCount;

	private int textCount;

	private int returnCount;

	private string returnLetters = string.Empty;

	private string textMessage;

	private bool calculateTime;

	private bool numberMessage;

	public TimePrint()
	{
	}

	public TimePrint(string message, float duration, bool fixedDuration)
	{
		AddMessage(message, duration, fixedDuration);
	}

	public void Update()
	{
		if (calculateTime)
		{
			OnTimerTick();
		}
	}

	public void AddMessage(string message, float duration, bool fixedDuration)
	{
		if (fixedDuration)
		{
			textMessage = message;
			totalTime = 1f;
			timeLeft = duration * (float)message.Length;
			calculateTime = true;
			letterDelay = duration;
			letterCount = 0;
			letterDelayCount = timeLeft - letterDelay;
		}
		else
		{
			textMessage = message;
			totalTime = 1f;
			timeLeft = duration;
			calculateTime = true;
			letterDelay = duration / (float)message.Length;
			letterCount = 0;
			letterDelayCount = duration - letterDelay;
		}
	}

	public void AddMessage(int amount, float duration, bool fixedDuration)
	{
		returnCount = 0;
		numberMessage = true;
		if (fixedDuration)
		{
			textCount = amount;
			totalTime = 1f;
			timeLeft = duration * (float)amount;
			calculateTime = true;
			letterDelay = duration;
			letterCount = 0;
			letterDelayCount = timeLeft - letterDelay;
		}
		else
		{
			textCount = amount;
			totalTime = 1f;
			timeLeft = duration;
			calculateTime = true;
			letterDelay = duration / (float)amount;
			letterCount = 0;
			letterDelayCount = timeLeft - letterDelay;
		}
	}

	private void OnTimerTick()
	{
		timeLeft -= Time.deltaTime / totalTime;
		while (timeLeft <= letterDelayCount)
		{
			if (!numberMessage)
			{
				if (letterCount < textMessage.Length)
				{
					returnLetters += textMessage[letterCount];
					letterDelayCount -= letterDelay;
					letterCount++;
					continue;
				}
				calculateTime = false;
				break;
			}
			if (letterCount < textCount)
			{
				returnCount++;
				letterDelayCount -= letterDelay;
				letterCount++;
				continue;
			}
			numberMessage = false;
			calculateTime = false;
			break;
		}
	}

	public void Stop()
	{
		returnLetters = textMessage;
		calculateTime = false;
	}

	public string GetCurrentMessage()
	{
		return returnLetters;
	}

	public int GetCurrentCount()
	{
		return returnCount;
	}

	public bool isTyping()
	{
		return calculateTime;
	}
}
