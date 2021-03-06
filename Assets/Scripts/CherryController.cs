﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public static CherryController instance = null;

    [SerializeField]
    private float minY = 2f;
    [SerializeField]
    private float maxY = 29f;
    [SerializeField]
    private GameObject bonusCherry = null;

    private List<GameObject> cherrys = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }


    public void SpawnCherry()
    {
        //Debug.Log("Spawn Cherry");
        GameObject cherry = Instantiate(bonusCherry, transform);
        cherrys.Add(cherry);
        Vector3 pos = cherry.transform.position;
        pos.y = Random.Range(minY, maxY);
        cherry.transform.position = pos;
    }

    public void ResetCherry()
    {
        for (int i = 0; i < cherrys.Count; i++)
        {
            if (cherrys[i] != null) Destroy(cherrys[i]);
        }
    }
}
