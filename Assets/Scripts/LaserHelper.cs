using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHelper : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Gradient gradient;
    public float alpha;
    // Start is called before the first frame update
    void Start()
    {
       lineRenderer = GetComponent<LineRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
       GradientAlphaKey[] frank = new GradientAlphaKey[1];
       frank[0].alpha = alpha;
       gradient.alphaKeys = frank;
       lineRenderer.colorGradient = gradient;
    //    lineRenderer.colorGradient.SetKeys(lineRenderer.colorGradient.colorKeys, frank);
    }
}
