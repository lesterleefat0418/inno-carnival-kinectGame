using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    public AudioSource buttonPressed;
    public AudioSource resultPage;
    public AudioSource failureResult;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void PlayButtonPressed()
    {
        if (buttonPressed.isPlaying) {
            buttonPressed.Stop();
        }
        buttonPressed.Play();
    }

    public void PlayResultPage()
    {
        if (resultPage.isPlaying)
        {
            resultPage.Stop();
        }
        resultPage.Play();
    }


    public void PlayFailurePage()
    {
        if(failureResult != null) { 
            if (failureResult.isPlaying)
            {
                failureResult.Stop();
            }
            failureResult.Play();
        }
    }
}
