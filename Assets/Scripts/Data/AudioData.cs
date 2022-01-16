using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AudioData")]
public class AudioData : ScriptableObject
{
    public MusicModel[] music;
    public SoundModel[] sounds;
}
