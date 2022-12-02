using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccluderHelper : MonoBehaviour
{
    AudioSource aud;
    void Start()
    {
        aud = GetComponent<AudioSource>();
    }
    public void PlaySound()
    {
        aud.Play();
    }
}
