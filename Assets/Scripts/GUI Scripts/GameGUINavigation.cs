using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameGUINavigation : MonoBehaviour {
	public static GameGUINavigation instance = null;

	[SerializeField]
	private Text txtBigCDT = null;
	[SerializeField]
	private GameObject txtGameOver = null;

	[SerializeField]
	private Text ghostTimer = null;
	[SerializeField]
	private Text gameTimer = null;

	[SerializeField]
	private Text txt_score = null;
	
	[SerializeField]
	private GameObject lifePfb = null;
	[SerializeField]
	private Transform lives = null;

	private List<GameObject> lifes = new List<GameObject>();

	private void Awake()
    {
		if (instance == null) instance = this;
    }

	private void Start()
	{
		lifes.Clear();
		foreach (Transform child in lives)
		{
			Destroy(child.gameObject);
		}
		for (int i = 0; i < GameManager.instance.lives; i++)
		{
			GameObject obj = Instantiate(lifePfb, lives);
			lifes.Add(obj);
		}
		StartCoroutine(ShowBigCDT());
	}
	private void Update()
	{
		if (GameManager.instance.gameState == GameManager.GameState.Game)
        {
			SetScore(GameManager.instance.score);
			SetGhostTimer(GameManager.instance.timeScare);
			SetGameTimer(GameManager.instance.timeGame);
		}
	}

	public void ReduLife()
    {
		Destroy(lifes[lifes.Count - 1]);
		lifes.RemoveAt(lifes.Count - 1);
    }

	private IEnumerator ShowBigCDT()
    {
		txtBigCDT.gameObject.SetActive(true);
		txtBigCDT.text = "3";		
		yield return new WaitForSeconds(1f);
		txtBigCDT.text = "2";
		yield return new WaitForSeconds(1f);
		txtBigCDT.text = "1";
		yield return new WaitForSeconds(1f);
		txtBigCDT.text = "GO!";
		yield return new WaitForSeconds(1f);
		txtBigCDT.gameObject.SetActive(false);
	}

	public void GameOver()
    {
		txtGameOver.SetActive(true);
    }

	public void Exit()
	{
		txtBigCDT.gameObject.SetActive(false);
		GameManager.instance.EndGame(false, false);
	}

	private string FormatTime(float time)
	{
		int minutes = (int)time / 60;
		int seconds = (int)time - 60 * minutes;
		int milliseconds = (int)(100 * (time - minutes * 60 - seconds));
		return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
	}

	private void SetScore(int score)
    {
		txt_score.text = "Score\n" + score;
	}

	private void SetGhostTimer(float time)
    {
		if (time <= 0 && ghostTimer.gameObject.activeSelf) ghostTimer.gameObject.SetActive(false);
		if (time > 0 && !ghostTimer.gameObject.activeSelf) ghostTimer.gameObject.SetActive(true);
		ghostTimer.text = "Ghost Time: " + FormatTime(time);
	}

	private void SetGameTimer(float time)
	{
		gameTimer.text = "Game Time: " + FormatTime(time);
	}
}
