using UnityEngine;
using UnityEngine.UI;

public class StepPanelController : MonoBehaviour
{
    public bool expected = false;
    public bool success = false;

    [SerializeField] private GameObject subIcon;
    private Image panelImage;

    void Awake()
    {
        panelImage = GetComponent<Image>();
        SequenceManager.OnSequenceStarted += ResetPanel;
    }

    public void InitPanel(bool expected)
    {
        this.expected = expected;
        if (this.expected)
        {
            subIcon.SetActive(true);
        }
        else
        {
            subIcon.SetActive(false);
        }
        panelImage.color = Color.gray;
    }
    
    public void ResetPanel()
    {
        panelImage.color = Color.gray;
        success = false;
    }

    public void UpdatePanel(bool actual)
    {
        if (actual == expected)
        {
            panelImage.color = Color.green;
            success = true;
        }
        else
        {
            panelImage.color = Color.red;
            success = false;
        }
    }
}
