using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameStartController : MonoBehaviour
{

    public GameObject[] counts;
    public Animator StartGameAnimation;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(startCountDown());
    }

    void OnDisable()
    {
        if (StartGameAnimation != null) StartGameAnimation.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator startCountDown()
    {

        if (counts[0] != null) counts[0].SetActive(true);
        yield return new WaitForSeconds(1f);
        if (counts[0] != null) counts[0].SetActive(false);
        if (counts[1] != null) counts[1].SetActive(true);
        yield return new WaitForSeconds(1f);
        if (counts[1] != null) counts[1].SetActive(false);
        if (counts[2] != null) counts[2].SetActive(true);
        yield return new WaitForSeconds(1f);
        if (counts[2] != null) counts[2].SetActive(false);
        if (counts[3] != null) counts[3].SetActive(true);
        if (StartGameAnimation != null) StartGameAnimation.enabled = true;

        
    }

}
