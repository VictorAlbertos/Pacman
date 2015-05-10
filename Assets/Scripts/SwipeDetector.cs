using UnityEngine;
using System.Collections;

public class SwipeDetector : MonoBehaviour {
	private float minSwipeDistY;
	private float minSwipeDistX;
	private Vector2 startPos;
	private ComeCocos comeCocos;

	void Start() {
		comeCocos = GameObject.Find("come_cocos").GetComponent<ComeCocos>();
		minSwipeDistY = Screen.width * 0.20f;
		minSwipeDistX = Screen.width * 0.20f;
	}

	void Update() {
		//#if UNITY_ANDROID
		if (Input.touchCount > 0)  {
			Touch touch = Input.touches[0];
			switch (touch.phase) {
				case TouchPhase.Began:
					startPos = touch.position;
					break;

				case TouchPhase.Ended:
					float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
					if (swipeDistVertical > minSwipeDistY) {
						float swipeValue = Mathf.Sign(touch.position.y - startPos.y);
						if (swipeValue > 0) comeCocos.up();
						else if (swipeValue < 0) comeCocos.down();
					}
					
					float swipeDistHorizontal = (new Vector3(touch.position.x,0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;
					if (swipeDistHorizontal > minSwipeDistX) {
						float swipeValue = Mathf.Sign(touch.position.x - startPos.x);
						if (swipeValue > 0) comeCocos.right();
						else if (swipeValue < 0) comeCocos.left();
					}
					break;
			}
		}
	}
}