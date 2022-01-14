using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    private LevelsData _levels;

    private int _level;

    private string _pathPlayerData = "player_data.json";
    private string _pathLevelsData = "levels_data.json";

    private static PlayerDataModel _playerData;
    private static LevelDataModel _levelsData;

    private static DataManager _instance;

    internal int Level
    {
        get
        {
            return _level;
        }
        set
        {
            _level = value;
        }
    }

    internal int LevelsCount
    {
        get
        {
            return _levels.data.Length;
        }
    }

    internal string GetLevelPath()
    {
        return  $"{_levels.folder}/{_levels.data[_level].layoutFiles}";
    }

    internal int GetLevelTilesCount()
    {
        return _levels.data[_level].tilesData.sprites.Length;
    }

    internal Sprite GetSprite(int id)
    {
        return _levels.data[_level].tilesData.sprites[id];
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        } else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        _playerData = Utils.ReadJson<PlayerDataModel>(_pathPlayerData);
        if (_playerData == null)
        {
            _playerData = new PlayerDataModel();
        }

        _levelsData = Utils.ReadJson<LevelDataModel>(_pathLevelsData);
        if (_levelsData == null)
        {
            _levelsData = new LevelDataModel();
        }
    }

    private void SaveData()
    {
        Utils.WriteJson<PlayerDataModel>(_playerData, _pathPlayerData);
        Utils.WriteJson<LevelDataModel>(_levelsData, _pathLevelsData);
    }
}
