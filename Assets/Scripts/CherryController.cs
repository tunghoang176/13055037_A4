using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public static CherryController instance = null;

    [SerializeField]
    private float timeSpawn = 30f;
    [SerializeField]
    private float minY = -13.5f;
    [SerializeField]
    private float maxY = 13.5f;
    [SerializeField]
    private GameObject bonusCherry = null;

    private List<GameObject> cherrys = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnCherry());
    }

    private IEnumerator SpawnCherry()
    {
        yield return new WaitForSeconds(timeSpawn);
        Debug.Log("Spawn Cherry");
        GameObject cherry = Instantiate(bonusCherry, transform);
        cherrys.Add(cherry);
        Vector3 pos = cherry.transform.position;
        pos.y += Random.Range(minY, maxY);
        cherry.transform.position = pos;

        StartCoroutine(SpawnCherry());
    }

    public void ResetCherry()
    {
        for (int i = 0; i < cherrys.Count; i++)
        {
            if (cherrys[i] != null) Destroy(cherrys[i]);
        }
    }
}
