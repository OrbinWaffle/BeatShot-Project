using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour, ISyncable
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private SpawnInfo[] spawnInfos;
    [SerializeField] private Transform[] spawnZones;
    [SerializeField] private int spawnInterval = 4;
    int currentBeat = 1;
    void Start()
    {
        foreach(Transform spawnZone in spawnZones)
        {
            spawnZone.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSync()
    {
        currentBeat--;
        if(currentBeat <= 0)
        {
            Spawn();
            currentBeat = spawnInterval;
        }
    }

    void Spawn()
    {
        if(!isActive)
        {
            return;
        }
        GameObject enemyToSpawn = spawnInfos[0].prefab;
        float rand = Random.Range(0f, 1f);
        float cumulativeChance = 0;
        foreach(SpawnInfo info in spawnInfos)
        {
            cumulativeChance += info.chance;
            if(rand < cumulativeChance)
            {
                enemyToSpawn = info.prefab;
                break;
            }
        }

        if(RhythmManager.mainRM.isPlayerDead)
        {
            return;
        }
        Vector3 spawnPos = Vector3.zero;
        Transform tentativeSpawnZone = spawnZones[Random.Range(0, spawnZones.Length)];
        Transform spawnZone;
        if(tentativeSpawnZone.childCount > 0)
        {
            Transform[] children = new Transform[tentativeSpawnZone.childCount];
            for(int i = 0; i < tentativeSpawnZone.childCount; ++i)
            {
                children[i] = tentativeSpawnZone.GetChild(i);
            }
            spawnZone = children[Random.Range(0, children.Length)];
        }
        else
        {
            spawnZone = tentativeSpawnZone;
        }
        spawnPos = spawnZone.transform.position + new Vector3(Random.Range(-spawnZone.localScale.x/2, spawnZone.localScale.x/2), 0.5f, Random.Range(-spawnZone.localScale.z/2, spawnZone.localScale.z/2));
        GameObject enemyInstance = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        EnemyController EC = enemyInstance.GetComponentInChildren<EnemyController>();
        RhythmManager.mainRM.AddSyncable(EC);
        EC.OnSync();
        SyncedAnimation SA = enemyInstance.GetComponentInChildren<SyncedAnimation>();
        RhythmManager.mainRM.AddSyncable(SA);
        SA.OnSync();
    }
}

[System.Serializable]
public class SpawnInfo
{
    public GameObject prefab;
    public float chance;
}