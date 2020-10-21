using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "PacStudent")
		{
			GameManager.score += 10;
		    GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
            Destroy(gameObject);

		    if (pacdots.Length == 1)
		    {
		        GameManager.instance.LoadLevel();
		    }
		}
	}
}
