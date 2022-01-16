using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    private AudioSource _soundSource;
    private AudioSource _musicSource;

    [SerializeField]
    private AudioData _data;

    public static bool SoundOn { get; private set; }
    public static bool MusicOn { get; private set; }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;

            MusicOn = Utils.GetBoolPlayerPref("music");
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;

            SoundOn = Utils.GetBoolPlayerPref("sound");
            _soundSource = gameObject.AddComponent<AudioSource>();
            _soundSource.loop = false;
        }
    }

    private void Start()
    {
        if (MusicOn)
        {
            PlayMusic();
        }

        if (SoundOn)
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
        MusicOn = !MusicOn;

        if (MusicOn)
        {
            PlayMusic();
            EventManager.AddEventListener(EventId.CHANGE_SCREEN, PlayMusic);
        }
        else
        {
            _musicSource.Stop();
            EventManager.RemoveEventListener(EventId.CHANGE_SCREEN, PlayMusic);
        }

        Utils.SetBoolPlayerPref("music", MusicOn);
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
        if (!MusicOn)
        {
            return;
        }

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
        SoundOn = !SoundOn;

        if (SoundOn)
        {
            EventManager.AddEventListener(EventId.PLAY_SOUND, PlaySound);
        }
        else
        {
            _soundSource.Stop();
            EventManager.RemoveEventListener(EventId.PLAY_SOUND, PlaySound);
        }

        Utils.SetBoolPlayerPref("sound", SoundOn);
    }

    private void PlaySound(object obj)
    {
        if (!SoundOn)
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
