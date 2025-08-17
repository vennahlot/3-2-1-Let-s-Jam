using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    [SerializeField] List<AudioClip> sounds;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        SequenceManager.OnPrepareBeat += PlayMetronome;
    }

    void OnDisable()
    {
        SequenceManager.OnPrepareBeat -= PlayMetronome;
    }

    void PlayMetronome(int beat)
    {
        audioSource.PlayOneShot(sounds[0]);
    }
}
