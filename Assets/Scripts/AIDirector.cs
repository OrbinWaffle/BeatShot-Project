using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour, ISyncable
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private SpawnInfo[] spawnInfos;
    [SerializeField] private Transform[] spawnZones;
    [SerializeField] private int spawnInterval = 4;
    [Tooltip("Number of beats until enemiesPerInterval is incremented")]
    [SerializeField] private int diffScaleBeats = 16;
    [SerializeField] private int enemiesPerInterval = 1;
    [SerializeField] private int maxEnemies = 10;
    int currentBeat = 1;
    int diffBeatCounter = 1;
    void Start()
    {
        diffBeatCounter = diffScaleBeats;
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
        diffBeatCounter--;
        if(currentBeat <= 0)
        {
            Spawn();
            currentBeat = spawnInterval;
        }
        if(diffBeatCounter <= 0)
        {
            diffBeatCounter = diffScaleBeats;
            if(enemiesPerInterval >= maxEnemies)
            {
                return;
            }
            ++enemiesPerInterval;
            UIManager.mainUIM.IncrementStars();
        }
    }

    void Spawn()
    {
        if(!isActive)
        {
            return;
        }
        for(int j = 0 ; j < enemiesPerInterval; ++j)
        {
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
            spawnPos = spawnZone.transform.position + new Vector3(Random.Range(-spawnZone.localScale.x/2, spawnZone.localScale.x/2), 0f, Random.Range(-spawnZone.localScale.z/2, spawnZone.localScale.z/2));
            GameObject spawnedObj = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
            ISyncable[] syncables = spawnedObj.GetComponentsInChildren<ISyncable>();
            foreach(ISyncable syncable in syncables)
            {
                RhythmManager.mainRM.AddSyncable(syncable);
                syncable.OnSync();
            }
        }
    }
}

[System.Serializable]
public class SpawnInfo
{
    public GameObject prefab;
    public float chance;
}