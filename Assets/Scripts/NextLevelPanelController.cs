using System.Collections.Generic;
using UnityEngine;

public class NextLevelPanelController : MonoBehaviour
{
    [SerializeField] private List<TrackController> trackControllers;

    void Awake()
    {
        gameObject.SetActive(false);
        SequenceManager.OnSequenceStopped += CheckAllTracksPassed;
    }

    private void CheckAllTracksPassed()
    {
        foreach (TrackController trackController in trackControllers)
        {
            if (!trackController.CheckAllPassed())
            {
                return;
            }
        }
        gameObject.SetActive(true);
    }
}
