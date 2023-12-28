using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyButtonBinder : MonoBehaviour
{
    public Button button;
    public KeyCode key = KeyCode.None;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (key != KeyCode.None && button != null && Input.GetKeyDown(key)) {
            button.onClick.Invoke();
        }
    }
}
