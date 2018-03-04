using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

	public delegate void PlayerDelegate();
	public static event PlayerDelegate OnPlayerDied;
	public static event PlayerDelegate OnPlayerScored;

	public float tapForce = 200;
	public float tileSmooth = 2;
	public Vector3 startPos;

	public AudioSource tapAudio;
	public AudioSource scoreAudio;
	public AudioSource dieAudio;

	private GameManager gameManager;

	private Rigidbody2D rb;
	private Quaternion downRotation;
	private Quaternion forwardRotation;

	private void Start() {
		transform.position = new Vector3(transform.position.x * Camera.main.aspect, transform.position.y, transform.position.z);

		rb = GetComponent<Rigidbody2D>();
		downRotation = Quaternion.Euler(0, 0, -90);
		forwardRotation = Quaternion.Euler(0, 0, 35);
		rb.simulated = true;

		gameManager = GameManager.instance;
	}

	private void Update() {
		if (gameManager.GameOver) {
			return;
		}

		if (Input.GetMouseButtonDown(0)) {
			tapAudio.Play();
			transform.rotation = forwardRotation;
			rb.velocity = Vector3.zero;
			rb.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tileSmooth * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "ScoreZone") {
			// Register a score event
			OnPlayerScored(); // Event sent to GameManager
			// Play a sound
			scoreAudio.Play();
		}

		if (col.gameObject.tag == "DeadZone") {
			rb.simulated = false;
			// Register a dead event
			OnPlayerDied(); // Event sent to Gamemanager
			// Play a sound
			dieAudio.Play();
		}
	}

	private void OnEnable() {
		GameManager.OnGameStarted += OnGameStarted;
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}

	private void Disable() {
		GameManager.OnGameStarted -= OnGameStarted;
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	
	}

	private void OnGameStarted() {
		rb.velocity = Vector3.zero;
		rb.simulated = true;
	}

	private void OnGameOverConfirmed() {
		transform.localPosition = startPos;
		transform.rotation = Quaternion.identity;
	}
}
