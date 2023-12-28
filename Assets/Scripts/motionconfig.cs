using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class motionconfig  {

    public float bodyDist = 17;
    public float handDist = 16;
    public bool debug = false;
    public float resetTime = 10;
    public float holdDistlimit = 0.15f;
    public float holdThreshold = 2;
    public float sensitivity = 19;
    public float SubmenuSensitivity = 30;
    public int holdIndicatorPos = 2;
    public bool traceHead = true;
    public bool traceDepth = true;
    public float SpeedX = 3.0f;
}
