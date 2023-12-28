
using UnityEngine;

public class ResultPage : UIPage
{

    private void OnEnable()
    {
        if (GameLoader.Instance != null && AudioManager.Instance != null)
        {

            Debug.Log("marks: " + GameLoader.Instance.resultBoard.score);
            if (GameLoader.Instance.resultBoard.score >= GameLoader.Instance.successMark)
            {
                this.setPage(0);
                AudioManager.Instance.PlayResultPage();
            }
            else
            {
                this.setPage(1);
                AudioManager.Instance.PlayFailurePage();
            }
        }
    }

}

