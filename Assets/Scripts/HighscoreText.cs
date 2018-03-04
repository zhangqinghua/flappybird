using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class HighscoreText : MonoBehaviour
{
    private Text highscoreText;

	private void OnEnable() {
		highscoreText = GetComponent<Text>();
		highscoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();

	}
}
