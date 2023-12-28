using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kv2LeftShoulderTracker : MonoBehaviour {
    public int bodyID;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        bodyGestureManager.Instance.updateLastKnowBodyLeftShoulder(bodyID, transform.position);

    }
}
