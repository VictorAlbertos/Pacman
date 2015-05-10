using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RankingScreen : MonoBehaviour {

	private RectTransform panel; 

	void Start () {
		panel = GetComponentInChildren<RectTransform>(); 
		for (int i = 1; i <= 25; i++)
		{
			GameObject prefab = (GameObject)Instantiate(Resources.Load("text"));
			prefab.transform.SetParent(panel.transform, false);
			Text label = prefab.GetComponent<Text>();
			label.text = "Hola";
		}
	}
	
	public void loadSplashScreen() {
		Application.LoadLevel(0);
	}
}
