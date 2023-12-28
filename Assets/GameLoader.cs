using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum Page { 
    StartPage,
    InstructionPage,
    GameStartPage,
    GamePage,
    ResultPage
}

public class GameLoader : MonoBehaviour
{
    public static GameLoader Instance = null; 
    public Page curPage = Page.StartPage;
    public GameObject[] gamePages;
    public GameObject character;
    public GameObject characterPositionControlObject;

    public TimerController gameTimer;
    public ScoreBoardController scoreBoard;
    public ScoreBoardController resultBoard;
    public TimerController resultTimer;
    public GameObject btnHome;
    public InstructionPage instruction;

    public ObjectEmitter objEmitter;
    public bool mapKinectToCharacter = false;
    public float kinectMovementMultiplier = 1.0f;
    public int gameTime = 55;
    public int successMark = 60;
    public CanvasGroup TimeUP, LogoCanvas;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ConfigPage.Instance == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        else
        {
            this.gameTime = ConfigPage.Instance.configData.gameTime;
            this.successMark = ConfigPage.Instance.configData.successMarkLevel;
        }

        GoToPage(Page.StartPage);
    }

    // Update is called once per frame
    void Update()
    {
        if (mapKinectToCharacter) {
            if (bodyGestureManager.Instance.numberOfBody() > 0) {
                int closestBodyId = bodyGestureManager.Instance.GetClosestBody();
                Vector3 spineShouldPos = bodyGestureManager.Instance.getSpineShoulderPos(closestBodyId);
                Vector3 newPos = new Vector3(spineShouldPos.x * kinectMovementMultiplier, 0f, 0f);

                characterPositionControlObject.transform.position = newPos;
            }
        }

        if (curPage == Page.ResultPage ||
            curPage == Page.StartPage) 
        {
            if (Input.GetKeyDown(KeyCode.PageDown) ||
                Input.GetKeyDown(KeyCode.DownArrow)) {
                Debug.Log("Next Page");
                this.nextPage();
            }
        }
        else if (curPage == Page.InstructionPage)
        {
            if(instruction != null && instruction.gameObject.activeInHierarchy) { 

                if (Input.GetKeyDown(KeyCode.PageDown) ||
                    Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if(instruction.curPageId > 0){
                        instruction.startGame();
                    }
                    else { 
                        instruction.nextInstructionPage();
                    }
                }
            }
        }

        /*var allKeys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();
        foreach (var key in allKeys)
        {
            if (Input.GetKeyDown(key)) { Debug.Log(key + " was pressed."); }
        }*/

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
            Debug.Log("Replay");
        }


    }

    public void score(int point) {

        if(scoreBoard.score == 0 && point < 0)
            scoreBoard.score = 0;
        else
            scoreBoard.score += point;
    }

    private int pageIndex(Page p) {
        Page[] pages = (Page[])Enum.GetValues(typeof(Page));
        int currentIndex = Array.IndexOf(pages, p);
        return currentIndex;
    }

    public void nextPage() {
        Page[] pages = (Page[])Enum.GetValues(typeof(Page));
        int currentIndex = pageIndex(curPage);
        Page nextPage = pages[(currentIndex + 1) % pages.Length];
        GoToPage(nextPage);
    }

    public void GoToPage(Page toPage) {


        for(int i=0; i< gamePages.Length; i++)
        {
            if(i == pageIndex(toPage))
            {
                curPage = toPage;
                gamePages[i].SetActive(true);
            }
            else
            {
                gamePages[i].SetActive(false);
            }
        }


        //view will appear
        switch (toPage) {
            case Page.StartPage:
                if (character.GetComponent<CharacterController>() != null)
                    character.GetComponent<CharacterController>().characterSprite.enabled = false;

                if (TimeUP != null) TimeUP.DOFade(0f, 0f);
                if (objEmitter != null) objEmitter.isEmitting = false;
                break;
            case Page.InstructionPage:
                break;
            case Page.GameStartPage:
                if (LogoCanvas != null) LogoCanvas.DOFade(1f, 0f);
                StartCoroutine(GameStart());
                break;
            case Page.GamePage:
                StartCoroutine(Game());
                break;
            case Page.ResultPage:
                if (TimeUP != null) TimeUP.DOFade(0f, 0f);
                if (LogoCanvas != null) LogoCanvas.DOFade(0f, 0f);
                //StartCoroutine(Result());
                break;
        }
    }

    IEnumerator GameStart() {

        if (character.GetComponent<CharacterController>() != null)
            character.GetComponent<CharacterController>().characterSprite.enabled = true;

        Animator character_Animator = character.GetComponent<Animator>();
        //m_Animator.ResetTrigger("Crouch");

        if(character_Animator != null)
            character_Animator.SetTrigger("GameStart");

        characterPositionControlObject.transform.position = Vector3.zero;
        this.mapKinectToCharacter = true;

        yield return new WaitForSeconds(2f);

        Animator gameStartText_Animator = gamePages[pageIndex(Page.GameStartPage)].GetComponent<Animator>();
        //m_Animator.ResetTrigger("Crouch");
        if(gameStartText_Animator != null)
            gameStartText_Animator.SetTrigger("GameStart");

        gameTimer.countTime = gameTime;
        gameTimer.ResetCount();
        scoreBoard.Reset();
        
        Invoke("nextPage", 3f);
    }

    IEnumerator Game() {
        yield return new WaitForSeconds(1f);
        if(character.GetComponent<SuperMOMController>() != null)
            character.GetComponent<SuperMOMController>().isPlaying = true;

        if (character.GetComponent<CharacterController>() != null)
            character.GetComponent<CharacterController>().isPlaying = true;

        if(objEmitter != null) objEmitter.isEmitting = true;
        gameTimer.StartCount();
        scoreBoard.score = 0;

        yield return new WaitForSeconds(gameTimer.countTime);
        if(objEmitter != null) objEmitter.isEmitting = false;

        if (character.GetComponent<SuperMOMController>() != null)
            character.GetComponent<SuperMOMController>().isPlaying = false;

        if (character.GetComponent<CharacterController>() != null)
            character.GetComponent<CharacterController>().isPlaying = false;

        Animator character_Animator = character.GetComponent<Animator>();
        //m_Animator.ResetTrigger("Crouch");
        if(character_Animator != null)
            character_Animator.SetTrigger("GameEnd");
        this.mapKinectToCharacter = false;
        if (TimeUP != null) TimeUP.DOFade(1.0f, 0.5f);
        resultBoard.score = scoreBoard.score;
        mapKinectToCharacter = false;
        Invoke("nextPage", 2f);
    }

    IEnumerator Result() {
        yield return null;
        AudioManager.Instance.PlayResultPage();
        /*
        btnHome.SetActive(false);
        resultTimer.countTime = 8;
        resultTimer.ResetCount();
        resultTimer.StartCount();
        */
    }

    public void OnResultPageCountDownCompleted() {
        //btnHome.SetActive(true);
    }
}


