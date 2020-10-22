using UnityEngine;
using System.Collections;

public class Energizer : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.name == "PacStudent")
        {
            MusicManager.instance.EatePowrPill();
            GameManager.instance.ScareGhosts();
            Destroy(gameObject);
        }
    }
}
