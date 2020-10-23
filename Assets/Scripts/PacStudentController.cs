﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacStudentController : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.1f;
    [SerializeField]
    private List<PointScore> pointScores = new List<PointScore>();

    public Vector2 _dest = Vector2.zero;
    Vector2 oldP = Vector2.zero;
    public Vector2 currentInput = Vector2.zero;
    public Vector2 lastInput = Vector2.zero;

    [SerializeField]
    private AudioClip acDeath = null;
    private bool _deadPlaying = false;
    private Animator anim;
    private Rigidbody2D body;

    [SerializeField]
    private GameObject fxWalk = null;

    private Vector3 posOld;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        posOld = transform.position;
    }

    void Start()
    {
        _dest = transform.position;
        oldP = Vector2.MoveTowards(transform.position, _dest, speed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (GameManager.instance.gameState)
        {
            case GameManager.GameState.Game:
                ReadInputAndMove();
                Animate();
                break;
            case GameManager.GameState.Dead:
                if (!_deadPlaying)
                    StartCoroutine(PlayDeadAnimation());
                break;
        }
    }

    IEnumerator PlayDeadAnimation()
    {
        lastInput = Vector2.zero;
        _deadPlaying = true;
        MusicManager.instance.PlayOneShot(acDeath);
        anim.SetBool("Die", true);
        fxWalk.SetActive(false);

        if (GameManager.instance.lives <= 0)
        {
            //Debug.Log("Game Over");
            GameManager.instance.EndGame(false);
            yield return new WaitForSeconds(1);
            anim.speed = 0;
        }
        else
        {
            //Debug.Log("Reset Scene");
            yield return new WaitForSeconds(1);
            anim.SetBool("Die", false);
            _deadPlaying = false;
            
            StartCoroutine(GameManager.instance.ResetScene());
        }
    }

    void Animate()
    {
        Vector2 dir = _dest - (Vector2)transform.position;
        anim.SetFloat("DirX", dir.x);
        anim.SetFloat("DirY", dir.y);
    }

    bool Valid(Vector2 direction)
    {
        // cast line from 'next to pacman' to pacman
        // not from directly the center of next tile but just a little further from center of next tile
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
        RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
        return hit.collider.tag == "pellet" || hit.collider.tag == "ghost" || hit.collider == GetComponent<Collider2D>();
    }

    public void ResetDestination()
    {
        _dest = posOld;
        anim.SetFloat("DirX", 1);
        anim.SetFloat("DirY", 0);
    }
    
    void ReadInputAndMove()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
        body.MovePosition(p);
        MusicManager.instance.Walk(p != oldP);
        fxWalk.SetActive(p != oldP);
        oldP = p;

        // get the next direction from keyboard
        if (Input.GetAxis("Horizontal") > 0) lastInput = Vector2.right;
        if (Input.GetAxis("Horizontal") < 0) lastInput = -Vector2.right;
        if (Input.GetAxis("Vertical") > 0) lastInput = Vector2.up;
        if (Input.GetAxis("Vertical") < 0) lastInput = -Vector2.up;

        // if pacman is in the center of a tile
        if (Vector2.Distance(_dest , transform.position) < 0.00001f)
        {
            if (Valid(lastInput))
            {
                _dest = (Vector2)transform.position + lastInput;
                currentInput = lastInput;
                //Debug.Log("Valid(lastInput)");
            }
            else   // if next direction is not valid
            {
                if (Valid(currentInput))
                {
                    // and the prev. direction is valid
                    _dest  = (Vector2)transform.position + currentInput;   // continue on that direction
                    //Debug.Log("Valid(currentInput)");
                }
                // otherwise, do nothing
            }
        }
    }

    public Vector2 getDir()
    {
        return currentInput;
    }

    public void UpdateScore(int score)
    {
        for (int i = 0; i < pointScores.Count; i++)
        {
            if(pointScores[i].score == score)
            {
                Instantiate(pointScores[i].sprite, transform.position, Quaternion.identity);
            }
        }
    }
}

[System.Serializable]
public class PointScore
{
    public int score;
    public GameObject sprite;
}
