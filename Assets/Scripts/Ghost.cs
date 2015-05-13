using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;    

public class Ghost : MonoBehaviour {
	public static float factor = 1;
	public Transform[] waypoints;
	int cur = 0;
	public bool canStart;
	private float speed = 0.15f * factor;
	private float speedMovement = 7 * factor;	
	public bool hasBeenKilled;
	private ComeCocos comeCocos;
	private Vector2 initialPosition;

	void Start() {
		if(speedMovement > 12) speedMovement = 12;
		GetComponent<Animator>().speed = 0;
		initialPosition = transform.position;
		comeCocos = GameObject.Find("come_cocos").GetComponent<ComeCocos>();
		StartCoroutine(startWithDelay());
	}
	
	public int delayOnStartInSeconds = 2;
	IEnumerator startWithDelay() {
		yield return new WaitForSeconds(delayOnStartInSeconds);
		canStart = true;
		GetComponent<Animator>().speed = 1;
	}
	
	void FixedUpdate () {
		if (!canStart || !UIHandler.canStart) return;
		
		if (cur == waypoints.Length-1) randomPath();
		else followWaypoints();
		
		handleAnimation();
		
		if (!hasBeenKilled) {
			GetComponent<Animator>().SetBool("comeCocosCanKill", comecocosCanKill());
		} else  {
			GetComponent<Animator>().SetBool("comeCocosCanKill", false);
		}
	}
	
	private void handleAnimation() {
		float dirX = 0.0f;
		float dirY = 0.0f;
		
		if (cur == waypoints.Length-1) {
			if (currentDirection == Vector2.up) dirY = 0.2f;
			else if (currentDirection == -Vector2.up) dirY = -0.2f;
			else if (currentDirection == Vector2.right) dirX = 0.2f;
			else if (currentDirection == -Vector2.right) dirX = -0.2f;
		} else {
			Vector2 dir = waypoints[cur].position - transform.position;
			dirX = dir.x;
			dirY = dir.y;
		}
		
		GetComponent<Animator>().SetFloat("DirX", dirX);
		GetComponent<Animator>().SetFloat("DirY", dirY);
	}
	
	
	Vector2 currentDirection = -Vector2.right;
	Vector2 nextDirection = -Vector2.right;
	private void randomPath() {
		if (validChangeDir()) {
			currentDirection = nextDirection;
		}

		if(valid(currentDirection)) {
			transform.Translate(currentDirection * speedMovement * Time.deltaTime);
		}
		else changeToValidDirection(true);
	}
	
	private void changeToValidDirection(bool calledFromCurrentDirection) {
		List<Vector2> allPosibleDirections = new List<Vector2> {Vector2.up, Vector2.right, -Vector2.right, -Vector2.up};
		allPosibleDirections.RemoveAll(val => val == -currentDirection);

		System.Random rng = new System.Random();		
		int n = allPosibleDirections.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			Vector2 value = allPosibleDirections[k];  
			allPosibleDirections[k] = allPosibleDirections[n];  
			allPosibleDirections[n] = value;  
		}

