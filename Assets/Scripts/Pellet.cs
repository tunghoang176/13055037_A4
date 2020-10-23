using UnityEngine;
using System.Collections;

public class Pellet : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "PacStudent")
		{
			MusicManager.instance.EatPellet();
			GameManager.instance.UpdateScore(10);
		    GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pellet");
            Destroy(gameObject);

		    if (pacdots.Length == 1)
		    {
				GameManager.instance.EndGame(true);
		    }
		}
	}
}
