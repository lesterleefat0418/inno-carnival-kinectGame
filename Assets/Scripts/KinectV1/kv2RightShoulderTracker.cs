﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kv2RightShoulderTracker : MonoBehaviour {
    public int bodyID;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        bodyGestureManager.Instance.updateLastKnowBodyRightShoulder(bodyID, transform.position);
    }
}
