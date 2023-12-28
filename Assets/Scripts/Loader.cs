using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using LitJson;
using ZXing;
using ZXing.QrCode;
using UnityEngine.Networking;

public class Loader : MonoBehaviour {

    public enum MethodOfGetPhoto
    {
        sms,
        qrCode
    }

    class getPhotoRequest
    {
        public int id;
        public string message;
        public int result_sms;
        public string result_bitly;
    }
    class configFile { 
        public string altImgPath = "";
        public float thanksPageLength =10;
        public int copies =1;
        }
    Action currentState = null;
    Action currentPreviewEvent = null;

    public MethodOfGetPhoto methodOfGetPhoto = MethodOfGetPhoto.sms;

    public bodyGestureManager bodyGestureMgr;
    public GameObject Startpage;
    //public GameObject instructpage;
    public GameObject Act1Page;
    public GameObject Act2Page;
    public GameObject Act3Page;
    //public GameObject Act4Page;
    public GameObject FinalPreviewPage;
    public GameObject getPhotoPage;
    //public GameObject printPhotoPage;

    public GameObject PreviewObj;
    public Renderer[] previewRenderer = new Renderer[4];
    public Renderer FinalPreview;
    public Transform phoneSendBtn;
    public Transform SkipBtn;
    public softkeyboardMgr phoneNoVK;
    public softkeyboardMgr phoneNoVK2;
    public Transform VKSwitch1;
    public Transform VKSwitch2;
    public GameObject nowPrinting;
    public GameObject pose1Ident;
    public GameObject pose2Ident;
    public GameObject pose3Ident;
    public TextMesh gestureInfo;
    public AudioSource SFX;

    public Camera UICam;
    public Image qrImage, qrPrepareUpload;

    Texture[] photos = new Texture[4];
    configFile cfgFile = new configFile();

    // Use this for initialization
    void Start () {
        Screen.SetResolution(1080, 1920, true);
        Cursor.visible = false;
        currentState = stateIdle;
        Startpage.SetActive(true);
        if (qrImage && qrPrepareUpload)
        {
            qrImage.enabled = false;
            qrImage.sprite = null;
            qrPrepareUpload.enabled = true;
        }

        PreviewObj.GetComponent<PreviewContoller>().postPreviewEvent = onPostPreview;
        //PrinterController.Instance.copies =1;

        Debug.Log(Application.persistentDataPath + "/config.json");
        if (System.IO.File.Exists(Application.persistentDataPath + "/config.json"))
        {
            cfgFile = JsonUtility.FromJson<configFile>(System.IO.File.ReadAllText(Application.persistentDataPath + "/config.json"));
            applyConfig();
        }
        initConfig();        
    }
	
    void onPostPreview()
    {
        currentPreviewEvent.Invoke();
    }

    bool isGestureDetected(int gestID)
    {
        gestureInfo.text ="";
        if (gestID == 0)
            return isGesture1()|| Input.GetMouseButton(0);
        if (gestID == 1)
            return isGesture2() || Input.GetMouseButton(0);
        if (gestID == 2)
            return isGesture3() || Input.GetMouseButton(0);
       
        return Input.GetMouseButton(0);
    }
    bool isInput()
    {
        return Input.GetMouseButtonUp(0);
    }
    

    bool singlePlay = false;
    bodyGestureManager.kinnectBody player1 =null;
    bodyGestureManager.kinnectBody player2 = null;
    void updateBodies()
    {
        player1 = null; player2 = null;
        if (bodyGestureManager.Instance.bodyMap.Count >0)
        {
            foreach(bodyGestureManager.kinnectBody b in bodyGestureManager.Instance.bodyMap.Values)
            {
                if(player1 == null)
                {
                    player1 = b;
                }
                else
                {
                    if(b.bodyPos.z < player1.bodyPos.z && player1.bodyID != b.bodyID)
                    {
                        player1 = b;
                    }
                }                
            }
        }
        if(bodyGestureManager.Instance.bodyMap.Count > 1)
        {
            foreach (bodyGestureManager.kinnectBody b in bodyGestureManager.Instance.bodyMap.Values)
            {
                if (player1.bodyID != b.bodyID)
                    if (player2 == null )
                {
                    player2 = b;
                }
            }
        }
    }
    
