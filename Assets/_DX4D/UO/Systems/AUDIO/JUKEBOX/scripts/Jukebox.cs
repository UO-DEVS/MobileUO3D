//BY DX4D
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    /// <summary>Used to adjust 0-100 based volume scales to 0-1 based volume scale. DEFAULT: 0.0001f</summary>
    const float VOLUME_SCALER = 0.0001f;
    const int MIN_VOLUME = 0;
    const int MAX_VOLUME = 100;

    [Header("COMPONENT LINKS")]
    [SerializeField] Camera _camera;

    //MASTER VOLUME
    [Header("MASTER VOLUME")]
    [SerializeField] [Range(MIN_VOLUME, MAX_VOLUME)] int _masterVolume = 100;
    int masterVolume => _masterVolume;
    //internal void RefreshAudio() => RefreshMasterVolume();
    void RefreshMasterVolume()
    {
        RefreshMusicVolume(); //MUSIC
        RefreshSFXVolume(); //SFX
        RefreshVoiceVolume(); //VOICE
    }
    //MUSIC
    [Header("MUSIC CHANNEL")]
    [SerializeField] AudioSource _musicAudioSource;
    AudioSource musicPlayer => _musicAudioSource;
    [SerializeField] [Range(MIN_VOLUME, MAX_VOLUME)] int _musicVolume = 50;
    internal int musicVolume => _musicVolume;
    void RefreshMusicVolume() { musicPlayer.volume = musicVolume * masterVolume * VOLUME_SCALER; }
    //SFX
    [Header("SFX CHANNEL")]
    [SerializeField] AudioSource _soundEffectAudioSource;
    AudioSource sfxPlayer => _soundEffectAudioSource;
    [SerializeField] [Range(MIN_VOLUME, MAX_VOLUME)] int _sfxVolume = 80;
    internal int sfxVolume => _sfxVolume;
    void RefreshSFXVolume() { sfxPlayer.volume = sfxVolume * masterVolume * VOLUME_SCALER; }
    //VOICE
    [Header("VOICE CHANNEL")]
    [SerializeField] AudioSource _voiceAudioSource;
    AudioSource voicePlayer => _voiceAudioSource;
    [SerializeField] [Range(MIN_VOLUME, MAX_VOLUME)] int _voiceVolume = 80;
    internal int voiceVolume => _voiceVolume;
    void RefreshVoiceVolume() { voicePlayer.volume = voiceVolume * masterVolume * VOLUME_SCALER; }

    [Header("PLAYLIST")]
    [SerializeField] List<AudioClip> _playlist = new List<AudioClip>();
    [SerializeField] Queue<AudioClip> _audioQueue = new Queue<AudioClip>();

    [Header("NOW PLAYING")]
    [SerializeField] AudioClip _nextTrack;
    [SerializeField] AudioClip _lastPlayed;
    [SerializeField] AudioClip _nowPlaying;

    [Header("AUDIO SETTINGS")]
    [SerializeField] bool loop = false;
    [SerializeField] bool crossfadeTracks = false;
    [SerializeField] [Range(0.1f, 10f)] float crossfadeDelay = 3f;
    [Tooltip("How many times will volume reduce when crossfading?")]
    [SerializeField] [Range(1, 100)] int crossfadePasses = 10;

    [Header("AUDIO SOURCE")]
    [SerializeField] bool useCameraAudioSource = false;
    [SerializeField] bool useOwnAudioSource = true;

    //ON VALIDATE
    private void OnValidate()
    {
        RefreshMasterVolume(); //REFRESH AUDIO SOURCE VOLUME

        //if (useCameraAudioSource)
        //{
        //    if (!_camera) _camera = Camera.main;
        //    if (_camera && !_audioSource || !_audioSource.isActiveAndEnabled) _audioSource = _camera.GetComponent<AudioSource>();
        //    if (_camera && !_audioSource)
        //    {
        //        _audioSource = _camera.gameObject.AddComponent<AudioSource>();

        //#if UNITY_EDITOR && DEBUG
        //                Debug.LogWarning("No AudioSource component attached to " + _camera.name + "...creating it at runtime."
        //                    + "" + "Attach an AudioSource to " + _camera.name + " to ensure data integrity and improve performance.");
        //#endif
        //    }
        //}
        //else
        if (useOwnAudioSource && (!musicPlayer || !sfxPlayer || !voicePlayer))
        {
            AudioSource[] _audioSources = GetComponents<AudioSource>();

            if (_audioSources.Length < 3)
            {
                for (int i = 0; i < (3 - _audioSources.Length); i++)
                {
                    gameObject.AddComponent<AudioSource>(); //ADD COMPONENT
#if UNITY_EDITOR && DEBUG
                    Debug.LogWarning("No AudioSource component attached to " + name + "...creating it at runtime."
                        + "" + "Attach another AudioSource to " + name + ".");
#endif
                }
            }

            _audioSources = GetComponents<AudioSource>(); //FETCH AUDIO SOURCES

            //SET UP ORCHESTRA
            _musicAudioSource = _audioSources[0];
            _soundEffectAudioSource = _audioSources[1];
            _voiceAudioSource = _audioSources[2];
        }
    }

    //FIXED UPDATE
    private void FixedUpdate()
    {
        UpdateCurrentMusicTrack();
    }

    //VOLUME CONTROL
    //GET
    public int GetVolume(AudioChannel _channel)
    {
        switch (_channel)
        {
            case AudioChannel.Master:
                {
                    return masterVolume;
                }
            case AudioChannel.Music:
                {
                    return musicVolume;
                }
            case AudioChannel.SFX:
                {
                    return sfxVolume;
                }
            case AudioChannel.Voice:
                {
                    return voiceVolume;
                }
#if UNITY_EDITOR && DEBUG
            default:
                {
                    Debug.LogWarning("WARNING: INVALID AUDIO CHANNEL SPECIFIED FOR " + gameObject.name.ToUpper());
                    break;
                }
#endif
        }

        return 0;
    }
    //SET
    public void SetVolume(AudioChannel _channel, int _newVolume)
    {
        switch (_channel)
        {
            case AudioChannel.Master:
                {
                    _masterVolume = _newVolume;
                    RefreshMasterVolume();
                    break;
                }
            case AudioChannel.Music:
                {
                    _musicVolume = _newVolume;
                    RefreshMusicVolume();
                    break;
                }
            case AudioChannel.SFX:
                {
                    _sfxVolume = _newVolume;
                    RefreshSFXVolume();
                    break;
                }
            case AudioChannel.Voice:
                {
                    _voiceVolume = _newVolume;
                    RefreshVoiceVolume();
                    break;
                }
#if UNITY_EDITOR && DEBUG
            default:
                {
                    Debug.LogWarning("WARNING: INVALID AUDIO CHANNEL SPECIFIED FOR " + gameObject.name.ToUpper());
                    break;
                }
#endif
        }
    }

    //QUEUE UP MUSIC CLIP
    public void QueueUpMusic(AudioClip _clip, float _fadeDelay = 0f)
    {
        if (_fadeDelay > 0)
        {
            if (!crossfadeTracks) crossfadeTracks = true;
            crossfadeDelay = _fadeDelay;
        }

        _audioQueue.Enqueue(_clip);
    }
    //PLAY MUSIC CLIP
    public void PlayMusic(AudioClip _clip, float _fadeDelay = 0f)
    {
        if (_fadeDelay > 0)
        {
            if (!crossfadeTracks) crossfadeTracks = true;
            crossfadeDelay = _fadeDelay;
        }

        PlaySound(AudioChannel.Music, _clip); //PLAY MUSIC CLIP
    }
    //PLAY SFX CLIP
    public void PlaySoundEffect(AudioClip _clip)
    {
        PlaySound(AudioChannel.SFX, _clip); //PLAY MUSIC CLIP
    }
    //PLAY VOICE CLIP
    public void PlayVoice(AudioClip _clip)
    {
        PlaySound(AudioChannel.Voice, _clip); //PLAY MUSIC CLIP
    }


    //PLAY SOUND
    void PlaySound(AudioChannel _channel, AudioClip _clip)
    {
        switch (_channel)
        {
            case AudioChannel.Master:
                {
                    PlayNow(sfxPlayer, _clip); //PLAY ON SFX CHANNEL
                    break;
                }
            case AudioChannel.SFX:
                {
                    PlayNow(sfxPlayer, _clip); //PLAY SFX
                    break;
                }
            case AudioChannel.Voice:
                {
                    PlayNow(voicePlayer, _clip); //PLAY VOICE
                    break;
                }
            case AudioChannel.Music:
                {
                    PlayNow(musicPlayer, _clip); //PLAY MUSIC
                    break;
                }
#if UNITY_EDITOR && DEBUG
            default:
                {
                    Debug.LogWarning("WARNING: INVALID AUDIO CHANNEL SPECIFIED FOR " + gameObject.name.ToUpper());
                    break;
                }
#endif
        }
    }
    //PLAY AUDIO CLIP
    void PlayNow(AudioSource _audioPlayer, AudioClip _clip)
    {
        StartCoroutine(PlayClip(_audioPlayer, _clip)); //PLAY AUDIO CLIP
    }

    // A U D I O  C O N T R O L

    //PLAY
    void Play(AudioSource _audioPlayer)//AudioClip _clip)
    {
        if (crossfadeTracks) StartCoroutine(FadeInTrack());

        if (_audioPlayer.clip)
        {
#if UNITY_EDITOR && DEBUG
            Debug.Log("AUDIO MUSIC - " + "Playing track " + _audioPlayer.clip);
#endif
            _audioPlayer.Play();
        }


    }
    //PLAY MUSIC
    void PlayMusic()//AudioClip _clip)
    {
        if (crossfadeTracks) StartCoroutine(FadeInTrack());

        _lastPlayed = _nowPlaying;
        _nowPlaying = musicPlayer.clip;
        musicPlayer.Play();

#if UNITY_EDITOR && DEBUG
        Debug.Log("MUSIC - " + "Playing track " + musicPlayer.clip);
#endif
    }


    //STOP MUSIC
    void Stop(AudioSource _audioPlayer)
    {
        _audioPlayer.Stop();

#if UNITY_EDITOR && DEBUG
        Debug.Log("AUDIO - " + "Stopped audio player");
#endif
    }

    //STOP MUSIC
    void StopMusic()
    {
        //if (!_audioSource.isPlaying) return;

        if (crossfadeTracks) StartCoroutine(FadeOutTrack());
        else
        {
            if (loop) QueueUpMusic(musicPlayer.clip); //QUEUE UP AGAIN
            musicPlayer.Stop();

#if UNITY_EDITOR && DEBUG
            Debug.Log("AUDIO MUSIC - " + "Stopped audio without crossfade");
#endif
        }
    }

    //UPDATE CURRENT MUSIC TRACK
    void UpdateCurrentMusicTrack()
    {
        if (!musicPlayer.isPlaying)
        {
            //if (!musicPlayer.clip)
            LoadNextMusicTrack();
            if (musicPlayer.clip) PlayMusic();
        }
    }
    //LOAD NEXT TRACK
    void LoadNextMusicTrack()
    {
        musicPlayer.clip = GetNextMusicTrack();
    }
    //GET NEXT TRACK
    AudioClip GetNextMusicTrack()
    {
        if (_nextTrack)
        {
            AudioClip _clip = _nextTrack;
            _nextTrack = null;
            return _clip;
        }

        if (_playlist != null && _playlist.Count > 0)
        {
            AudioClip _clip = _playlist[0];
            if (loop) _audioQueue.Enqueue(_clip);
            _playlist.RemoveAt(0);
            return _clip;
        }

        if (_audioQueue.Count > 0)
        {
            AudioClip _clip = _audioQueue.Dequeue();
            if (loop) _audioQueue.Enqueue(_clip);
            return _clip;
        }
        else if (loop && _lastPlayed) return GetLastTrack();
        else return null;
    }
    AudioClip GetLastTrack()
    {
        return _lastPlayed;
    }
    /*
    //LAST
    void LastTrack()
    {
        StartCoroutine(PlayLastClip());
    }
    */
    // C O R O U T I N E S
    //PLAY CLIP
    IEnumerator PlayClip(AudioSource _audioPlayer, AudioClip _clip)
    {
        if (_clip)
        {

#if UNITY_EDITOR && DEBUG
            Debug.Log("AUDIO - " + "Loading track " + _clip.name + " in " + crossfadeDelay + " seconds");
#endif

            if (_audioPlayer == musicPlayer)
            {
                if (_clip != _nowPlaying)
                {
                    StopMusic();

                    yield return new WaitForSeconds(crossfadeDelay);

                    musicPlayer.clip = _clip;
                    Play(musicPlayer); //PLAY
                }
#if UNITY_EDITOR && DEBUG
                else { Debug.Log("AUDIO - " + "Audio clip " + _clip.name + " was already playing"); }
#endif
            }
            else
            {
                _audioPlayer.clip = _clip;
                Play(_audioPlayer); //PLAY
            }
        }
#if UNITY_EDITOR && DEBUG
        else { Debug.Log("AUDIO - " + "Could not play audio clip"); }
#endif
    }
    /*
    IEnumerator PlayNextClip()
    {
        Stop();

        yield return new WaitForSeconds(crossfadeDelay);

        Play(GetNextTrack());
    }

    IEnumerator PlayLastClip()
    {
#if UNITY_EDITOR && DEBUG
        Debug.Log("AUDIO - " + "Playing last track");
#endif

        Stop();

        yield return new WaitForSeconds(crossfadeDelay);

        _audioSource.clip = GetLastTrack();
        _audioSource.Play();
    }*/
    //FADE OUT TRACK
    IEnumerator FadeOutTrack()
    {
#if UNITY_EDITOR && DEBUG
        Debug.Log("AUDIO MUSIC - " + "Fading out current track");
#endif

        float _interval = (crossfadeTracks ? (crossfadeDelay / crossfadePasses) : 1f);
        float _fadeAmountPerInterval = (musicPlayer.volume / crossfadePasses); //GET FADE INTERVAL

        for (int i = 1; i <= crossfadePasses; i++)
        {
            yield return new WaitForSeconds(_interval); //WAIT

            //musicVolume -= (int)_fadeAmountPerInterval;
            musicPlayer.volume -= _fadeAmountPerInterval;

            if (musicPlayer.volume <= 0f)
            {
                if (loop) QueueUpMusic(musicPlayer.clip); //QUEUE UP AGAIN
                musicPlayer.Stop();
                break;
            }
        }

        //StartCoroutine(FadeInTrack());  //FADE IN NEXT TRACK
    }
    //FADE IN TRACK
    IEnumerator FadeInTrack(float _targetVolume = 1f)
    {
#if UNITY_EDITOR && DEBUG
        Debug.Log("AUDIO MUSIC - " + "Fading in current track");
#endif

        float _interval = (crossfadeTracks ? (crossfadeDelay / crossfadePasses) : 1f);
        float _fadeAmountPerInterval = (_targetVolume / crossfadePasses); //GET FADE INTERVAL

        for (int i = crossfadePasses; i > 0; i--)
        {
            yield return new WaitForSeconds(_interval); //WAIT

            //musicVolume += (int)_fadeAmountPerInterval;
            musicPlayer.volume += _fadeAmountPerInterval;
        }
    }
}
