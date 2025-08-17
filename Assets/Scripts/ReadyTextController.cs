using TMPro;
using UnityEngine;

public class ReadyTextController : MonoBehaviour
{
    private TextMeshProUGUI text;

    private readonly string[] lines = { "3", "2", "1", "LET'S JAM!" };


    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false);
        SequenceManager.OnPrepareStarted += ShowReadyText;
        SequenceManager.OnSequenceStarted += HideReadyText;
    }

    void OnEnable()
    {
        SequenceManager.OnPrepareBeat += UpdateText;
    }

    void OnDisable()
    {
        SequenceManager.OnPrepareBeat -= UpdateText;
    }

    private void ShowReadyText()
    {
        gameObject.SetActive(true);
    }

    private void HideReadyText()
    {
        gameObject.SetActive(false);
    }

    private void UpdateText(int beat)
    {
        text.text = lines[beat];
    }
}
