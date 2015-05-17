using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using Parse;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using UnityEngine.Advertisements;

public class GameOver : MonoBehaviour {
	private Text score;
	private Text enterName;
	private InputField inputField;
	private Button button;
	private GameObject loading;
	private const String iOS_KEY = "39360";
	private const String ANDROID_KEY = "39334";

	
	void Start () {
		loading = GameObject.Find("loading");

		score = GameObject.Find("score").GetComponent<Text>();
		score.text = "score: " + UIHandler.score.ToString();

		enterName = GameObject.Find("enterName").GetComponent<Text>();
		enterName.gameObject.SetActive(false);

		inputField = GameObject.Find("inputField").GetComponent<InputField>();
		inputField.gameObject.SetActive(false);

		button = GameObject.Find("buttonDone").GetComponent<Button>();
		button.gameObject.SetActive(false);

		if (!Advertisement.isSupported) {
			start();
		} else {
			if (Application.platform == RuntimePlatform.Android)
				Advertisement.Initialize(ANDROID_KEY);
			else if(Application.platform == RuntimePlatform.IPhonePlayer)
				Advertisement.Initialize(iOS_KEY);

			StartCoroutine(tryAd());
			StartCoroutine(startAnyway());
		}

	}

	public IEnumerator tryAd() {
		yield return new WaitForSeconds(1f);
		if (Advertisement.isReady()) {
			Advertisement.Show(null, new ShowOptions {
				pause = true,
				resultCallback = result => {
					start();
				}
			});
		} else StartCoroutine(tryAd());
	}

	private bool enable = true;
	public IEnumerator startAnyway() {
		yield return new WaitForSeconds(10f);

		if (enable) {
			UIHandler.score = 0;
			Application.LoadLevel(0);
		}	
	}

	private void start() {
		var query = ParseObject.GetQuery("GameScore").WhereEqualTo("deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);
		query.FindAsync().ContinueWith(t => {
			results = t.Result;
			enable = results == null;
		});
		
		StartCoroutine(callback());
	}

	void Update() {
		loading.transform.Rotate(Vector3.forward * -5);
	}

	IEnumerable<ParseObject> results;
	private IEnumerator callback() {
		if(results == null) {
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(callback());
		} else {
			loading.SetActive(false);
			if (results.Count() > 0) {
				ParseObject gameScore = (ParseObject) results.ElementAt(0);
				int bestScore = int.Parse(string.Format("{0}", gameScore["score"]));
				if (UIHandler.score > bestScore) {
					gameScore["score"] = UIHandler.score;
					gameScore.SaveAsync();
				}
				UIHandler.score = 0;
				Application.LoadLevel(0);
			} else  {
				showInputUserName();
			}
		}
	}

	private void showInputUserName(){
		inputField.gameObject.SetActive(true);
		enterName.gameObject.SetActive(true);
		button.gameObject.SetActive(true);
	}

	public void onClickDone() {
		string userName = inputField.text;
		if (String.IsNullOrEmpty(userName)) return;

		ParseObject gameScore = ParseObject.Create("GameScore");
		gameScore["score"] = UIHandler.score;
		gameScore["playerName"] = userName;
		gameScore["deviceUniqueIdentifier"] = SystemInfo.deviceUniqueIdentifier;
		gameScore.SaveAsync();
		Application.LoadLevel(0);
		UIHandler.score = 0;
	}

}
