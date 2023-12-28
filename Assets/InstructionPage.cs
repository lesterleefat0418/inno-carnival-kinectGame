using UnityEngine.UI;

public class InstructionPage : UIPage
{
    public Text timer_ch, timer_eng;

    private void OnEnable()
    {
        if(timer_ch != null)
            timer_ch.text = ConfigPage.Instance.configData.gameTime.ToString();

        if (timer_eng != null)
            timer_eng.text = ConfigPage.Instance.configData.gameTime.ToString();

        this.setPage(0);
    }

    private void OnDisable()
    {
        //this.setPage(0);
    }

    public void nextInstructionPage()
    {
        if(AudioManager.Instance != null) { 
            this.setPage(1);
            AudioManager.Instance.PlayButtonPressed();
        }
    }

    public void startGame()
    {
        if(GameLoader.Instance != null && !this.isAnimated) { 
            GameLoader.Instance.nextPage();
            AudioManager.Instance.PlayButtonPressed();
        }
    }
}