		//sortListByNearComeCocos(allPosibleDirections);
		foreach (Vector2 candidate in allPosibleDirections) {
			if (valid(candidate)) {
				if (calledFromCurrentDirection) currentDirection = candidate;
				else nextDirection = candidate;
				break;
			}
		}
	}

	private void sortListByNearComeCocos(List<Vector2> allPosibleDirections) {
		Vector2 position = transform.position - comeCocos.transform.position;
		Vector2 newDirection;
		if(Math.Abs(position.x) >= Math.Abs(position.y)) {
			if(position.x <= 0) {
				//print("right");
				newDirection = Vector2.right;
			} else {
				//print("left");
				newDirection = -Vector2.right;
				}
		} else {
			if(position.y <= 0) {
				//print("down");
				newDirection = Vector2.up;
			} else {
				//print("up");
				newDirection = -Vector2.up;
			}
		}

		allPosibleDirections.RemoveAll(val => val == -newDirection);
		allPosibleDirections.Insert(0, newDirection);
	}

			
	bool valid(Vector2 dir) {
		Vector2 pos = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
		bool valid = (hit.collider == GetComponent<Collider2D>());

		if (hit.collider.gameObject.tag == "coco" || hit.collider.gameObject.tag == "come_cocos" || hit.collider.gameObject.tag == "special_coco" || hit.collider.gameObject.tag == "ghost"
		    || hit.collider.gameObject.name == "limitLeft" || hit.collider.gameObject.name == "limitRight" || hit.collider.gameObject.tag == "box_choice") return true;
		return valid;
	}
	
	private void followWaypoints() {
		Vector2 waypoint = new Vector2(waypoints[cur].position.x, waypoints[cur].position.y);
		if (checked ((Vector2)transform.position)  != checked ((Vector2)waypoint)) {
			Vector2 p = Vector2.MoveTowards(transform.position,
			                                waypoints[cur].position,
			                                speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		} else {
			cur = (cur + 1) % waypoints.Length;
		}
	}
	
	public void killMe() {
		GetComponent<Animator>().SetBool("comeCocosCanKill", false);
		hasBeenKilled = true;
		cur = 0;
		transform.position = initialPosition;
		canStart = false;
		StartCoroutine(delayTurnOffAnim());
		StartCoroutine(startWithDelay());
	}
	
	IEnumerator delayTurnOffAnim() {
		yield return new WaitForSeconds(0.15f);
		GetComponent<Animator>().speed = 0;
	}
	
	public void comeCocosCanKillNotification() {
		mFlashes = false;
		StopCoroutine(warningTimeKilling());
		StartCoroutine(warningTimeKilling());
	}
	
	IEnumerator warningTimeKilling() {
		yield return new WaitForSeconds(8);
		StartCoroutine(flashes());
	}
	
	private bool mFlashes;
	IEnumerator flashes() {
		mFlashes = true;
		yield return new WaitForSeconds(2);
		mFlashes = false;
	}
	
	private bool flipColor;
	private bool comecocosCanKill() {
		if (mFlashes) {
			flipColor = !flipColor;
			if (flipColor) return false;
			else return true;
		} else {
			return comeCocos.canKill;
		}
	}

	private bool firstTimeCollider = false;
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag != "box_choice") return;
		firstTimeCollider = true;
	}

	void OnTriggerStay2D(Collider2D collider) {
		if (collider.name == "limitRight") {
			doOnLimitRightCollide();
			return;
		}

		if (collider.name == "limitLeft") {
			doOnLimitLeftCollide();
			return;
		}

		if (collider.tag != "box_choice" || !firstTimeCollider) return;


		Renderer renderer = GetComponent<Renderer>();

		if (currentDirection == Vector2.up || currentDirection == -Vector2.up) {
			if(Math.Round(collider.bounds.center.y, 1) == Math.Round(renderer.bounds.center.y, 1))
			{
				changeToValidDirection(false);
				firstTimeCollider = false;
			}
		} else {
			if(Math.Round(collider.bounds.center.x, 1) == Math.Round(renderer.bounds.center.x, 1))
			{
				changeToValidDirection(false);
				firstTimeCollider = false;
			}
		}

	}

	private void doOnLimitRightCollide() {
		GameObject limitLeft = GameObject.Find("limitLeft");
		float newX = limitLeft.transform.position.x + 2;
		transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
	}

	private void doOnLimitLeftCollide() {
		GameObject limitRight = GameObject.Find("limitRight");
		float newX = limitRight.transform.position.x - 2;
		transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
	}

	bool validChangeDir() {
		Vector2 nextDir = nextDirection;
		Vector2 nextDirSafe = nextDir;
		Vector2 pos = transform.position;
		
		//Left
		if (nextDir == -Vector2.right && currentDirection == Vector2.up) nextDirSafe = new Vector2(-1,-1);
		if (nextDir == -Vector2.right && currentDirection == -Vector2.up) nextDirSafe = new Vector2(-1, 1);
		
		//Right
		if (nextDir == Vector2.right && currentDirection == Vector2.up) nextDirSafe = new Vector2(1,-1);
		if (nextDir == Vector2.right && currentDirection == -Vector2.up) nextDirSafe = new Vector2(1, 1);
		
		//Up
		if (nextDir == Vector2.up && currentDirection == Vector2.right) nextDirSafe = new Vector2(-1,1);
		if (nextDir == Vector2.up && currentDirection == -Vector2.right) nextDirSafe = new Vector2(1, 1);
		
		//Down
		if (nextDir == -Vector2.up && currentDirection == Vector2.right) nextDirSafe = new Vector2(-1,-1);
		if (nextDir == -Vector2.up && currentDirection == -Vector2.right) nextDirSafe = new Vector2(1, -1);
		
		//Debug.DrawRay(pos, nextDir, Color.green);
		//Debug.DrawRay(pos, nextDirSafe, Color.cyan);

		RaycastHit2D hit = Physics2D.Linecast(pos + nextDir, pos);
		RaycastHit2D hitSafe = Physics2D.Linecast(pos + nextDirSafe, pos);

		if (hit.collider.gameObject.tag == "coco" || hit.collider.gameObject.tag == "come_cocos" || hit.collider.gameObject.tag == "special_coco" 
		    || hit.collider.gameObject.name == "limitLeft" || hit.collider.gameObject.name == "limitRight") {
			return true;
		}
		
		bool valid = (hit.collider == GetComponent<Collider2D>()) && (hitSafe.collider == GetComponent<Collider2D>());
		return valid;
	}
}
