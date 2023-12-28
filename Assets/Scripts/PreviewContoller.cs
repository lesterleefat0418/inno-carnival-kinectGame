using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewContoller : MonoBehaviour {

    public GameObject PreviewPlane;
    public GameObject flashPlane;

    public GameObject[] CountDown;

    public delegate void PreviewControlEvent();
    public PreviewControlEvent postPreviewEvent =null;


    // Use this for initialization
    void Start () 
    {
		
	}

    private void OnEnable()
    {
        StartCoroutine(StartPreview());
    }
  
    public IEnumerator StartPreviewseq()
    {   
        yield return StartPreview();
    }

    public void reset()
    {
       // StopAllCoroutines();
        for (int i = 0; i < CountDown.Length; i++)
            CountDown[i].SetActive(false);

        PreviewPlane.SetActive(false);
        flashPlane.SetActive(false);
    }

    IEnumerator StartPreview()
    {
        reset();
        CountDown[0].SetActive(true);
        yield return new WaitForSeconds(1);
        CountDown[0].SetActive(false);
        CountDown[1].SetActive(true);
        yield return new WaitForSeconds(1);
        CountDown[1].SetActive(false);
        CountDown[2].SetActive(true);
        yield return new WaitForSeconds(1);
        CountDown[2].SetActive(false);
        yield return 0;

        RenderPhoto.Instance.startCap = true;
        yield return 1;
        
        while (!RenderPhoto.Instance.photoReady)
            yield return 1;
        flashPlane.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        if (postPreviewEvent != null)
        {
            postPreviewEvent();
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
