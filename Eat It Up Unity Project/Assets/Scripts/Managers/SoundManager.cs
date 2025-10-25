using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioSource ambientSource;
    [SerializeField]
    private AudioSource sfxAudioSource;
    [SerializeField]
    private List<AudioClip> ambientSounds;

    [SerializeField]
    private List<SoundFXClass> soundsFX = new List<SoundFXClass>();

    private static string Master = "masterVolume";
    private static string Music = "musicVolume";
    private static string SoundFX = "SFXVolume";

    private bool musicEnabled = true;
    private bool soundFXEnabled = true;

    public bool MusicEnabled { get { return musicEnabled; } }
    public bool SoundFXEnabled { get { return soundFXEnabled; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        CheckForVolumesBool();
    }

    [System.Serializable]
    public class SoundFXClass
    {
        [SerializeField]
        public SoundFXType soundFXType;

        [SerializeField]
        public AudioClip sfxToPlay;
    }

    [System.Serializable]
    public enum SoundFXType
    {
        Spawn,
        Jump,
        DoubleJump,
        Collectable,
        Death,
        Door
    }

    void Start()
    {
        PlayAmbientMusic();
    }

    public void SetMasterVolume(float newVolume)
    {
        audioMixer.SetFloat(Master, Mathf.Log10(newVolume) * 20f);
    }

    public void SetMusicVolume(float newVolume)
    {
        audioMixer.SetFloat(Music, Mathf.Log10(newVolume) * 20f);
    }

    public void SetSFXVolume(float newVolume)
    {
        audioMixer.SetFloat(SoundFX, Mathf.Log10(newVolume) * 20f);
    }

    public void ToggleMusicVolume(bool enabled)
    {
        musicEnabled = enabled;
        if (enabled)
            SetMusicVolume(.09f);
        else
            SetMusicVolume(0.0001f);
    }

    public void ToggleSFXVolume(bool enabled)
    {
        soundFXEnabled = enabled;
        if (enabled)
            SetSFXVolume(.3f);
        else
            SetSFXVolume(0.0001f);
    }
    public void PlayAmbientMusic()
    {
        ambientSource.clip = ambientSounds[Random.Range(0, ambientSounds.Count)];
        ambientSource.Play();
    }

    public void PlaySFX(SoundFXType sfxType)
    {
        AudioClip soundToSpawn = null;
        foreach (SoundFXClass soundClass in soundsFX)
        {
            if (soundClass.soundFXType == sfxType)
            {
                soundToSpawn = soundClass.sfxToPlay;
                break;
            }
        }
        AudioSource newSFX = Instantiate(sfxAudioSource, transform);
        newSFX.clip = soundToSpawn;
        newSFX.Play();
        Destroy(newSFX, newSFX.clip.length);
    }

    [ContextMenu("TestCheckVolume")]
    public void CheckForVolumesBool()
    {
        float volume;

        audioMixer.GetFloat(Music, out volume);
        if (volume <= -70f)
            musicEnabled = false;
        else
            musicEnabled = true;

        audioMixer.GetFloat(SoundFX, out volume);
        if (volume <= -70f)
            soundFXEnabled = false;
        else
            soundFXEnabled = true;
    }
}
