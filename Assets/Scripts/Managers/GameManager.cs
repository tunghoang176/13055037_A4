﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    //--------------------------------------------------------
    // Game variables

    public static int Level = 1;
    public static int lives = 3;

    public enum GameState { Init, Game, Dead, Scores }
    public static GameState gameState;

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
    private GameGUINavigation gui;

    private Vector3 posPacMan = new Vector3();
    private Vector3 posBlinky = new Vector3();
    private Vector3 posPinky = new Vector3();
    private Vector3 posInky = new Vector3();
    private Vector3 posClyde = new Vector3();

    public static bool scared;
    static public int score;
    static public int hightScore;

    public float scareLength;
    private float _timeToCalm;

    public float SpeedPerLevel;

    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    //-------------------------------------------------------------------
    // function definitions

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += LoadScene;

            hightScore = PlayerPrefs.GetInt("HightScore", 0);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }

        if (SceneManager.GetActiveScene().name == "StartScene") return;
        AssignGhosts();
    }

    void Start()
    {
        gameState = GameState.Init;
    }

    void LoadScene(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "StartScene") return;
        AssignGhosts();
        ResetVariables();

        clyde.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        blinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        pinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        inky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        pacman.GetComponent<PlayerController>().speed += Level * SpeedPerLevel / 2;
    }

    public void LoadLevel()
    {
        Level++;
        SceneManager.LoadScene("RoundStart");
    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
        scared = false;
        PlayerController.killstreak = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (scared && _timeToCalm <= Time.time)
            CalmGhosts();
    }

    public void ResetScene()
    {
        CalmGhosts();

        pacman.transform.position = posPacMan;
        blinky.transform.position = posBlinky;
        pinky.transform.position = posPinky;
        inky.transform.position = posInky;
        clyde.transform.position = posClyde;

        pacman.GetComponent<PlayerController>().ResetDestination();
        blinky.GetComponent<GhostMove>().InitializeGhost();
        pinky.GetComponent<GhostMove>().InitializeGhost();
        inky.GetComponent<GhostMove>().InitializeGhost();
        clyde.GetComponent<GhostMove>().InitializeGhost();

        gameState = GameState.Init;
        gui.H_ShowReadyScreen();

    }

    public void ToggleScare()
    {
        if (!scared) ScareGhosts();
        else CalmGhosts();
    }

    public void ScareGhosts()
    {
        scared = true;
        blinky.GetComponent<GhostMove>().Frighten();
        pinky.GetComponent<GhostMove>().Frighten();
        inky.GetComponent<GhostMove>().Frighten();
        clyde.GetComponent<GhostMove>().Frighten();
        _timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
    }

    public void CalmGhosts()
    {
        scared = false;
        blinky.GetComponent<GhostMove>().Calm();
        pinky.GetComponent<GhostMove>().Calm();
        inky.GetComponent<GhostMove>().Calm();
        clyde.GetComponent<GhostMove>().Calm();
        PlayerController.killstreak = 0;
    }

    void AssignGhosts()
    {
        // find and assign ghosts
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
            pacman = GameObject.Find("pacman");
            posPacMan = pacman.transform.position;
        }

        if (clyde == null || pinky == null || inky == null || blinky == null) Debug.Log("One of ghosts are NULL");
        if (pacman == null) Debug.Log("Pacman is NULL");

        gui = GameObject.FindObjectOfType<GameGUINavigation>();

        if (gui == null) Debug.Log("GUI Handle Null!");
    }

    public void LoseLife()
    {
        lives--;
        gameState = GameState.Dead;

        // update UI too
        UIScript ui = GameObject.FindObjectOfType<UIScript>();
        Destroy(ui.lives[ui.lives.Count - 1]);
        ui.lives.RemoveAt(ui.lives.Count - 1);
    }

    public static void DestroySelf()
    {
        score = 0;
        Level = 0;
        lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }
}
