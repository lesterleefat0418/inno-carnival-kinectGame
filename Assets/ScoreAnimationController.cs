using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAnimationController : MonoBehaviour
{
    public Sprite[] numbers;
    public SpriteRenderer scoreSpriteRenderer;
    private int _score;
    public int score {
        get {
            return _score;
        }

        set {
            _score = value;
            UpdateUI();
        }
    }

    public void UpdateUI() {
        scoreSpriteRenderer.sprite = numbers[_score];
    }

    public void OnAnimationComplete() {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
