using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffect_AudioSource_Pool : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _grillMixerGroup;
    [SerializeField] private AudioClip[] _grillSizzles;
    private List<AudioSource> _audioSourcePool = new List<AudioSource>();
    private int _poolSize = 10;

    void Start()
    {
        // Create a reusable pool of AudioSources.
        for (int i = 0; i < _poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _grillMixerGroup;
            _audioSourcePool.Add(source);
        }
    }
}
