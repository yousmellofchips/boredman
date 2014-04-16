using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static bool gameFrozen = false;
	private static int lives = 3;
	public static int currentLevel = 0;

	private const int maxLives = 3;

	private static bool gameOver = false;

	public static void RestartLevel() {
		//GameObject musicObj = GameObject.Find("InGameMusic");
		//AudioSource musicSource = musicObj.GetComponent<AudioSource>();

		Application.LoadLevel(currentLevel);
	}

	public static void LoseLife() {
		lives--;
		if (lives <= -1) {
			gameFrozen = true;
			gameOver = true;
			GameObject obj = GameObject.Find("GameOverText");
			obj.transform.position = new Vector3(0.5f, 0.5f, 0);
		} else {
			RestartLevel();
		}
	}

	void Update() {
		if (gameOver) {
			if (Input.anyKeyDown) {
				RestartGame();
			}
			return;
		}

		// TODO: this, but not every bloody frame!!
		for (int i = 3; i > 0; i--) {
			string name = "Bloke_LifeImage" + i.ToString();
			GameObject obj = GameObject.Find(name);
			if (i <= lives) {
				obj.transform.position = new Vector3(obj.transform.position.x, 4.8f, 0);
			} else 
			{
				obj.transform.position = new Vector3(obj.transform.position.x, -9000f, 0);
			}
		}
	}

	void RestartGame() {
		GameObject obj = GameObject.Find("GameOverText");
		obj.transform.position = new Vector3(0, 0, 0);
		lives = maxLives;
		gameFrozen = false;
		gameOver = false;
		currentLevel = 0;
		Application.LoadLevel(currentLevel);
	}
}
