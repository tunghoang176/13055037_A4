using System;
using UnityEngine;
using System.Collections;

public class PacStudentController : MonoBehaviour
{
    public float speed = 0.4f;
    [SerializeField] GameObject[] pointSprites = null;

    Vector2 currentInput  = Vector2.zero;
    Vector2 _dir = Vector2.zero;
    Vector2 lastInput = Vector2.zero;

    public static int killstreak = 0;

    private bool _deadPlaying = false;
    private Animator anim;
    private Rigidbody2D body;

    private Vector3 posOld;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        posOld = transform.position;
    }

    void Start()
    {
        currentInput  = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (GameManager.gameState)
        {
            case GameManager.GameState.Game:
                ReadInputAndMove();
                Animate();
                break;
            case GameManager.GameState.Dead:
                if (!_deadPlaying)
                    StartCoroutine("PlayDeadAnimation");
                break;
        }
    }

    IEnumerator PlayDeadAnimation()
    {
        Debug.Log("Pacman Die: " + GameManager.lives);

        _deadPlaying = true;
        anim.SetBool("Die", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("Die", false);
        _deadPlaying = false;
        lastInput = Vector2.zero;

        if (GameManager.lives <= 0)
        {
            Debug.Log("Game Over");
            GameGUINavigation.instance.H_ShowGameOverScreen();
        }
        else
        {
            Debug.Log("Reset Scene");
            GameManager.instance.ResetScene();
        }
    }

    void Animate()
    {
        Vector2 dir = currentInput  - (Vector2)transform.position;
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
        return hit.collider.name == "pacdot" || (hit.collider == GetComponent<Collider2D>());
    }

    public void ResetDestination()
    {
        currentInput  = posOld;
        anim.SetFloat("DirX", 1);
        anim.SetFloat("DirY", 0);
    }

    void ReadInputAndMove()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, currentInput , speed);
        body.MovePosition(p);

        // get the next direction from keyboard
        if (Input.GetAxis("Horizontal") > 0) lastInput = Vector2.right;
        if (Input.GetAxis("Horizontal") < 0) lastInput = -Vector2.right;
        if (Input.GetAxis("Vertical") > 0) lastInput = Vector2.up;
        if (Input.GetAxis("Vertical") < 0) lastInput = -Vector2.up;

        // if pacman is in the center of a tile
        //if (Vector2.Distance(currentInput , transform.position) < 0.00001f)
        {
            //if (Valid(lastInput))
            {
                currentInput  = (Vector2)transform.position + lastInput;
                _dir = lastInput;
                //Debug.Log("Valid(lastInput)");
            }
            //else   // if next direction is not valid
            {
                //if (Valid(_dir))
                {
            //        // and the prev. direction is valid
                    //currentInput  = (Vector2)transform.position + _dir;   // continue on that direction
                    //Debug.Log("Valid(_dir)");
                }
            //    // otherwise, do nothing
            }
        }
    }

    public Vector2 getDir()
    {
        return _dir;
    }

    public void UpdateScore()
    {
        killstreak++;

        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;

        Instantiate(pointSprites[killstreak - 1], transform.position, Quaternion.identity);
        GameManager.score += (int)Mathf.Pow(2, killstreak) * 100;
    }
}
