using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float time = 0.0f;

    private ParticleSystem ps;
    private float t = 0.0f;

    [System.Obsolete]
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps != null) t = ps.duration + ps.startDelay;
    }

    private void Start()
    {
        if (time > 0.0f && time > t) t = time;
        StartCoroutine(_AutoDestroy());
    }

    private IEnumerator _AutoDestroy()
    {
        yield return new WaitForSeconds(t);
        if (this != null && gameObject != null) Destroy(gameObject);
    }
}
