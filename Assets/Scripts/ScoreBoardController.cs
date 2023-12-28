using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardController : MonoBehaviour
{
    public Sprite[] digits;
    public UnityEngine.UI.Image lastThirdDigitImage;
    public UnityEngine.UI.Image lastSecondDigitImage;
    public UnityEngine.UI.Image lastDigitImage;
    public UnityEngine.RectTransform textContainer;
    public Vector3 threeDigitPosition = new Vector3();
    public Vector3 threeDigitScale = new Vector3();
    public Vector3 twoDigitPosition = new Vector3();
    public Vector3 twoDigitScale = new Vector3();


    public int _score = 0;
    public int score
    {
        get {
            return _score;
        }

        set {
            _score = value;
            UpdateUI();
        }
    }

    public void Reset()
    {
        score = 0;
        lastThirdDigitImage.enabled = false;
        lastSecondDigitImage.enabled = false;
        lastDigitImage.enabled = true;
        UpdateUI();
    }

    private void UpdateUI() {

        _score = Mathf.Abs(_score);

        int lastDigit = _score % 10;
        int lastSecondDigit = (_score / 10) % 10;
        int lastThirdDigit = (_score / 100) % 10;

        //Debug.Log(textContainer.position);
        if (_score >= 100)
        {
            //set small scale
            textContainer.localPosition = threeDigitPosition;
            textContainer.localScale = threeDigitScale;
            lastThirdDigitImage.enabled = true;        
        }
        else {
            //set big scale
            textContainer.localPosition = twoDigitPosition;
            textContainer.localScale = twoDigitScale;
            lastThirdDigitImage.enabled = false;
        }

        if (_score >= 10)
        {
            lastSecondDigitImage.enabled = true;
        }
        else {
            lastSecondDigitImage.enabled = false;
        }
        lastThirdDigitImage.sprite = digits[lastThirdDigit];
        lastSecondDigitImage.sprite = digits[lastSecondDigit];
        lastDigitImage.sprite = digits[lastDigit];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
