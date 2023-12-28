using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashcontroller : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(fadeout());

    }

    IEnumerator fadeout()
    {
        Material m = gameObject.GetComponent<Renderer>().material;
        float alpha = 0;
        m.color = new Color(1, 1, 1, alpha);

        for(int i =0; i <= 10; i++)
        {
            alpha = i * 0.1f;
            m.color = new Color(1, 1, 1, 1-alpha);
            yield return new WaitForSeconds(0.1f);
        }
        
    }
	// Update is called once per frame
	void Update () {
		
	}
}
