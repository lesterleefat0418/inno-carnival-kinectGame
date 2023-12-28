using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TimerController : MonoBehaviour
{
    public Sprite[] digits;
    public UnityEngine.UI.Image tenDigitImage;
    public UnityEngine.UI.Image oneDigitImage;
    private int _countTime;
    public int countTime {
        get {
            return _countTime;
        }

        set {
            _countTime = value;
        }
    }
    public float timer = 0f;
    public bool isCounting = false;

    public UnityEvent onTimesUpCallback;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void UpdateUI(int time) {
        int lastDigit = time % 10;
        int lastSecondDigit = (time / 10) % 10;
        tenDigitImage.sprite = digits[lastSecondDigit];
        oneDigitImage.sprite = digits[lastDigit];
    }

    public void ResetCount() {
        timer = countTime;
        UpdateUI(countTime);
    }

    public void StartCount() {
        isCounting = true;
    }

    public void PauseCount() {
        isCounting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCounting) {
            timer -= Time.deltaTime;
            UpdateUI(Mathf.CeilToInt(timer));
        }
        if (timer <= 0) {
            isCounting = false;
            if (onTimesUpCallback != null) {
                onTimesUpCallback.Invoke();
            }
        }
    }


}
