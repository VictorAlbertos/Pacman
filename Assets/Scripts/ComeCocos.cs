using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ComeCocos : MonoBehaviour {
	private static int SOUND_SIREN = 4, SOUND_INTERMISSION = 3, SOUND_DEATH = 2, SOUND_EAT_COCO = 1, SOUND_EAT_GOST = 0;
	public static int lives = 3;
	public bool canKill;
	private static float factor = 1;
	private float speed = 7 * factor;	
	Vector2 direction;
	Vector2 nextDirection;
	AnimDir animDir;
	bool hasBeenChanged;
	bool idle;

	void Start() {
		if(speed > 12) speed = 12;
		nextDirection = Vector2.right;
		direction = Vector2.right;
		animDir = AnimDir.Right;
	}

	private bool firstTimeUpdate = true;
	void Update() {
		if (!UIHandler.canStart) {
			GetComponent<Animator>().speed = 0;
			return;
		}

		if (firstTimeUpdate) {
			firstTimeUpdate = false;
			soundSiren();
		}

		if (validChangeDir() && hasBeenChanged) {
			hasBeenChanged = false;
			direction = nextDirection;
			GetComponent<Animator>().SetInteger("animDir", (int) animDir);
		}

		if(valid(direction)) {
			transform.Translate(direction * speed * Time.deltaTime);
			idle = false;
		} else idle = true;

		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); 

		if (Input.GetKey(KeyCode.UpArrow)) up();
			
		if (Input.GetKey(KeyCode.DownArrow)) down();
			
		if (Input.GetKey(KeyCode.RightArrow)) right();
			
		if (Input.GetKey(KeyCode.LeftArrow)) left();

		checkIfPassToNextLevel();
		GetComponent<Animator>().speed = 1;
	}

	public void up() {
		animDir = AnimDir.Up;
		nextDirection = Vector2.up;
		hasBeenChanged = true;
	}

	public void down() {
		animDir = AnimDir.Down;
		nextDirection = -Vector2.up;
		hasBeenChanged = true;
	}

	public void right() {
		animDir = AnimDir.Right;
		nextDirection = Vector2.right;
		hasBeenChanged = true;
	}

	public void left() {
		animDir = AnimDir.Left;
		nextDirection = -Vector2.right;
		hasBeenChanged = true;
	}
	
	
	bool valid(Vector2 dir) {
		Vector2 pos = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
		bool valid = (hit.collider == GetComponent<Collider2D>());

		if (hit.collider.gameObject.tag == "ghost" || hit.collider.gameObject.tag == "coco" || hit.collider.gameObject.tag == "box_choice" || hit.collider.gameObject.tag == "special_coco" 
		    || hit.collider.gameObject.name == "limitLeft" || hit.collider.gameObject.name == "limitRight") return true;
		return valid;
	}

	bool validChangeDir() {
		Vector2 nextDir = nextDirection;
		Vector2 nextDirSafe = nextDir;
		Vector2 pos = transform.position;

		//Left
		if (nextDir == -Vector2.right && direction == Vector2.up && !idle) nextDirSafe = new Vector2(-1,-1);
		if (nextDir == -Vector2.right && direction == -Vector2.up && !idle) nextDirSafe = new Vector2(-1, 1);

		//Right
		if (nextDir == Vector2.right && direction == Vector2.up && !idle) nextDirSafe = new Vector2(1,-1);
		if (nextDir == Vector2.right && direction == -Vector2.up && !idle) nextDirSafe = new Vector2(1, 1);
	
		//Up
		if (nextDir == Vector2.up && direction == Vector2.right && !idle) nextDirSafe = new Vector2(-1,1);
		if (nextDir == Vector2.up && direction == -Vector2.right && !idle) nextDirSafe = new Vector2(1, 1);

		//Down
		if (nextDir == -Vector2.up && direction == Vector2.right && !idle) nextDirSafe = new Vector2(-1,-1);
		if (nextDir == -Vector2.up && direction == -Vector2.right && !idle)nextDirSafe = new Vector2(1, -1);

		//Debug.DrawRay (pos, nextDir, Color.green);
		//Debug.DrawRay (pos, nextDirSafe, Color.blue);
		RaycastHit2D hit = Physics2D.Linecast(pos + nextDir, pos);
		RaycastHit2D hitSafe = Physics2D.Linecast(pos + nextDirSafe, pos);

		if (hit.collider.gameObject.tag == "ghost" || hit.collider.gameObject.tag == "coco" || hit.collider.gameObject.tag == "box_choice" || hit.collider.gameObject.tag == "special_coco" 
		    || hit.collider.gameObject.name == "limitLeft" || hit.collider.gameObject.name == "limitRight") return true;

		bool valid = (hit.collider == GetComponent<Collider2D>()) && (hitSafe.collider == GetComponent<Collider2D>());
		return valid;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.name == "limitLeft") doOnLimitLeftCollide();
		if (collider.name == "limitRight") doOnLimitRightCollide();
		if (collider.tag == "ghost") doOnGhostCollide(collider);
	}

	public void soundCoco() {
		AudioSource[] audios = GetComponents<AudioSource>();
		AudioSource audio = audios[SOUND_EAT_COCO];
		if(!audio.isPlaying) audio.Play();
	}

	public void soundSiren() {
		AudioSource[] audios = GetComponents<AudioSource>();
		AudioSource audio = audios[SOUND_SIREN];
		if(!audio.isPlaying) audio.Play();
	}

	private void doOnLimitLeftCollide() {
		GameObject limitRight = GameObject.Find("limitRight");
		float newX = limitRight.transform.position.x - 2;
		transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
	}

	private void doOnLimitRightCollide() {
		GameObject limitLeft = GameObject.Find("limitLeft");
		float newX = limitLeft.transform.position.x + 2;
		transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
	}

	private void doOnGhostCollide(Collider2D collider) {
		Ghost ghost = collider.gameObject.GetComponent<Ghost>();
		if (canKill && !ghost.hasBeenKilled) {
			killGhost(collider);
			return;
		}

		lives--;

		GetComponents<AudioSource>()[SOUND_SIREN].Stop();
		GetComponents<AudioSource>()[SOUND_DEATH].Play();
		StartCoroutine(restarWithDealy());
	}

	IEnumerator restarWithDealy() {
		UIHandler.canStart = false;
		yield return new WaitForSeconds(2.5f);

		Coco.nCocosEaten = 0;
		if (lives == 0) Application.LoadLevel(2);
		else Application.LoadLevel(1);
	}

	private int nDeadGhost;
	private void killGhost(Collider2D collider) {
		AudioSource[] audios = GetComponents<AudioSource>();
		AudioSource audio = audios[SOUND_EAT_GOST];
		audio.Play();

		nDeadGhost++;
		int plusScore = 200 * nDeadGhost;
		UIHandler.score += plusScore;
		Ghost ghost = collider.gameObject.GetComponent<Ghost>();

		ScoreGhostKilled scoreGhostKilled = GameObject.Find("scoreGhostKilled").GetComponent<ScoreGhostKilled>();
		scoreGhostKilled.showScoreOnGhostDead(plusScore, ghost);

		ghost.killMe();
	}

	private void showScoreOnGhostDead(int score, Ghost ghost) {
		GameObject scoreGhostKilled = GameObject.Find("scoreGhostKilled");
		TextMesh text = scoreGhostKilled.GetComponent<TextMesh>();
		text.transform.position = ghost.transform.position;
		text.text = "+" + score.ToString();
	}

	private const int timeCanKillInSeconds = 10;
	public void turnAsAKillingMachine() {
		nDeadGhost = 0;
		GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost"); 
		foreach (GameObject ghostObject in ghosts)
		{
			Ghost ghost = ghostObject.GetComponent<Ghost>();
			ghost.hasBeenKilled = false;
		}
		
		CancelInvoke();
		canKill = true;
		InvokeRepeating("turnOffAsAKillingMachine", timeCanKillInSeconds, timeCanKillInSeconds);
	}
	
	private void turnOffAsAKillingMachine() {
		canKill = false;
		CancelInvoke();
	}

	private bool firstTime = true;
	private void checkIfPassToNextLevel() {
		if (!Coco.isAllCocosEaten()) return;
		if (!firstTime) return;

		GetComponents<AudioSource>()[SOUND_INTERMISSION].Play();
		StartCoroutine(newLevelWithDelay());
	}

	IEnumerator newLevelWithDelay() {
		UIHandler.canStart = false;

		yield return new WaitForSeconds(5f);
		ComeCocos.factor = ComeCocos.factor * 1.025f;
		Ghost.factor = Ghost.factor * 1.025f;

		firstTime = false;
		Coco.nCocosEaten = 0;
		Application.LoadLevel(1);
	}

}

enum AnimDir{Right = 1, Left = 2, Up = 3, Down = 4};
