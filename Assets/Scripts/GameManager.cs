using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private const int kInitialLevel = 1; // first actual playable level

	private int lives = maxLives;
	private int currentLevel = kInitialLevel;
	private AudioSource musicSource;

	private GameState gameState;

	private static GameManager _instance = null;

	public void LoseLife() {
		lives--;
		if (lives <= -1) {
			gameState = GameState.GameOver;
		} else {
			gameState = GameState.InitLevel;
		}
	}

	public void NextLevel() {
		if (++currentLevel >= Application.levelCount) {
			currentLevel = kInitialLevel;

			gameState = GameState.GameCompleted;
		} else {
			gameState = GameState.InitLevel;
		}
	}

	void Awake() {
		gameState = GameState.Init;
	}

	void Update() {

		switch (gameState) {
		case GameState.Init:
		{
			GameFrozen = false;
			lives = maxLives;
			currentLevel = 0; // Title screen level special number zero
			Application.LoadLevel(currentLevel);
			gameState = GameState.TitleScreen;
			break;
		}
		case GameState.TitleScreen:
		{
			// TODO: some pretty image of nonsense, some title music (started in Init?)
			//		 Flip after some time period to hiscores screen / state
			//       wait for key / button, turn off music, switch state:
			GameObject obj = GameObject.Find("TitleText");
			if (obj == null) {
				obj = new GameObject();
				obj.name = "TitleText";
				GUIText text = obj.AddComponent<GUIText>();
				text.text = "BoredMan - The Quest For Smokes";
				text.fontSize = 20;
				text.anchor = TextAnchor.MiddleCenter;
				obj.transform.position = new Vector3(0.5f, 0.5f, 0);
			}
			
			if (Input.anyKeyDown) {
				Destroy(obj);
				currentLevel = kInitialLevel;
				gameState = GameState.InitLevel;
			}
			break;
		}
		case GameState.InitLevel: {
			GameFrozen = false;
			Application.LoadLevel(currentLevel);
			gameState = GameState.InGame;
			break;
		}
		case GameState.InGame:
		{
			UpdateLives();

			break;
		}
		case GameState.GameOver: {
			// Show game over label. Wait for button (TODO: probably after some arbitrary delay?)
			GameFrozen = true;
			GameObject obj = GameObject.Find("GameOverText");
			if (obj == null) {
				obj = new GameObject();
				obj.name = "GameOverText";
				GUIText text = obj.AddComponent<GUIText>();
				text.text = "GAME OVER";
				text.fontSize = 20;
				text.anchor = TextAnchor.MiddleCenter;
				obj.transform.position = new Vector3(0.5f, 0.5f, 0);
			}

			if (Input.anyKeyDown) {
				Destroy(obj);
				gameState = GameState.Init;
			}
			break;
		}
		case GameState.GameCompleted: {
			// TODO: shiny completion sequence, happy days. Then...

			gameState = GameState.Init;
			break;
		}
		case GameState.Credits: {
			// TODO: show scrolly credits until button pressed, then transition to TitleScreen (or Init)
			break;
		}
		}
	}

	void UpdateLives() {
		GameObject[] lifeHeads = GameObject.FindGameObjectsWithTag("LifeHead");
		if (lifeHeads != null) {
			foreach (GameObject lifeObj in lifeHeads) {
				Destroy(lifeObj);
			}
		}

		// Add the label for "lives" first, then add all the heads (one per life)
		float lifeXPos = -7.65f;
		if (GameObject.Find("Lives_Label_Image") == null) {
			GameObject lifeLabel = (GameObject)Instantiate(Resources.Load("Lives_Label_Image"));
			lifeLabel.name = "Lives_Label_Image";
			lifeLabel.transform.position = new Vector3(lifeXPos, 4.75f, 0f);
		}
		lifeXPos += 0.4f;

		for (int life = 0; life < lives; life++) {
			GameObject lifeHead = (GameObject)Instantiate(Resources.Load("Bloke_LifeImage"));
			lifeHead.tag = "LifeHead";
			lifeHead.transform.position = new Vector3(lifeXPos, 4.8f, 0f);

			lifeXPos += 0.3f;
		}
	}
}
