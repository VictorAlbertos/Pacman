using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Parse;
using System.Linq;

public class RankingScreen : MonoBehaviour {
	private RectTransform panel; 
	private const int limit = 100;
	private GameObject loading;
	private Text yourScore;

	void Start () {
		loading = GameObject.Find("loading");
		yourScore = GameObject.Find("yourScore").GetComponent<Text>();

		panel = GetComponentInChildren<RectTransform>(); 

		var query = ParseObject.GetQuery("GameScore").OrderByDescending("score").Limit(limit);
		query.FindAsync().ContinueWith(t => {
			results = t.Result;
		});

		StartCoroutine(callback());
	}

	void Update() {
		loading.transform.Rotate(Vector3.forward * -5);
	}

	private ParseObject currentPlayer;
	private int positionCurrentPlayer;
	IEnumerable<ParseObject> results;
	private IEnumerator callback() {
		if(results == null) {
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(callback());
		} else {
			loading.SetActive(false);
			int position = 1;
			foreach (var gameScore in results)
			{
				bool isPlayer = false;
				string idUser = string.Format("{0}", gameScore["deviceUniqueIdentifier"]);
				if (idUser.Equals(SystemInfo.deviceUniqueIdentifier)) {
					currentPlayer = gameScore;
					positionCurrentPlayer = position;
					StartCoroutine(printInfoCurrentUser());
					isPlayer = true;
				}

				GameObject prefab = (GameObject)Instantiate(Resources.Load("text"));
				prefab.transform.SetParent(panel.transform, false);
				Text label = prefab.GetComponent<Text>();

				if (isPlayer) {
					label.text = "#" + position + " YOU: " + string.Format("{0}", gameScore["score"]);  
				} else {
					label.text = "#" + position + " " + gameScore["playerName"] + ": " + string.Format("{0}", gameScore["score"]);  
				}
				position++;
			}

			if(currentPlayer == null)
				StartCoroutine(printInfoCurrentUser());
		}
	}

	private bool firstTime = true;
	public IEnumerator printInfoCurrentUser() {
		if(currentPlayer == null) {
			if(firstTime) {
				firstTime = false;
				var query = ParseObject.GetQuery("GameScore").WhereEqualTo("deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);
				query.FindAsync().ContinueWith(t => {
					currentPlayer = (ParseObject) t.Result.ElementAt(0);
				});
			}
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(printInfoCurrentUser());

		} else {
			string name = string.Format("{0}", currentPlayer["playerName"]);
			string score = string.Format("{0}", currentPlayer["score"]);
			if(positionCurrentPlayer == 0) {
				yourScore.text = name + ", your best score is: " + score + "\n Current position: below # " + limit;
			} else {
				yourScore.text = name + ", your best score is: " + score + "\n Current position: # " + positionCurrentPlayer;
			}
		}
	}

	public void loadSplashScreen() {
		Application.LoadLevel(0);
	}
}
