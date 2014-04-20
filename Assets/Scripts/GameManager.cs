using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager Get() {
		if (_instance == null) {
			GameObject singleton = new GameObject ();
			_instance = singleton.AddComponent<GameManager> ();
			singleton.name = "(singleton) " + typeof(GameManager).ToString ();

			DontDestroyOnLoad (singleton);
		}
		return _instance;
	}

	public bool GameFrozen {
		get;
		set; 
	}

	private const int maxLives = 3;

	private GameState gameState;
	private bool gameOver = false;
	private int lives = 3;
	private int currentLevel = 0;
	private AudioSource musicSource;

	private static GameManager _instance = null;
	
	public void RestartLevel() {
		Application.LoadLevel(currentLevel);
	}

	public void LoseLife() {
		lives--;
		if (lives <= -1) {
			gameState = GameState.GameOver;
//			GameFrozen = true;
//			gameOver = true;
//			GameObject obj = GameObject.Find("GameOverText");
//			obj.transform.position = new Vector3(0.5f, 0.5f, 0);
		} else {
			RestartLevel();
		}
	}

	void Awake() {
		gameState = GameState.Init;
		GameFrozen = false;
	}

	void Update() {

		switch (gameState) {
		case GameState.Init:
		{
			ResetGame();
			gameState = GameState.TitleScreen;
			break;
		}
		case GameState.TitleScreen:
		{
			// TODO: some pretty image of nonsense, some title music (started in Init?)
			//		 Flip after some time period to hiscores screen / state
			//       wait for key / button, turn off music, switch state:
			gameState = GameState.InGame;
			break;
		}
		case GameState.InGame:
		{
			break;
		}
		case GameState.GameOver:{
			// TODO: show game over label. wait for button (probably after some arbitrary delay)
			GameFrozen = true;
			gameOver = true;
			GameObject obj = GameObject.Find("GameOverText");
			if (obj == null) {
				obj = new GameObject();
				GUIText text = obj.AddComponent<GUIText>();
				text.text = "GAME OVER";
				obj.transform.position = new Vector3(0.5f, 0.5f, 0);
			}

			if (Input.anyKeyDown) {
				Destroy(obj);
				gameState = GameState.Init;
			}
			break;
		}
		case GameState.Credits: {
			// TODO: show scrolly credits until button pressed, then transition to TitleScreen (or Init)
			break;
		}
		}

//		if (gameOver) {
//			if (Input.anyKeyDown) {
//				RestartGame();
//			}
//			return;
//		}
//
//		// TODO: this, but not every bloody frame!!
//		for (int i = 3; i > 0; i--) {
//			string name = "Bloke_LifeImage" + i.ToString();
//			GameObject obj = GameObject.Find(name);
//			if (i <= lives) {
//				obj.transform.position = new Vector3(obj.transform.position.x, 4.8f, 0);
//			} else 
//			{
//				obj.transform.position = new Vector3(obj.transform.position.x, -9000f, 0);
//			}
//		}
	}

	void RestartGame() {
		GameObject musicObj = GameObject.Find("InGameMusic");
		musicSource = musicObj.GetComponent<AudioSource>();
		musicSource.Play ();

		GameObject obj = GameObject.Find("GameOverText");
		obj.transform.position = new Vector3(0, 0, 0);
		ResetGame ();
		Application.LoadLevel(currentLevel);
	}

	void ResetGame ()
	{
		lives = maxLives;
		GameFrozen = false;
		gameOver = false;
		currentLevel = 0;
	}
}
