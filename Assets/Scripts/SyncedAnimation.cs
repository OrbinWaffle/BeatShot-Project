
//This helper script triggers events in an animator when a beat is called.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedAnimation : MonoBehaviour, ISyncable
{
    [SerializeField] private int beatsPerAnim = 1;
    Animator anim;
    int beat = 1;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void OnSync()
    {
        anim.speed = RhythmManager.mainRM.GetTrueBPM() / 120f;
        if(beatsPerAnim == -1)
        {
            return;
        }
        try
        {
            beat--;
            if(beat <= 0)
            {
                anim.SetTrigger("Beat");
                beat = beatsPerAnim;
            }
        }
        catch
        {
            Debug.Log("um im dead actually probably so like i can't do this");
        }
    }
}
