using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    private LevelsData _levels;

    private string _pathPlayerData = "player_data.json";
    private string _pathLevelsData = "levels_data.json";

    private static PlayerDataModel _playerData;
    private static LevelDataModel _levelsData;

    private static DataManager _instance;

    public int LevelsCount
    {
        get
        {
            return _levels.data.Length;
        }
    }

    public string LevelsFolder
    {
        get
        {
            return _levels.folder;
        }
    }

    public LevelModel this[int id]
    {
        get
        {
            return _levels.data[id];
        }
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(this);
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
