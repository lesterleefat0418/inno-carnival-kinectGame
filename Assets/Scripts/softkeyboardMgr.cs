using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class softkeyboardMgr : MonoBehaviour {

    public TextMesh textInput;
    public UnityEngine.UI.Text DisplayText;
    public Camera UICam;
    bool init = true;
    int charLimit = 8;
    public AudioSource SFX;
    // Use this for initialization
    void Start () {
		
	}

    public void Reset()
    {
        textInput.text = "";
        DisplayText.text = textInput.text;
    }
    void OnEnable()
    {
        init = true;
        DisplayText.gameObject.SetActive(true);
        
    }
    private void OnDisable()
    {
        DisplayText.text = textInput.text;
    }
    float flickTimer =0;
    bool endMark = false;
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(UICam.ScreenPointToRay(Input.mousePosition),out hit))
            {
                softkeyBtn akey = hit.transform.GetComponent<softkeyBtn>();
                if (akey != null) 
                {
                    SFX.PlayOneShot(SFX.clip);
                    if (init)
                    {
                        textInput.text = "";
                        init = false;
                    }
                    if(akey.keyValue == "space")
                    {
                        if(charLimit > textInput.text.Length)
                            textInput.text+=" ";
                    }
                    else if (akey.keyValue == "backspace")
                    {
                        if(textInput.text.Length > 1)
                        {
                            textInput.text = textInput.text.Substring(0, textInput.text.Length - 1);
                        }
                        else
                        {
                            textInput.text = "";
                        }
                            
                    }
                    else
                    {
                        if (charLimit > textInput.text.Length)
                            textInput.text += akey.keyValue;
                    }
                }
            }
        }
        if (DisplayText)
        {
            DisplayText.text = textInput.text + (endMark ? " " : "|");
            flickTimer += Time.deltaTime;
            if (flickTimer > 1)
            {
                flickTimer = 0;
                endMark = !endMark;
            }
        }
    }
}
