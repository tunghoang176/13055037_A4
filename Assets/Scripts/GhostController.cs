using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GhostController : MonoBehaviour
{
	[SerializeField]
	private float speed = 0.05f;

	// ----------------------------
	// Navigation variables
	private Vector3 waypoint;           // AI-determined waypoint
	private Queue<Vector3> waypoints;   // waypoints used on Init and Scatter states

	// direction is set from the AI component
	public Vector3 _direction;
	public Vector3 direction
	{
		get
		{
			return _direction;
		}

		set
		{
			_direction = value;
			Vector3 pos = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
			waypoint = pos + _direction;
			//Debug.Log ("waypoint (" + waypoint.position.x + ", " + waypoint.position.y + ") set! _direction: " + _direction.x + ", " + _direction.y);

		}
	}

	// ----------------------------
	// Ghost mode variables
	public float scatterLength = 5f;
	public float waitLength = 0.0f;

	private float timeToEndScatter;
	private float timeToEndWait;

	public enum State { Wait, Init, Scatter, Chase, Run };
	public State state;

	private Vector3 _startPos;
	private float _timeToWhite;
	private float _timeToToggleWhite;
	private float _toggleInterval;
	private bool isWhite = false;

	//-----------------------------------------------------------------------------------------
	// variables end, functions begin
	void Start()
	{
		_toggleInterval = GameManager.instance.timeScareAdd * 0.33f * 0.20f;
		InitializeGhost();

		_startPos = this.transform.position;
	}

	public float DISTANCE;

	void FixedUpdate()
	{
		DISTANCE = Vector3.Distance(transform.position, waypoint);

		if (GameManager.instance.gameState == GameManager.GameState.Game)
		{
			animate();

			switch (state)
			{
				case State.Wait:
					Wait();
					break;

				case State.Init:
					Init();
					break;

				case State.Scatter:
					Scatter();
					break;

				case State.Chase:
					ChaseAI();
					break;

				case State.Run:
					RunAway();
					break;
			}
		}
	}

	//-----------------------------------------------------------------------------------
	// Start() functions

	public void InitializeGhost()
	{
		waypoint = transform.position;  // to avoid flickering animation
		state = State.Wait;
		timeToEndWait = Time.time + waitLength;
		InitializeWaypoints(state);
	}

	public void InitializeGhost(Vector3 pos)
	{
		transform.position = pos;
		waypoint = transform.position;  // to avoid flickering animation
		state = State.Wait;
		timeToEndWait = Time.time + waitLength + 5f;
		InitializeWaypoints(state);
	}


	private void InitializeWaypoints(State st)
	{
		//--------------------------------------------------
		// File Format: Init and Scatter coordinates separated by empty line
		// Init X,Y 
		// Init X,Y
		// 
		// Scatter X,Y
		// Scatter X,Y

		//--------------------------------------------------
		// hardcode waypoints according to name.
		string data = "";
		switch (name)
		{
			case "ghost1":
				data = @"14.5 19
16 19

16 22
19 22
19 25
22 25
27 25
27 29
22 29";
				break;
			case "ghost2":
				data = @"13 14
14.5 14
14.5 12
13 12

13 9
10 9
13 6
13 2
2 2
2 9
7 9";
				break;
			case "ghost3":
				data = @"14.5 12
19 12
19 14
22 14

22 9
27 9
27 6
22 6
16 6
16 2
22 2
27 2";
				break;
			case "ghost4":
				data = @"16 17
14.5 17
14.5 19
13 19

13 22
10 22
10 25
13 25
13 29
7 29
7 25
7 22
2 22
2 29";
				break;

		}

		//-------------------------------------------------
		// read from the hardcoded waypoints
		string line;

		waypoints = new Queue<Vector3>();
		Vector3 wp;

		if (st == State.Init)
		{
			using (StringReader reader = new StringReader(data))
			{
				// stop reading if empty line is reached
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Length == 0) break; // DOES IT WORK????

					string[] values = line.Split(' ');
					float x = float.Parse(values[0]);
					float y = float.Parse(values[1]);

					wp = new Vector3(x, y, 0);
					waypoints.Enqueue(wp);
				}
			}
		}

		if (st == State.Scatter)
		{
			// skip until empty line is reached, read coordinates afterwards
			bool scatterWps = false;    // Scatter waypoints

			using (StringReader reader = new StringReader(data))
			{
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Length == 0)
					{
						scatterWps = true; // we reached the scatter waypoints
						continue; // do not read empty line, go to next line
					}

					if (scatterWps)
					{
						string[] values = line.Split(' ');
						int x = Int32.Parse(values[0]);
						int y = Int32.Parse(values[1]);

						wp = new Vector3(x, y, 0);
						waypoints.Enqueue(wp);
					}
				}
			}
		}

		// if in wait state, patrol vertically
		if (st == State.Wait)
		{
			Vector3 pos = transform.position;

			// ghost3 and ghost4 start going down and then up
			if (transform.name == "ghost3" || transform.name == "ghost4")
			{
				waypoints.Enqueue(new Vector3(pos.x, pos.y - 0.5f, 0f));
				waypoints.Enqueue(new Vector3(pos.x, pos.y + 0.5f, 0f));
			}
			// while ghost2 start going up and then down
			else
			{
				waypoints.Enqueue(new Vector3(pos.x, pos.y + 0.5f, 0f));
				waypoints.Enqueue(new Vector3(pos.x, pos.y - 0.5f, 0f));
			}
		}

	}

	//------------------------------------------------------------------------------------
	// Update functions
	void animate()
	{
		Vector3 dir = waypoint - transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
		GetComponent<Animator>().SetBool("Run", false);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "PacStudent")
		{
			//Destroy(other.gameObject);
			if (state == State.Run)
			{
				MusicManager.instance.EatGhost();
				GameManager.instance.UpdateScore(300, true);

				Calm();
				InitializeGhost(_startPos);
			}
			else
			{
				GameManager.instance.LoseLife();
			}
		}
	}

	//-----------------------------------------------------------------------------------
	// State functions
	void Wait()
	{
		if (Time.time >= timeToEndWait)
		{
			state = State.Init;
			waypoints.Clear();
			InitializeWaypoints(state);
		}

		// get the next waypoint and move towards it
		MoveToWaypoint(true);
	}

	void Init()
	{
		_timeToWhite = 0;

		// if the Queue is cleared, do some clean up and change the state
		if (waypoints.Count == 0)
		{
			state = State.Scatter;

			//get direction according to sprite name
			string name = GetComponent<SpriteRenderer>().sprite.name;
			if (name[name.Length - 1] == '0' || name[name.Length - 1] == '1') direction = Vector3.right;
			if (name[name.Length - 1] == '2' || name[name.Length - 1] == '3') direction = Vector3.left;
			if (name[name.Length - 1] == '4' || name[name.Length - 1] == '5') direction = Vector3.up;
			if (name[name.Length - 1] == '6' || name[name.Length - 1] == '7') direction = Vector3.down;

			InitializeWaypoints(state);
			timeToEndScatter = Time.time + scatterLength;

			return;
		}

		// get the next waypoint and move towards it
		MoveToWaypoint();
	}

	void Scatter()
	{
		if (Time.time >= timeToEndScatter)
		{
			waypoints.Clear();
			state = State.Chase;
			return;
		}

		// get the next waypoint and move towards it
		MoveToWaypoint(true);

	}

	void ChaseAI()
	{

		// if not at waypoint, move towards it
		if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}

		// if at waypoint, run AI module
		else GetComponent<AI>().AILogic();

	}

	void RunAway()
	{
		GetComponent<Animator>().SetBool("Run", true);

		if (Time.time >= _timeToWhite && Time.time >= _timeToToggleWhite) ToggleBlueWhite();

		// if not at waypoint, move towards it
		if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}

		// if at waypoint, run AI run away logic
		else GetComponent<AI>().RunLogic();

	}

	//------------------------------------------------------------------------------
	// Utility functions
	void MoveToWaypoint(bool loop = false)
	{
		waypoint = waypoints.Peek();        // get the waypoint (CHECK NULL?)
		if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)    // if its not reached
		{                                                           // move towards it
			_direction = Vector3.Normalize(waypoint - transform.position);  // dont screw up waypoint by calling public setter
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}
		else    // if waypoint is reached, remove it from the queue
		{
			if (loop) waypoints.Enqueue(waypoints.Dequeue());
			else waypoints.Dequeue();
		}
	}

	public void Frighten()
	{
		state = State.Run;
		_direction *= -1;

		_timeToWhite = Time.time + GameManager.instance.timeScareAdd * 0.66f;
		_timeToToggleWhite = _timeToWhite;
		GetComponent<Animator>().SetBool("Run_White", false);

	}

	public void Calm()
	{
		// if the ghost is not running, do nothing
		if (state != State.Run) return;

		waypoints.Clear();
		state = State.Chase;
		_timeToToggleWhite = 0;
		_timeToWhite = 0;
		GetComponent<Animator>().SetBool("Run_White", false);
		GetComponent<Animator>().SetBool("Run", false);
	}

	public void ToggleBlueWhite()
	{
		isWhite = !isWhite;
		GetComponent<Animator>().SetBool("Run_White", isWhite);
		_timeToToggleWhite = Time.time + _toggleInterval;
	}

}
