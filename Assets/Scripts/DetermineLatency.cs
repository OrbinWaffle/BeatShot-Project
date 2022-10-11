using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetermineLatency : MonoBehaviour
{
    float averageLatency = 0;
    public List<float> latencyMeasurements = new List<float>();
    void Update()
    {
        if(Input.anyKeyDown)
        {
            int rhythmScore;
            float time;
            RhythmManager.mainRM.RateTime(Time.time, out rhythmScore, out time);
            latencyMeasurements.Add(time);
            averageLatency = GetAverage(latencyMeasurements);
            Debug.Log(averageLatency);
        }
    }
    float GetAverage(List<float> list)
    {
        float total = 0;
        for(int i = 0; i < list.Count; ++i)
        {
            total += list[i];
        }
        return total/list.Count;
    }
    public void ClearList()
    {
        latencyMeasurements = new List<float>();
    }
}
