﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderPhoto22 : MonoBehaviour {
    static public  RenderPhoto22 _instance;
    static public RenderPhoto22 Instance
    {
        get
        {
            return _instance;
        }
    }
    public Camera bgCam;

    public Camera bgCamSource;
    public Camera photoCam;

    //public Renderer bgTexPlane;
    //public Renderer arTexPlane;
    public Renderer frameTexPlane;

    public bool startCap;
    public bool photoReady = false;
    public RenderTexture bgTex;
    public RenderTexture photoTex;
    public RenderTexture frameTex;

    public Texture2D PrintPhoto;

    void Awake()
    {
        if(_instance == null)
            _instance = this;
    }
    // Use this for initialization
    void Start () {
		
	}

    IEnumerator capPhoto()
    {   
        //bgCam.CopyFrom(bgCamSource);
        bgCam.targetTexture = bgTex;

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture.active = bgTex;
        bgCam.Render();
       //arCam.Render();
        
        //bgTexPlane.material.mainTexture = bgCam.targetTexture;
        RenderTexture.active = photoTex;
        photoCam.Render();
        


        if (!PrintPhoto)
            PrintPhoto = new Texture2D(2048, 2048, TextureFormat.ARGB32, false,false);
        PrintPhoto.filterMode = FilterMode.Point;
        yield return new WaitForEndOfFrame();
        RenderTexture.active = photoTex;
        PrintPhoto.ReadPixels(new Rect(0, 0, photoTex.width, photoTex.height), 0, 0);
        PrintPhoto.Apply();
        yield return 0;
        photoReady = true;
        RenderTexture.active = currentRT; 
    }
	
	// Update is called once per frame
	void Update () {
        if (startCap)
        {
            photoReady = false;
            StartCoroutine(capPhoto());
            startCap = false;
        }
	}
}
