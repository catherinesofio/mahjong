using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    private bool _soundOn;
    private bool _musicOn;

    private AudioSource _soundSource;
    private AudioSource _musicSource;

    [SerializeField]
    private AudioData _data;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;

            _musicOn = Utils.GetBoolPlayerPref("music");
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;

            _soundOn = Utils.GetBoolPlayerPref("sound");
            _soundSource = gameObject.AddComponent<AudioSource>();
            _soundSource.loop = false;
        }
    }

    private void Start()
    {
        if (_musicOn)
        {
            PlayMusic();
        }

        if (_soundOn)
        {
            EventManager.AddEventListener(EventId.PLAY_SOUND, PlaySound);
        }

        EventManager.AddEventListener(EventId.TOGGLE_MUSIC, ToggleMusic);
        EventManager.AddEventListener(EventId.TOGGLE_SOUND, ToggleSound);
        EventManager.AddEventListener(EventId.CHANGE_SCREEN, PlayMusic);
    }

    #region Music
    private void ToggleMusic(object obj = null)
    {
        _musicOn = !_musicOn;

        if (_musicOn)
        {
            PlayMusic();
            EventManager.AddEventListener(EventId.CHANGE_SCREEN, PlayMusic);
        }
        else
        {
            _musicSource.Stop();
            EventManager.RemoveEventListener(EventId.CHANGE_SCREEN, PlayMusic);
        }

        Utils.SetBoolPlayerPref("music", _musicOn);
    }

    private void PlayMusic()
    {
        var musicModel = _data.music.Where(x => x.screenId == ScreenManager.Screen).FirstOrDefault();

        if (musicModel != null && musicModel.clip != null)
        {
            _musicSource.Stop();
            _musicSource.clip = musicModel.clip;
            _musicSource.Play();
        }
    }

    private void PlayMusic(object obj)
    {
        var screenId = (ScreenId)obj;
        var musicModel = _data.music.Where(x => x.screenId == screenId).FirstOrDefault();

        if (musicModel != null && musicModel.clip != null)
        {
            _musicSource.Stop();
            _musicSource.clip = musicModel.clip;
            _musicSource.Play();
        }
    }
    #endregion

    #region Sound
    private void ToggleSound(object obj = null)
    {
        _soundOn = !_soundOn;

        if (_soundOn)
        {
            EventManager.AddEventListener(EventId.PLAY_SOUND, PlaySound);
        }
        else
        {
            _soundSource.Stop();
            EventManager.RemoveEventListener(EventId.PLAY_SOUND, PlaySound);
        }

        Utils.SetBoolPlayerPref("sound", _soundOn);
    }

    private void PlaySound(object obj)
    {
        if (!_soundOn)
        {
            return;
        }

        var soundId = (SoundId)obj;
        var soundModel = _data.sounds.Where(x => x.soundId == soundId).FirstOrDefault();

        if (soundModel != null && soundModel.clip != null)
        {
            _soundSource.Stop();
            _soundSource.clip = soundModel.clip;
            _soundSource.Play();
        }
    }
    #endregion
}
