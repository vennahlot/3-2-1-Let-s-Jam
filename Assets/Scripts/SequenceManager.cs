using UnityEngine;
using System;
using System.Collections;

public class SequenceManager : MonoBehaviour
{
    [Header("Timeline Settings")]
    [SerializeField] private float bpm = 96.0f;
    public int prepareBars = 1;
    public int totalBars = 1;
    public int beatsPerBar = 4;
    public int stepsPerBeat = 4;

    // ----- COMPUTED PROPERTIES -----
    public int PrepareBeats => prepareBars * beatsPerBar;
    public int TotalSteps => totalBars * beatsPerBar * stepsPerBeat;
    public double BeatInterval => 60.0 / bpm;
    public double StepInterval => 60.0 / bpm / stepsPerBeat;

    // ----- EVENT -----
    public static event Action<int> OnPrepareBeat;
    public static event Action OnPrepareStarted;
    public static event Action<int, bool> OnSequenceStep;
    public static event Action OnSequenceStopped;
    public static event Action OnSequenceStarted;

    // ----- STATES -----
    public bool IsPlaying { get; private set; } = false;
    private int lastStep = -1;
    private double startTime;
    
    // ----- Singletons -----
    public static SequenceManager Instance { get; private set; }

    // ----- LIFE CYCLE -----

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        double currentTime = AudioSettings.dspTime - startTime;
        int currentStep = (int)(currentTime / StepInterval);

        // Use while to avoid low frame rate skip steps.
        while (currentStep > lastStep)
        {
            lastStep++;
            if (lastStep >= TotalSteps)
            {
                Stop();
                Debug.Log("Timeline finished.");
                return;
            }
            OnSequenceStep?.Invoke(currentStep, lastStep % stepsPerBeat == 0);
        }
    }
    
    // ----- METHODS -----

    public void Play()
    {
        OnPrepareStarted?.Invoke();
        Debug.Log("Prepare started.");
        StartCoroutine(PreparePlay());
    }

    private IEnumerator PreparePlay()
    {
        for (int i = 0; i < PrepareBeats; i++)
        {
            OnPrepareBeat?.Invoke(i);
            yield return new WaitForSeconds((float)BeatInterval);
        }
        StartPlay();
    }
    
    private void StartPlay()
    {
        IsPlaying = true;
        lastStep = -1;
        startTime = AudioSettings.dspTime;
        OnSequenceStarted?.Invoke();
        Debug.Log("Sequence started.");
    }

    public void Stop()
    {
        IsPlaying = false;
        lastStep = -1;
        OnSequenceStopped?.Invoke();
        Debug.Log("Sequence stopped.");
    }

}
