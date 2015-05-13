using UnityEngine;
using System.Collections;

public class Coco : MonoBehaviour {
	private const int totalCocos = 242;
	public static int nCocosEaten;
	private ComeCocos comeCocos;

	void Start() {
		comeCocos = GameObject.Find("come_cocos").GetComponent<ComeCocos>();
	}

	void OnTriggerEnter2D(Collider2D co) {
		if (co.name == "come_cocos") {
			comeCocos.soundCoco();

			if (tag == "special_coco") {
				comeCocos.turnAsAKillingMachine();

				GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost"); 
				foreach (GameObject ghostObject in ghosts)
				{
					Ghost ghost = ghostObject.GetComponent<Ghost>();
					ghost.comeCocosCanKillNotification();
				}
				StartCoroutine(recreateSpecialCocoWithDelay());
			} else {
				Destroy(gameObject);
				nCocosEaten++;
			}
		
			UIHandler.score += 10;
		}
	}

	private Vector2 originalPosition;
	IEnumerator recreateSpecialCocoWithDelay() {
		originalPosition = gameObject.transform.position;
		gameObject.transform.position = new Vector2(1000,1000);
		yield return new WaitForSeconds(Random.Range(25, 60));
		gameObject.transform.position = originalPosition;
	}

	public static bool isAllCocosEaten() {
		return nCocosEaten == totalCocos;
	}
}
