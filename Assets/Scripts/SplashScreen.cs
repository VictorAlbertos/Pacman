using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	public void loadGamePLayScene() {
		ComeCocos.lives = 3;
		Application.LoadLevel(1);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); 
	}

	public void loadRanking() {
		Application.LoadLevel(3);
	}
}
