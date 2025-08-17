using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour
{
    [SerializeField] private List<bool> trackData;
    [SerializeField] private GameObject stepPanelPrefab;
    private readonly List<GameObject> stepPanels = new();

    // ----- Unity Lifecycle -----
    void Awake()
    {
        SequenceManager.OnSequenceStep += SyncCurrentStep;
        ConstructTrack();
    }

    // ----- Methods -----
    public void ConstructTrack()
    {
        foreach (bool stepExpected in trackData)
        {
            GameObject stepPanel = Instantiate(stepPanelPrefab);
            stepPanel.transform.SetParent(transform);
            StepPanelController stepPanelController = stepPanel.GetComponent<StepPanelController>();
            stepPanelController.InitPanel(stepExpected);
            stepPanels.Add(stepPanel);
        }
    }

    private void SyncCurrentStep(int step, bool isBeat)
    {
        StepPanelController stepPanel = stepPanels[step].GetComponent<StepPanelController>();
        stepPanel.UpdatePanel(false);  // Assume no sound at very beginning.
    }

    public void RecordSoundEmission(int step)
    {
        StepPanelController stepPanel = stepPanels[step].GetComponent<StepPanelController>();
        stepPanel.UpdatePanel(true);
    }

    public bool CheckAllPassed()
    {
        foreach (GameObject stepPanel in stepPanels)
        {
            StepPanelController stepPanelController = stepPanel.GetComponent<StepPanelController>();
            if (!stepPanelController.success)
            {
                return false;
            }
        }
        return true;
    }
}