    bool isGesture2()
    {
        gestureInfo.text += "isGesture3\n";
        if (player1 != null && player2 != null)
        {
            gestureInfo.text += String.Format("body1:  up1:{0} \n", player1.rightPos.y > player1.rShoulderPos.y);
            gestureInfo.text += String.Format("body2:  up1:{0} \n", player2.leftPos.y > player2.lShoulderPos.y);
           
            gestureInfo.text += String.Format("body2:  up2:{0} \n", player1.leftPos.y > player1.lShoulderPos.y);
            gestureInfo.text += String.Format("body1:  up2:{0} \n", player2.rightPos.y > player2.rShoulderPos.y);
            if ((player1.rightPos.y > player1.rShoulderPos.y) &&(player2.leftPos.y > player2.lShoulderPos.y)
                && (player2.rightPos.y < player2.rShoulderPos.y) && (player1.leftPos.y < player1.lShoulderPos.y))
            {
                return true;
            }
            else
            {
                if ((player2.rightPos.y > player2.rShoulderPos.y) &&(player1.leftPos.y > player1.lShoulderPos.y)
                    && (player1.rightPos.y < player1.rShoulderPos.y) && (player2.leftPos.y < player2.lShoulderPos.y))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            if (singlePlay && player1 != null)
            {
                gestureInfo.text += String.Format("hand:  dist:{0} \n", Vector3.Distance(player1.leftPos, player1.rightPos));
                if (!(Vector3.Distance(player1.leftPos, player1.lShoulderPos) < 1.5f) || !(Vector3.Distance(player1.rightPos, player1.rShoulderPos) < 1.5f))
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }
    bool isGesture3()
    {
        gestureInfo.text += "isGesture2\n";
        if (player1 != null && player2 != null)
        {
            gestureInfo.text += String.Format("body1: Left:{0} Right:{1}\n", Vector3.Dot(player1.lShoulderPos - player1.leftPos, Vector3.up), Vector3.Dot((player1.rShoulderPos - player1.rightPos), Vector3.up));
            if (!(Vector3.Dot(player1.lShoulderPos - player1.leftPos, Vector3.up) < 0f) || !(Vector3.Dot((player1.rShoulderPos - player1.rightPos), Vector3.up) < 0f))
            {
                return false;
            }
            gestureInfo.text += String.Format("body1:  dist:{0} \n", Vector3.Distance(player1.leftPos, player1.rightPos));
            if (!(Vector3.Distance(player1.leftPos, player1.rightPos) < 4.5f))
            {
                return false;
            }
            gestureInfo.text += String.Format("body2: Left:{0} Right:{1}\n", Vector3.Dot(player2.lShoulderPos - player2.leftPos, Vector3.up), Vector3.Dot((player2.rShoulderPos - player2.rightPos), Vector3.up));
            if (!(Vector3.Dot((player2.lShoulderPos - player2.leftPos), Vector3.up) < 0f) || !(Vector3.Dot((player2.rShoulderPos - player2.rightPos), Vector3.up) < 0f))
            {
                return false;
            }
            gestureInfo.text += String.Format("body2: dist:{0} \n", Vector3.Distance(player2.leftPos, player2.rightPos));
            if (!(Vector3.Distance(player2.leftPos, player2.rightPos) <4.5f))
            {
                return false;
            }
            return true;
        }
        else
        {
            if (singlePlay && player1 != null)
            {
                gestureInfo.text += String.Format("body1: Left:{0} Right:{1}\n", Vector3.Dot(player1.lShoulderPos - player1.leftPos, Vector3.up), Vector3.Dot((player1.rShoulderPos - player1.rightPos), Vector3.up));
                gestureInfo.text += String.Format("body1: Left dist:{0} Right dist:{1}\n", Vector3.Distance(player1.leftPos, player1.headPos), Vector3.Distance(player1.rightPos, player1.headPos));

                if (!(Vector3.Dot(player1.lShoulderPos - player1.leftPos, Vector3.up) < 0f) || !(Vector3.Dot((player1.rShoulderPos - player1.rightPos), Vector3.up) < 0f))
                {
                    return false;
                }
                if (!(Vector3.Distance(player1.leftPos, player1.rightPos) < 1.5f))
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }
    bool isGesture1()
    {
        gestureInfo.text += "isGesture1\n";
        if (player1 != null && player2 != null)
        {
            bool result = true;
            gestureInfo.text += String.Format("body1: Left :{0} Right :{1} Spine:{2}\n", player1.leftPos.y, player1.rightPos.y, player1.SpineShoulderPos.y);
            if (player1.bodyPos.x < 0)
            {
                if (!(player1.leftPos.y > player1.SpineShoulderPos.y) || !(player1.rightPos.y < player1.SpineShoulderPos.y))
                {
                    result = false;
                }
            }
            else
            {
                if (!(player1.leftPos.y < player1.SpineShoulderPos.y) || !(player1.rightPos.y > player1.SpineShoulderPos.y))
                {
                    result = false;
                }
            }
            gestureInfo.text += String.Format("body2: Left :{0} Right :{1} Spine:{2}\n", player2.leftPos.y, player2.rightPos.y, player2.SpineShoulderPos.y);
            if (player2.bodyPos.x < 0)
            {
                if (!(player2.leftPos.y > player2.SpineShoulderPos.y) || !(player2.rightPos.y < player2.SpineShoulderPos.y))
                {
                    result = false;
                }
            }
            else
            {
                if (!(player2.leftPos.y < player2.SpineShoulderPos.y) || !(player2.rightPos.y > player2.SpineShoulderPos.y))
                {
                    result = false;
                }
            }
            return result;
        }
        else
        {
            if (singlePlay && player1 != null)
            {
                bool result = true;
                gestureInfo.text += String.Format("body1: Left :{0} Right :{1} Spine:{2}\n", player1.leftPos.y, player1.rightPos.y, player1.SpineShoulderPos.y);

                if (player1.bodyPos.x < 0)
                {
                    if (!(player1.leftPos.y > player1.SpineShoulderPos.y) || !(player1.rightPos.y < player1.SpineShoulderPos.y))
                    {
                        result = false;
                    }
                }
                else
                {
                    if (!(player1.leftPos.y < player1.SpineShoulderPos.y) || !(player1.rightPos.y > player1.SpineShoulderPos.y))
                    {
                        result = false;
                    }
                }
                return result;
            }
        }
        return false;
    }
    void stateIdle()
    {
        if (isInput())
        {
            SFX.PlayOneShot(SFX.clip);
            Startpage.SetActive(false);

            if(qrImage && qrPrepareUpload)
            {
                qrImage.enabled = false;
                qrImage.sprite = null;
                qrPrepareUpload.enabled = true;
            }

            RenderPhoto.Instance.photoReady = false;
            RenderPhoto22.Instance.photoReady = false;
            RenderPhoto.Instance.PrintPhoto = null;
            RenderPhoto22.Instance.PrintPhoto = null;

            Act1Page.SetActive(true);
            gestureTimer = 0;
            currentState = statAct1;
            actActiveTimer =0;

            pose1Ident.SetActive(false);
        }
    }
    float gestureTimer =0;
    float gestureActiveTime = 1;
    
    float actActiveTimer =0;
    float actActiveTimerLimit = 3;
    void statAct1()
    {
        actActiveTimer += Time.deltaTime;
        if (player1 != null && player2 != null)
        {
            pose1Ident.SetActive(false);
        }
        else
        {
            pose1Ident.SetActive(true);
        }
        if (isGestureDetected(0) && actActiveTimer > actActiveTimerLimit)
        {
            gestureTimer  += Time.deltaTime;
            if(gestureTimer > gestureActiveTime || Input.GetMouseButton(0))
            {
                gestureTimer = 0;
                SFX.PlayOneShot(SFX.clip);
                currentState = statAct1Preview;
                currentPreviewEvent = Act1postPreview;
                PreviewObj.SetActive(true);
            }
        }
        else
        {
            if(gestureTimer >0)
                gestureTimer -= Time.deltaTime;
        }
    }

    void Act1postPreview()
    {
        PreviewObj.SetActive(false);
        photos[0] = Instantiate<Texture2D>( RenderPhoto.Instance.PrintPhoto);
        previewRenderer[0].material.mainTexture = photos[0];
        Act1Page.SetActive(false);
        Act2Page.SetActive(true);
        actActiveTimer =0;
        gestureTimer =0;
        currentState = statAct2;
        pose2Ident.SetActive(false);
    }

    void statAct1Preview()
    {

    }

    void statAct2()
    {
        actActiveTimer += Time.deltaTime;
        if (player1 != null && player2 != null)
        {
            pose2Ident.SetActive(false);
        }
        else
        {
            pose2Ident.SetActive(true);
        }
        if (isGestureDetected(1) && actActiveTimer > actActiveTimerLimit)
        {
            gestureTimer += Time.deltaTime;
            if (gestureTimer > gestureActiveTime|| Input.GetMouseButton(0))
            {
                gestureTimer = 0;
                SFX.PlayOneShot(SFX.clip);
                currentState = statAct2Preview;
                currentPreviewEvent = Act2postPreview;           
                PreviewObj.SetActive(true);
                actActiveTimer =0;
            }
        }
        else
        {
            if (gestureTimer > 0)
                gestureTimer -= Time.deltaTime;
        }
    }

    void statAct2Preview()
    {

    }

    void Act2postPreview()
    {
        PreviewObj.SetActive(false);
        photos[1] = Instantiate<Texture2D>(RenderPhoto.Instance.PrintPhoto);
        previewRenderer[1].material.mainTexture = photos[1];
        Act2Page.SetActive(false);
        Act3Page.SetActive(true);
        actActiveTimer =0;
        gestureTimer =0;
        currentState = statAct3;
        pose3Ident.SetActive(true);
    }

    void statAct3()
    {
        actActiveTimer+= Time.deltaTime;
        if(player1 != null && player2 != null)
        {
            pose3Ident.SetActive(false);
        }
        else
        {
            pose3Ident.SetActive(true);
        }
            
        if (isGestureDetected(2) && actActiveTimer> actActiveTimerLimit)
        {
            gestureTimer += Time.deltaTime;
            if (gestureTimer > gestureActiveTime|| Input.GetMouseButton(0))
            {                
                gestureTimer = 0;
                SFX.PlayOneShot(SFX.clip);
                currentState = statAct3Preview;
                currentPreviewEvent = Act3postPreview;
                PreviewObj.SetActive(true); 
            }
        }
        else
        {
            if (gestureTimer > 0)
                gestureTimer -= Time.deltaTime;
        }

    }
    void statAct3Preview()
    {

    }
    void Act3postPreview()
    {
        PreviewObj.SetActive(false);
        photos[2] = Instantiate<Texture2D>(RenderPhoto.Instance.PrintPhoto);
        previewRenderer[2].material.mainTexture = photos[2];       
        RenderPhoto22.Instance.startCap = true;
        currentState = waitFinalpreview;
        StartCoroutine(waitFinal());
    }
    IEnumerator waitFinal()
    {
        while (!RenderPhoto22.Instance.photoReady)
        {
            yield return 0;
        }
        FinalPreview.material.mainTexture = RenderPhoto22.Instance.PrintPhoto;
        nowPrinting.SetActive(false);
        waitingAPI = false;

        if (methodOfGetPhoto == MethodOfGetPhoto.sms)
        {
            Act3Page.SetActive(false);
            FinalPreviewPage.SetActive(true);
            phoneNoVK.Reset();
            phoneNoVK2.Reset();
            phoneNoVK.gameObject.SetActive(true);
            phoneNoVK2.gameObject.SetActive(false);
            currentState = statFinalPreviewPage;
        }
        else
        {
            //upload to server for QR code
            yield return waitForAPI(RenderPhoto22.Instance.PrintPhoto);
        }
    }
    void waitFinalpreview()
    {

    }
    void statFinalPreviewPage()
    {
        Act3Page.SetActive(false);
        FinalPreviewPage.SetActive(true);

        if (isInput() /*&& !waitingAPI*/)
        {
            
            RaycastHit hit;
            if (Physics.Raycast(UICam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                /* if(methodOfGetPhoto == MethodOfGetPhoto.sms) { 
                     if(hit.transform == VKSwitch1)
                     {
                         phoneNoVK.gameObject.SetActive(true);
                         phoneNoVK2.gameObject.SetActive(false);

                     }
                     if (hit.transform == VKSwitch2)
                     {
                         phoneNoVK2.gameObject.SetActive(true);
                         phoneNoVK.gameObject.SetActive(false);
                     }
                     if (hit.transform == phoneSendBtn)
                     {
                         if (phoneNoVK.textInput.text.Length < 8 && phoneNoVK2.textInput.text.Length < 8) 
                             return;
                         SFX.PlayOneShot(SFX.clip);
                         waitingAPI = true;
                         PrinterController.Instance.StartPrint(RenderPhoto22.Instance.PrintPhoto);
                         nowPrinting.SetActive(true);
                         StartCoroutine(waitForAPI(0));
                     }
                 }*/
                //Get QR code
                if (hit.transform == phoneSendBtn && !waitingAPI)
                {
                    SFX.PlayOneShot(SFX.clip);
                    waitingAPI = true;
                    //PrinterController.Instance.StartPrint(RenderPhoto22.Instance.PrintPhoto);
                    nowPrinting.SetActive(true);
                }

                //skip
                if (hit.transform == SkipBtn)
                {
                    Debug.Log("back to first page");
                    SFX.PlayOneShot(SFX.clip);
                    FinalPreviewPage.SetActive(false);
                    currentState = stateIdle;
                    Startpage.SetActive(true);
                }
            }
        }        
    }

    void onGetPhotoFinished()
    {
        getPhotoPage.SetActive(true);       
        FinalPreviewPage.SetActive(false);
        currentState = stateGetPhoto;
        endTimer =0;
    }
    IEnumerator waitForAPI(Texture2D finalImage)
    {
        //Debug.Log("Retry:"+retry);
        /*if(retry == 2)
        {
            onGetPhotoFinished();
            yield break;
        }*/
        WWWForm photoReq = new WWWForm();
        if(methodOfGetPhoto == MethodOfGetPhoto.sms) { 
            photoReq.AddField("X-API-KEY", "k8088ss448os4c4ocwc400wokkwo8kg4s0w4s8k4");
            string mobile = "";
            if (phoneNoVK.textInput.text.Length > 7)
                mobile += "852" + phoneNoVK.textInput.text;
            mobile += ",";
            if (phoneNoVK2.textInput.text.Length > 7)
                mobile += "852" + phoneNoVK2.textInput.text;

            photoReq.AddField("mobile", mobile);
            photoReq.AddBinaryData("image", RenderPhoto22.Instance.PrintPhoto.EncodeToJPG());
        }
        else
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            long currentTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;
            byte[] photoEncoded = finalImage.EncodeToJPG();
            photoReq.AddBinaryData(UploadPhotoConfig.PHOTO_UPLOAD_BODY, photoEncoded, currentTime + ".jpg", "image/jpeg");
        }

        yield return 0;
        //string url = "https://www.lancomewomensday.com/api/photos";

        string url = UploadPhotoConfig.PHOTO_UPLOAD_URL;
        using (UnityWebRequest w = UnityWebRequest.Post(url, photoReq))
        {
            w.SetRequestHeader(UploadPhotoConfig.PHOTO_UPLOAD_HEADER, UploadPhotoConfig.PHOTO_UPLOAD_HEADER_KEY);
            w.downloadHandler = new DownloadHandlerBuffer();

            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.Log(w.error);
                yield return new WaitForSeconds(5);
                StartCoroutine(waitForAPI(finalImage));
            }
            else
            {
                Debug.Log("Uploaded:"+w.downloadHandler.text);
                //getPhotoRequest req = JsonUtility.FromJson<getPhotoRequest> (w.downloadHandler.text);
                JsonData downloadedFile = JsonMapper.ToObject(w.downloadHandler.text);
                Debug.Log("result url:" + downloadedFile["image"]);
                currentState = statFinalPreviewPage;
                //onGetPhotoFinished();

                string imagePath = cfgFile.altImgPath + "/Image";
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                if (Directory.Exists(imagePath))
                    File.WriteAllBytes(Path.Combine(imagePath, DateTime.Now.ToString("ddmmyyhhmmss")) + ".jpg", RenderPhoto22.Instance.PrintPhoto.EncodeToJPG());

                SaveQRCodeToLocal((string)downloadedFile["image"]);
            }
        }  
    }

    private void SaveQRCodeToLocal(string webQR_URL = "")
    {
        string qrImagePath = cfgFile.altImgPath + "/qr";
        if(!Directory.Exists(qrImagePath))
            Directory.CreateDirectory(qrImagePath);

        // Loacl storage qr image
        if (Directory.Exists(qrImagePath))
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            long currentTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;
            if (webQR_URL != "") { 
                File.WriteAllBytes(Path.Combine(qrImagePath, DateTime.Now.ToString("ddmmyyhhmmss")) + ".jpg", QrCode(webQR_URL).EncodeToJPG());
                if (qrImage != null && qrPrepareUpload != null)
                {
                    qrImage.enabled = true;
                    qrPrepareUpload.enabled = false;
                    qrImage.sprite = QrSprite;
                }
            }
        }
    }

    private Texture2D  qrCode = null;
    public Texture2D QrCode(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        qrCode = encoded;
        return qrCode;
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    private Sprite QrSprite
    {
        get
        {
            return Texture2DToSprite(this.qrCode);
        }
    }

    private Sprite Texture2DToSprite(Texture2D image)
    {
        if (image != null)
            return Sprite.Create(image, ImageFormat(image), new Vector2(0.5f, 0.5f));
        else
            return null;
    }
    private Rect ImageFormat(Texture2D tex)
    {
        return new Rect(0.0f, 0.0f, tex.width, tex.height);
    }

    bool waitingAPI = false;
    float endTimer =0;
    void stateGetPhoto()
    {
        endTimer += Time.deltaTime;
        if (isInput())
        {
            SFX.PlayOneShot(SFX.clip);
            getPhotoPage.SetActive(false);
            currentState = stateIdle;
            Startpage.SetActive(true);
        }
        if(cfgFile.thanksPageLength < endTimer)
        {
            getPhotoPage.SetActive(false);
            currentState = stateIdle;
            Startpage.SetActive(true);
        }
    }

  //for config:
  //end page wait for sec
  //image path
  
    void initConfig() 
    {
        altImgPathBuf = cfgFile.altImgPath;
        //Debug.Log(altImgPathBuf);
        thanksPageLengthBuf = cfgFile.thanksPageLength.ToString();
        copiesBuf = cfgFile.copies.ToString();
    }

    void applyConfig()
    {
        if(System.IO.Directory.Exists(altImgPathBuf))
            cfgFile.altImgPath = altImgPathBuf;
        float tmp;
        if(float.TryParse(thanksPageLengthBuf,out tmp))
        {
            cfgFile.thanksPageLength = tmp;
        }
        int nums=1;
        if(int.TryParse(copiesBuf,out nums))
        {
            //PrinterController.Instance.copies = nums;
        }
    }
    void saveCfg()
    {
        System.IO.File.WriteAllText(Application.persistentDataPath+"/config.json",JsonUtility.ToJson(cfgFile));
        
    }
    string thanksPageLengthBuf;
    string altImgPathBuf;
    string copiesBuf;

    bool showSetting = false;
    private void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 24;
        labelStyle.normal.textColor = Color.white;
        if (showSetting)
        {
            GUI.Box(new Rect(10, 0, 400, 600), "Setting:");
            GUI.Label(new Rect(15, 20, 60, 25), "Thanks Page Leng(sec):", labelStyle);
            thanksPageLengthBuf = GUI.TextField(new Rect(345, 25, 40, 25), thanksPageLengthBuf);
            GUI.Label(new Rect(15, 55, 60, 25), "Alternative image saving path:", labelStyle);
            altImgPathBuf = GUI.TextField(new Rect(15, 85, 350, 25), altImgPathBuf);
            GUI.Label(new Rect(15, 120, 60, 25), "Print copy:", labelStyle);
            copiesBuf = GUI.TextField(new Rect(185, 125, 50, 25), copiesBuf);
            if (GUI.Button(new Rect(15, 180, 350, 25),"Apply")){
                applyConfig();
                initConfig();
                saveCfg();
            }
            
        }
    }
    void Update () {
        updateBodies();
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            singlePlay = !singlePlay;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            showSetting = !showSetting;
            Cursor.visible = showSetting;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            currentState = stateIdle;
            Startpage.SetActive(true);

            Act1Page.SetActive(false);
            Act2Page.SetActive(false);
            Act3Page.SetActive(false);
            FinalPreviewPage.SetActive(false);
            getPhotoPage.SetActive(false);
            StopAllCoroutines();
        }
        currentState.Invoke();
	}
}

public class UploadPhotoConfig
{
    public static string PHOTO_UPLOAD_URL = "https://php8.mginteractive.hk/enzo/photo/api/photos";
    public static string PHOTO_UPLOAD_HEADER = "x-api-key";
    public static string PHOTO_UPLOAD_HEADER_KEY = "~}?/6GpsiN1&GE*t=|m8G7<g:?vg^<Jx2H<E3lnt";
    public static string PHOTO_UPLOAD_BODY = "file";
    public delegate void NetWorkUploadCallback();
}

