//myChange
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RhythmManager : MonoBehaviour
{
    [SerializeField] private bool useIntro = true;
    [SerializeField] private bool startOnPlay = false;
    [SerializeField] private MusicTrack musicTrackIntro;
    [SerializeField] private MusicTrack musicTrackMain;
    float timeOfLastBeat;
    private List<ISyncable> ObjsToSync = new List<ISyncable>();
    public static RhythmManager mainRM;
    float lastTime = 0;
    AudioSource audSource;
    bool doingIntro = true;
    MusicTrack currentTrack;
    int beatsLeftInIntro;
    //string webText;
    void Start()
    {
        mainRM = this;
        var syncObjs = FindObjectsOfType<MonoBehaviour>().OfType<ISyncable>();
        foreach(ISyncable obj in syncObjs)
        {
            ObjsToSync.Add(obj);
        }
        if(useIntro == true)
        {
            currentTrack = musicTrackIntro;
        }
        else
        {
            currentTrack = musicTrackMain;
            doingIntro = false;
        }
        audSource = GetComponent<AudioSource>();
        audSource.clip = currentTrack.song;
        audSource.loop = true;
        beatsLeftInIntro = (int)(musicTrackIntro.song.length/(1/(musicTrackIntro.BPM/60)));
        timeOfLastBeat = -1/(currentTrack.BPM/60);
        if(startOnPlay == true)
        {
            BeginPlaying();
        }
        //webText = "Note: This is the WebGL version of the game, so there may be some music sync issues. Switching tabs can cause some problems.";
    }
    public void BeginPlaying()
    {
        audSource.Play();
        //webText = "";
    }
    void Update()
    {
        checkForBeat(currentTrack);
    }
    void checkForBeat(MusicTrack MT)
    {
        if(!audSource.isPlaying){return;};
        if(audSource.time < lastTime)
        {
            timeOfLastBeat = -1/(MT.BPM/60);
        }
        if(audSource.time + MT.offset - timeOfLastBeat >= 1/(MT.BPM/60))
        {
            timeOfLastBeat += 1/(MT.BPM/60);
            Beat();
        }
        lastTime = audSource.time;
    }
    void Beat()
    {
        if(doingIntro)
        {
            if(beatsLeftInIntro <= 0)
            {
                SwitchToMain();
                StartCoroutine(DelayedBeat());
            }
            beatsLeftInIntro--;
            return;
        }
        foreach(ISyncable syncable in ObjsToSync)
        {
            syncable.OnSync();
        }
    }
    void SwitchToMain()
    {
        doingIntro = false;
        currentTrack = musicTrackMain;
        audSource.clip = musicTrackMain.song;
        audSource.loop = true;
        audSource.Play();
        timeOfLastBeat = 0;
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
    IEnumerator DelayedBeat()
    {
        yield return new WaitForEndOfFrame();
        Beat();
    }
    /*void OnGUI()
    {
        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(new Rect(Screen.width/2-150, Screen.height/2-200, 300, 100), webText);
    }*/
}
