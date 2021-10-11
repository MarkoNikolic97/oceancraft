using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveData : ScriptableObject
{
    public Vector3 WaveDirection;
    public float Wavelength = 10f;

    [Range(0, 1)]
    public float Steepness;
}
