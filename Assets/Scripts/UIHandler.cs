using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
	public static int score = 0;
	private Text ready;
	public static bool canStart;

	void Start () {

		if (ComeCocos.lives == 2) {
			Destroy(GameObject.Find("live2"));
		} else if (ComeCocos.lives == 1) {
			Destroy(GameObject.Find("live2"));
			Destroy(GameObject.Find("live1"));
		} else if (ComeCocos.lives == 0) {
			Debug.Log("Muere");
		}
		ready = GameObject.Find("ready").GetComponent<Text>();
		StartCoroutine(show2());
	}
	
	void Update () {
		Text scoreLabel = GameObject.Find("score").GetComponent<Text>();
		scoreLabel.text = UIHandler.score.ToString();
	}

	IEnumerator show2() {
		yield return new WaitForSeconds(1);
		ready.text = "2";
		StartCoroutine(show1());
	}

	IEnumerator show1() {
		yield return new WaitForSeconds(1);
		ready.text = "1";
		StartCoroutine(showGo());
	}

	IEnumerator showGo() {
		yield return new WaitForSeconds(1);
		ready.text = "Go!";
		StartCoroutine(dismiss());
	}

	IEnumerator dismiss() {
		yield return new WaitForSeconds(1);
		ready.text = "";
		canStart = true;
	}


}
