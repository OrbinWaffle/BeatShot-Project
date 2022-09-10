using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedAnimation : MonoBehaviour, ISyncable
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void OnSync()
    {
        anim.SetTrigger("Beat");
    }
}
