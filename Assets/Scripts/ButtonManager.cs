using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject stopButton;

    void Awake()
    {
        SequenceManager.OnSequenceStarted += ShowStopButton;
        SequenceManager.OnSequenceStopped += ShowPlayButton;
    }
    
    private void ShowPlayButton()
    {
        playButton.SetActive(true);
        stopButton.SetActive(false);
    }

    private void ShowStopButton()
    {
        playButton.SetActive(false);
        stopButton.SetActive(true);
    }

}
