using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour {

    [SerializeField]
    private Text HSandTimer = null;

    private int highScore = 0;
    private float timeHS = 0;
    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        timeHS = PlayerPrefs.GetFloat("TimeHS", 0);

        HSandTimer.text = "HighScore: " + highScore + " Time: " + FormatTime(timeHS);
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
