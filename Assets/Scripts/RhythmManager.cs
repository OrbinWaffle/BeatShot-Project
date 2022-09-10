using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RhythmManager : MonoBehaviour
{
    public MusicTrack musicTrack;
    public int beatsTillStart = 4;
    float timeOfLastBeat;
    public List<ISyncable> ObjsToSync = new List<ISyncable>();
    public static RhythmManager mainRM;
    float lastTime = 0;
    AudioSource audSource;
    void Start()
    {
        mainRM = this;
        var syncObjs = FindObjectsOfType<MonoBehaviour>().OfType<ISyncable>();
        foreach(ISyncable obj in syncObjs)
        {
            ObjsToSync.Add(obj);
        }
        audSource = GetComponent<AudioSource>();
        audSource.clip = musicTrack.song;
    }
    public void BeginPlaying()
    {
        audSource.Play();
    }
    void Update()
    {
        if(audSource.time < lastTime)
        {
            timeOfLastBeat = -1/(musicTrack.BPM/60);
        }
        if(audSource.time + musicTrack.offset - timeOfLastBeat >= 1/(musicTrack.BPM/60))
        {
            timeOfLastBeat += 1/(musicTrack.BPM/60);
            Beat();
        }
        lastTime = audSource.time;
    }
    void Beat()
    {
        if(beatsTillStart > 1)
        {
            beatsTillStart--;
            return;
        }
        foreach(ISyncable syncable in ObjsToSync)
        {
            syncable.OnSync();
        }
    }
    public void AddSyncable(ISyncable syncable)
    {
        if(ObjsToSync.Contains(syncable)){return;}
        StartCoroutine(AddSyncableCoroutine(syncable));
    }
    public void RemoveSyncable(ISyncable syncable)
    {
        StartCoroutine(RemoveSyncableCoroutine(syncable));
    }
    IEnumerator AddSyncableCoroutine(ISyncable syncable)
    {
        yield return new WaitForEndOfFrame();
        ObjsToSync.Add(syncable);
    }
    IEnumerator RemoveSyncableCoroutine(ISyncable syncable)
    {
        yield return new WaitForEndOfFrame();
        ObjsToSync.Remove(syncable);
    }
}
