using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameGUINavigation : MonoBehaviour {
	public float initialDelay;

	[SerializeField]
	private GameObject popupHS = null;
	[SerializeField]
	private GameObject txtReady = null;
	[SerializeField]
	private GameObject txtGameOver = null;

	void Start () 
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}

	public void H_ShowReadyScreen()
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}

    public void H_ShowGameOverScreen()
    {
        StartCoroutine("ShowGameOverScreen");
    }

	IEnumerator ShowReadyScreen(float seconds)
	{
		GameManager.gameState = GameManager.GameState.Init;
		txtReady.gameObject.SetActive(true);
		yield return new WaitForSeconds(seconds);
		txtReady.gameObject.SetActive(false);
		GameManager.gameState = GameManager.GameState.Game;
	}

    IEnumerator ShowGameOverScreen()
    {
        Debug.Log("Showing GAME OVER Screen");
		txtGameOver.SetActive(true);
        yield return new WaitForSeconds(2);
        Menu();
    }

	public void getScoresMenu()
	{
		Time.timeScale = 0f;		// stop the animations
		GameManager.gameState = GameManager.GameState.Scores;
		popupHS.gameObject.SetActive(true);
	}

	//------------------------------------------------------------------
	// Button functions
	public void SetTimeScale(float time)
    {
		Time.timeScale = time;
    }

	public void Menu()
	{
		SceneManager.LoadScene("StartScene");
		Time.timeScale = 1.0f;

        // take care of game manager
	    GameManager.DestroySelf();
	}

	public void SubmitScores()
	{
		// Check username, post to database if its good to go
	    int highscore = GameManager.score;
        string username = popupHS.GetComponentInChildren<InputField>().GetComponentsInChildren<Text>()[1].text;

		Debug.Log("HighScore: " + username + " : " + highscore);
	}
}
