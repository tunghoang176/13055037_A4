using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCherry : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float limitX = 30f;

    private void Update()
    {
        Vector3 pos = transform.position;
        if(pos.x >= limitX)
        {
            Destroy(gameObject);
            return;
        }
        pos.x += speed * Time.deltaTime;
        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "PacStudent")
        {
            GameManager.score += 100;
            Destroy(gameObject);
        }
    }
}
