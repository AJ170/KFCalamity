using UnityEngine;

public class Boot : MonoBehaviour
{
	private Game _game;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		_game = new Game();
	}

	private void Update()
	{
		_game.Update();
	}
}
