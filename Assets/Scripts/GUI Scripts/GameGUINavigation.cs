using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameGUINavigation : MonoBehaviour {
	public static GameGUINavigation instance = null;

	public float initialDelay;

	[SerializeField]
	private GameObject txtReady = null;
	[SerializeField]
	private GameObject txtGameOver = null;

	[SerializeField]
	private Text ghostTimer = null;
	[SerializeField]
	private Text gameTimer = null;

	private void Awake()
    {
		if (instance == null) instance = this;
    }

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
        Exit();
    }

	public void Exit()
	{
		SceneManager.LoadScene("StartScene");
	}

	private string FormatTime(float time)
	{
		int minutes = (int)time / 60;
		int seconds = (int)time - 60 * minutes;
		int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
		return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
	}

	public void SetGhostTimer(float time)
    {
		if (time <= 0 && ghostTimer.gameObject.activeSelf) ghostTimer.gameObject.SetActive(false);
		if (time > 0 && !ghostTimer.gameObject.activeSelf) ghostTimer.gameObject.SetActive(true);
		ghostTimer.text = "Ghost Time: " + FormatTime(time);
	}

	public void SetGameTimer(float time)
	{
		gameTimer.text = "Game Time: " + FormatTime(time);
	}
}
