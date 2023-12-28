using System;
using UnityEngine;
using DG.Tweening;

public class ScoreController : MonoBehaviour
{
    public Sprite[] numbers;
    public Sprite[] type;
    public SpriteRenderer scoreBg, labor, scoreSpriteRenderer;
    private int _score;
    public Color trueColor, wrongColor;

    public float liveTime = 1f;

    float originalScale;

    public int score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
            UpdateUI(_score);
        }
    }

    public void UpdateUI(int mark)
    {
       // Debug.Log("mark: " + mark);
        if(scoreBg != null && labor != null) { 
            if(mark > 0)
            {
                scoreBg.color = trueColor;
                labor.sprite = type[1];
            }
            else
            {
                scoreBg.color = wrongColor;
                labor.sprite = type[0];
            }
        }

        scoreSpriteRenderer.sprite = numbers[Math.Abs(_score)];
    }


    void scaleDown()
    {
        if (this.gameObject != null && this.gameObject.transform != null && this.gameObject.transform.localScale.x == originalScale)
            this.gameObject.transform.DOScale(Vector3.zero, 0.2f).SetAutoKill(true).OnComplete(despawn);
    }

    void despawn()
    {
        ObjectPoolManager.DespawnGameObject(this.gameObject);
        ObjectPoolManager.PermanentlyDestroyGameObjectsOfType(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        originalScale = this.gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject != null) { 
            if(liveTime > 0f)
            {
                liveTime -= Time.deltaTime;
            }
            else
            {
                scaleDown();
            }
        }
    }
}

