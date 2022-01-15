using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private int _level;

    [SerializeField]
    private LevelsData _levels;

    private string _pathPlayerData = "player_data.json";
    private static PlayerDataModel _playerData;

    private static DataManager _instance;

    #region General Properties
    public int Level
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

    public int LevelsCount
    {
        get
        {
            return _levels.data.Length;
        }
    }

    public int Highscore
    {
        get
        {
            var data = _playerData.levelData.Where(x => x.level == _level).FirstOrDefault();

            return (data != null) ? data.highscore : 0;
        }
    }
    #endregion

    #region Level Getters
    public bool IsLevelCompleted(int level)
    {
        var data = _playerData.levelData.Where(x => x != null && x.level == level).FirstOrDefault();

        return data != null;
    }

    public string GetLevelPath()
    {
        return $"{_levels.folder}/{_levels.data[_level].layoutFiles}";
    }

    public int GetLevelTilesCount()
    {
        return _levels.data[_level].tilesData.sprites.Length;
    }

    public Sprite GetSprite(int id)
    {
        return _levels.data[_level].tilesData.sprites[id];
    }

    public Color GetTileSelectedColor() {
        return _levels.data[_level].tilesData.selectedColor;
    }
    public Color GetTileUnselectedColor()
    {
        return _levels.data[_level].tilesData.unselectedColor;
    }
    #endregion

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

        EventManager.AddEventListener(EventId.GAME_WON, SaveScore);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(EventId.GAME_WON, SaveScore);
    }

    private void LoadData()
    {
        _playerData = Utils.ReadJson<PlayerDataModel>(_pathPlayerData);
        if (_playerData == null)
        {
            _playerData = new PlayerDataModel();
            _playerData.levelData = new List<LevelDataModel>();
        }
    }

    private void SaveScore(object obj)
    {
        var score = (int)obj;
        var data = _playerData.levelData.Where(x => x.level == _level).FirstOrDefault();

        Debug.Log(JsonUtility.ToJson(_playerData));
        if (data != null && score > data.highscore)
        {
            data.highscore = score;
        } else
        {
            data = new LevelDataModel();
            data.level = _level;
            data.highscore = score;

            _playerData.levelData.Add(data);
        }
        Debug.Log(JsonUtility.ToJson(_playerData));

        Utils.WriteJson<PlayerDataModel>(_playerData, _pathPlayerData);
    }
}
