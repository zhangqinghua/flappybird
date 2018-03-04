using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GameOverScoreText : MonoBehaviour {

	private Text scoreText;

	private void OnEnable() {
		scoreText = GetComponent<Text>();
		scoreText.text = "Score: " + GameManager.instance.Score;
	}
}
