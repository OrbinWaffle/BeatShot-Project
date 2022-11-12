using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, ISyncable
{
    [SerializeField] public GameObject[] spawnObjs;
    [SerializeField] private int beatsTillSpawn = 4;
    public int beat = 0;
    void Awake()
    {
        beat = beatsTillSpawn;
    }
    public void OnSync()
    {
        --beat;
        if(beat <= 0)
        {
            Spawn();
        }
    }
    void Spawn()
    {
        foreach (GameObject spawnObj in spawnObjs)
        {
            GameObject objInstance = Instantiate(spawnObj, transform.position, Quaternion.identity);
            ISyncable[] syncables = objInstance.GetComponentsInChildren<ISyncable>();
            foreach(ISyncable syncable in syncables)
            {
                RhythmManager.mainRM.AddSyncable(syncable);
                syncable.OnSync();
            }
            ISyncable[] mySyncables = GetComponentsInChildren<ISyncable>();
            foreach(ISyncable syncable in mySyncables)
            {
                RhythmManager.mainRM.RemoveSyncable(syncable);
            }
        }
        Destroy(gameObject);
    }
}
