using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource _MusicAudio;
    AudioSource _SoundAudio;
    private float SoundVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        }
        set
        {
            _SoundAudio.volume = value;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }
    private float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        }
        set
        {
            _MusicAudio.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    private void Awake()
    {
        _MusicAudio = this.AddComponent<AudioSource>();
        _MusicAudio.playOnAwake = false;
        _MusicAudio.loop = true;

        _SoundAudio = this.AddComponent<AudioSource>();
        _SoundAudio.loop = false;
    }
    public void PlayMusic(string name)
    {
        if (this.MusicVolume < 0.1f)
        {
            return;
        }

        string oldName = "";
        if(_MusicAudio.clip != null)
        {
            oldName = _MusicAudio.clip.name;
        }
        if (oldName == name)
        {
            _MusicAudio.Play();
            return;
        }

        Manager.Resource.LoadMusic(name, (Object obj) =>
        {
            _MusicAudio.clip = obj as AudioClip;
            _MusicAudio.Play();
        });
    }
    public void PauseMusic()
    {
        _MusicAudio.Pause();
    }
    public void UnPauseMusic()
    {
        _MusicAudio.UnPause();
    }
    public void StopMusic()
    {
        _MusicAudio.Stop();
    }

    public void PlaySound(string name)
    {
        if (this.SoundVolume < 0.1f) return;

        Manager.Resource.LoadSound(name, (Object obj) =>
        {
            _SoundAudio.PlayOneShot(obj as AudioClip);
        });

    }
    public void SetMusicVolume(float value)
    {
        this.MusicVolume = value;
    }
    public void SetSoundVolume(float value)
    {
        this.SoundVolume = value;
    }
}
