using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int lives = 3;

    [SerializeField]
    public enum GameState { Init, Game, Dead, End }
    public GameState gameState;

    [SerializeField]
    private GameObject pacman = null;
    [SerializeField]
    private GameObject blinky = null;
    [SerializeField]
    private GameObject pinky = null;
    [SerializeField]
    private GameObject inky = null;
    [SerializeField]
    private GameObject clyde = null;

    private Vector3 posPacMan = new Vector3();
    private Vector3 posBlinky = new Vector3();
    private Vector3 posPinky = new Vector3();
    private Vector3 posInky = new Vector3();
    private Vector3 posClyde = new Vector3();

    public bool scared;
    public int score;
    private int highScore;
    private float timeHS;

    public float SpeedPerLevel;

    public static GameManager instance = null;

    public float timeScare = 0f;
    public float timeScareAdd = 10f;

    public float timeGame = 0f;

    private float timeStart = 4f;
    private float timeEnd = 3f;

    [SerializeField]
    private float timeSpawnCherry = 30f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += LoadScene;

            highScore = PlayerPrefs.GetInt("HighScore", 0);
            timeHS = PlayerPrefs.GetFloat("TimeHS", 0);
        }
        else
        {
            if (this != instance)
                Destroy(this.gameObject);
        }

        if (SceneManager.GetActiveScene().name == "StartScene") return;
        AssignGhosts();
    }

    private void Start()
    {
        gameState = GameState.Init;       
    }

    private void Update()
    {
        if (scared && timeScare <= 0)
            CalmGhosts();

        if (timeScare > 0)
        {
            timeScare -= Time.deltaTime;
        }
        else timeScare = 0f;

        if (gameState == GameState.Game)
        {
            timeGame += Time.deltaTime;

            if(timeGame > timeSpawnCherry)
            {
                CherryController.instance.SpawnCherry();
                timeSpawnCherry += timeSpawnCherry;
            }
        }
    }

    void LoadScene(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "StartScene":
                break;
            case "RoundStart":
                StartCoroutine(InGame());
                break;
            case "InnovationScene":
                StartCoroutine(InGame());
                break;
        }
    }

    private IEnumerator InGame()
    {
        gameState = GameState.Init;
        AssignGhosts();
        ResetVariables();
        MusicManager.instance.Ready();

        yield return new WaitForSeconds(timeStart);               
        gameState = GameState.Game;  
    }

    public void EndGame(bool isWin, bool isDelay = true)
    {
        Debug.Log("EndGame: " + isWin);
        MusicManager.instance.timeScared = 0;
        gameState = GameState.End;
        GameGUINavigation.instance.GameOver();
        if (score > highScore || (score == highScore && timeGame < timeHS))
        {
            highScore = score;
            timeHS = timeGame;

            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.SetFloat("TimeHS", timeHS);
        }
        StartCoroutine(iEndGame(isDelay));
    }

    private IEnumerator iEndGame(bool isDelay = true)
    {
        if (isDelay) yield return new WaitForSeconds(timeEnd);
        SceneManager.LoadScene("StartScene");
    }

    private void ResetVariables()
    {
        timeScare = 0.0f;
        scared = false;
        timeSpawnCherry = 30f;
        lives = 3;
    }

    public IEnumerator ResetScene()
    {
        CalmGhosts();

        pacman.transform.position = posPacMan;
        blinky.transform.position = posBlinky;
        pinky.transform.position = posPinky;
        inky.transform.position = posInky;
        clyde.transform.position = posClyde;

        pacman.GetComponent<PacStudentController>().ResetDestination();
        blinky.GetComponent<GhostController>().InitializeGhost();
        pinky.GetComponent<GhostController>().InitializeGhost();
        inky.GetComponent<GhostController>().InitializeGhost();
        clyde.GetComponent<GhostController>().InitializeGhost();

        gameState = GameState.Init;

        CherryController.instance.ResetCherry();

        yield return new WaitForSeconds(1f);
        gameState = GameState.Game;
    }

    public void ToggleScare()
    {
        if (!scared) ScareGhosts();
        else CalmGhosts();
    }

    public void ScareGhosts()
    {
        MusicManager.instance.Scared(timeScareAdd);
        //Debug.Log("Ghosts Scared");
        timeScare += timeScareAdd;

        scared = true;
        blinky.GetComponent<GhostController>().Frighten();
        pinky.GetComponent<GhostController>().Frighten();
        inky.GetComponent<GhostController>().Frighten();
        clyde.GetComponent<GhostController>().Frighten();
    }

    public void CalmGhosts()
    {
        scared = false;
        blinky.GetComponent<GhostController>().Calm();
        pinky.GetComponent<GhostController>().Calm();
        inky.GetComponent<GhostController>().Calm();
        clyde.GetComponent<GhostController>().Calm();
    }

    void AssignGhosts()
    {
        if (clyde == null)
        {
            clyde = GameObject.Find("clyde");
            posClyde = clyde.transform.position;
        }
        if (pinky == null)
        {
            pinky = GameObject.Find("pinky");
            posPinky = pinky.transform.position;
        }
        if (inky == null)
        {
            inky = GameObject.Find("inky");
            posInky = inky.transform.position;
        }
        if (blinky == null)
        {
            blinky = GameObject.Find("blinky");
            posBlinky = blinky.transform.position;
        }
        if (pacman == null)
        {
            pacman = GameObject.Find("PacStudent");
            posPacMan = pacman.transform.position;
        }
    }

    public void LoseLife()
    {
        lives--;
        gameState = GameState.Dead;

        GameGUINavigation.instance.ReduLife();
    }

    public void UpdateScore(int s, bool isPrf = false)
    {
        if(isPrf) pacman.GetComponent<PacStudentController>().UpdateScore(s);
        score += s;
    }
}
