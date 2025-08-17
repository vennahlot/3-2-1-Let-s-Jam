using System;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private TrackController targetTrack;
    private int currentStep;
    AudioSource audioSource;

    // ----- Unity Life Cycle -----

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        SequenceManager.OnSequenceStep += SyncCurrentStep;
    }

    void OnDisable()
    {
        SequenceManager.OnSequenceStep -= SyncCurrentStep;
    }

    // ----- Event Handlers -----

    void SyncCurrentStep(int step, bool isBeat)
    {
        currentStep = step;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        audioSource.PlayOneShot(audioSource.clip);
        targetTrack.RecordSoundEmission(currentStep);
    }
}
