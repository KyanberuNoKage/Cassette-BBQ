using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource _music_AudioSource;
    [SerializeField] AudioSource _soundEffects_AudioSource;

    [Space, Header("Music")] 
    [SerializeField] AudioClip[] _music_List;
}
