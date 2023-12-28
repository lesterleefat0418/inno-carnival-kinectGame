using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class ScreenController : MonoBehaviour
{
    public Vector2Int resolution;
    public bool mouseStatus = true;
    public bool enableFocusWindow = true;
    public FocusWindow focusWindow;
    // Start is called before the first frame update

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Screen.SetResolution(this.resolution.x, this.resolution.y, true);
        Cursor.visible = this.mouseStatus;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            mouseStatus = !this.mouseStatus;
            Cursor.visible = mouseStatus;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            this.enableFocusWindow = !this.enableFocusWindow;
            focusWindow.isOn = this.enableFocusWindow;
        }
    }
}