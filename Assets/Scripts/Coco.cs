using UnityEngine;
using System.Collections;

public class Coco : MonoBehaviour {
	private const int totalCocos = 246;
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
			}

			Destroy(gameObject);
			UIHandler.score += 10;
			nCocosEaten++;
		}
	}

	public static bool isAllCocosEaten() {
		return nCocosEaten == totalCocos;
	}
}
