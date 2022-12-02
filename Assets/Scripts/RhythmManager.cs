
//This script keeps track of the beats of the music, and syncs the whole game to it.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.SceneManagement;

public class RhythmManager : MonoBehaviour
{
    [Tooltip("Whether or not this song has an intro.")]
    [SerializeField] private bool useIntro = true;
    [Tooltip("Whether or not the song begins playing immediately when the game starts running.")]
    [SerializeField] private bool startOnPlay = false;
    [Tooltip("When the time difference between the input and the beat is less than this number, it is registered as a hit.")]
    [SerializeField] public float rhythmLeeway = 0.1f;
    [Tooltip("A measurement of the player's input lag. Used to adjust rhythm registration.")]
    [SerializeField] private float latency = 0f;
    [Tooltip("Music track for the intro.")]
    [SerializeField] private MusicTrack musicTrackIntro;
    [Tooltip("Main looping music track.")]
    [SerializeField] private MusicTrack musicTrackMain;
    [Tooltip("Master audio mixer.")]
    [SerializeField] private AudioMixer masterMixer;
    [Tooltip("Speed of snapshot transitions.")]
    [SerializeField] private float snapshotSpeed = 1f;
    [Tooltip("Mixer snapshot that will play on death.")]
    [SerializeField] private AudioMixerSnapshot deathSnapshot;
    public bool isPlayerDead = false;
    static AudioMixerSnapshot orgSnapShot;
    //Time that the last beat occured at.
    float timeOfLastBeat;
    //A list of objects that must be synced to the music.
    private List<ISyncable> ObjsToSync = new List<ISyncable>();
    public static RhythmManager mainRM;
    //Time of the music on the previous frame. Used to detect when the song loops.
    float lastTime = 0;
    //The BPM when the music play speed is taken into account.
    float trueBPM = 0;
    float timeLoopBegan;
    AudioSource audSource;
    bool doingMain = false;
    MusicTrack currentTrack;
    int beatsLeftInIntro;
    [Tooltip("How many beats have passed since the game started.")]
    public int beatsSurvived = 0;
    void Awake()
    {
        masterMixer.FindSnapshot("Default").TransitionTo(0f);
        //DataWriter.resetValues();
    }
    void Start()
    {
        mainRM = this;
        FindAllSyncables();
        if(useIntro == true)
        {
            currentTrack = musicTrackIntro;
        }
        else
        {
            currentTrack = musicTrackMain;
            doingMain = true;
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
    //Simply begins the audio.
    public void BeginPlaying()
    {
        audSource.Play();
        //webText = "";
    }
    void Update()
    {
        checkForBeat(currentTrack);
    }
    //Checks to see if a beat has occured since the last frame.
    void checkForBeat(MusicTrack MT)
    {
        if(!audSource.isPlaying){return;};
        //If the song has looped, reset the timeOfLastBeat.
        if(audSource.time < lastTime)
        {
            timeOfLastBeat = -1/(MT.BPM/60);
            timeLoopBegan = Time.time;
        }
        //Registers when a beat has occured.
        if(audSource.time + MT.offset - timeOfLastBeat >= 1/(MT.BPM/60))
        {
            timeOfLastBeat += 1/(MT.BPM/60);
            Beat();
        }
        lastTime = audSource.time;
    }
    //When this method is called, it will contact every object in the ObjsToSync list and tells them that a beat has occured.
    void Beat()
    {
        //If the intro is currently playing, don't call a beat yet.
        if(!doingMain)
        {
            if(beatsLeftInIntro <= 0)
            {
                SwitchToMain();
                StartCoroutine(DelayedBeat());
            }
            beatsLeftInIntro--;
            return;
        }
        if(isPlayerDead == false)
        {
            RhythmManager.mainRM.beatsSurvived++;
        }
        float masterPitch;
        masterMixer.GetFloat("MasterPitch", out masterPitch);
        trueBPM = musicTrackMain.BPM * audSource.pitch * masterPitch;
        //Calls a beat on every object in ObjsToSync.
        foreach(ISyncable syncable in ObjsToSync)
        {
            syncable.OnSync();
        }
    }
    //Switches the music track to the main looping track.
    void SwitchToMain()
    {
        doingMain = true;
        currentTrack = musicTrackMain;
        audSource.clip = musicTrackMain.song;
        audSource.loop = true;
        audSource.Play();
        timeOfLastBeat = 0;
        timeLoopBegan = Time.time;
    }
    //Takes a time as a float value and returns an int defining if it was on the beat.
    //0 means hit, 1 means miss, -1 means N/A.
    public void RateTime(float timeToRate, out int rating, out float time, out float trueTime)
    {
        if(!doingMain)
        {
            rating = -1;
            time = 0;
            trueTime = 0;
            return;
        }
        float timeToRateAdjusted = timeToRate - latency;
        float lastBeatDiff = timeToRateAdjusted - GetRhythmTimeNormalized(timeOfLastBeat);
        float nextBeatTime = GetNextBeat();
        float nextBeatDiff = timeToRateAdjusted - nextBeatTime;
        //Debug.Log(lastBeatDiff + "   " + nextBeatDiff);
        float closestDiff = Mathf.Abs(lastBeatDiff) <= Mathf.Abs(nextBeatDiff) ? lastBeatDiff : nextBeatDiff;
        //Debug.Log(closestDiff.ToString("F3") + " seconds " + (closestDiff<=0?"early":"late"));
        if(Mathf.Abs(closestDiff) < rhythmLeeway){rating = 1;}
        else{rating = 0;}
        time = closestDiff;
        trueTime = closestDiff + latency;
    }
    // Override that does not include time or trueTime
    public void RateTime(float timeToRate, out int rating)
    {
        float time;
        float trueTime;
        RateTime(timeToRate, out rating, out time, out trueTime);
    }
    // Override that does not include trueTIme.
    public void RateTime(float timeToRate, out int rating, out float time)
    {
        float trueTime;
        RateTime(timeToRate, out rating, out time, out trueTime);
    }
    public float GetRhythmTimeNormalized(float rhythmTime)
    {
        return(timeLoopBegan + rhythmTime);
    }
    public float GetNextBeat()
    {
        return(timeLoopBegan + (timeOfLastBeat + 1/(currentTrack.BPM/60)));
    }
    //Adds a new ISyncable object to the list.
    public void AddSyncable(ISyncable syncable)
    {
        if(ObjsToSync.Contains(syncable)){return;}
        StartCoroutine(AddSyncableCoroutine(syncable));
    }
    //Removes an ISyncable object from the list.
    public void RemoveSyncable(ISyncable syncable)
    {
        StartCoroutine(RemoveSyncableCoroutine(syncable));
    }
    /*
    The add/remove methods wait until the end of the frame to actually add the object.
    This is because errors can occur if an object is added at the same time a beat is
    currently being called on every object.
    */
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
    /*
    A beat called at the end of the frame instead of the start.
    This is used to ensure that a beat is not missed when switching
    to the main loop.
    */
    IEnumerator DelayedBeat()
    {
        yield return new WaitForEndOfFrame();
        Beat();
    }
    public void DeathNotification()
    {
        isPlayerDead = true;
        deathSnapshot.TransitionTo(snapshotSpeed);
        // DataWriter.writeScore(SceneManager.GetActiveScene().name, beatsSurvived.ToString());
    }
    //Finds every ISyncable object in the scene and adds it to the ObjsToSync list.
    void FindAllSyncables()
    {
        var syncObjs = FindObjectsOfType<MonoBehaviour>().OfType<ISyncable>();
        foreach(ISyncable obj in syncObjs)
        {
            ObjsToSync.Add(obj);
        }
    }
    public float GetTrueBPM()
    {
        return trueBPM;
    }
    /*void OnGUI()
    {
        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(new Rect(Screen.width/2-150, Screen.height/2-200, 300, 100), webText);
    }*/
}
