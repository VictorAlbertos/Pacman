using UnityEngine;
using System.Collections;

public class ScoreGhostKilled : MonoBehaviour {
	private TextMesh textMesh;
	private Vector2 originalPosition;
	public float speed;	

	void Start () {
		originalPosition = transform.position;
		textMesh = GetComponent<TextMesh>();
		speed = 2.5f;
	}
	
	void Update () {
		transform.Translate(Vector2.up * speed * Time.deltaTime);
	}

	public void showScoreOnGhostDead(int score, Ghost ghost) {
		Vector2 position = new Vector2(ghost.transform.position.x - 2, ghost.transform.position.y);
		textMesh.transform.position = position;
		textMesh.text = "+" + score.ToString();
		StartCoroutine(dissapearsWithDelay());
	}

	IEnumerator dissapearsWithDelay() {
		yield return new WaitForSeconds(1);
		transform.position = originalPosition;
	}
}
