using System;
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

    #region Properties
    public int LevelsCount
    {
        get
        {
            return _levels.data.Length;
        }
    }

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
    
    public int LevelHighscore
    {
        get
        {
            var data = _playerData.levelData.Where(x => x.level == _level).FirstOrDefault();

            return (data != null) ? data.highscore : 0;
        }
    }
    #endregion
    
    #region Current Level Getters
    public string GetLevelPath()
    {
        return $"{_levels.folder}/{_levels.data[_level].layoutFiles}";
    }

    public int GetLevelTilesCount()
    {
        return _levels.data[_level].tilesData.sprites.Length;
    }

    public Sprite GetLevelSprite(int id)
    {
        return _levels.data[_level].tilesData.sprites[id];
    }

    public Color GetLevelSelectedColor() {
        return _levels.data[_level].tilesData.selectedColor;
    }
    public Color GetLevelUnselectedColor()
    {
        return _levels.data[_level].tilesData.unselectedColor;
    }
    #endregion

    #region Menu Getters
    public Tuple<bool, bool> GetLevelData(int level)
    {
        var data = _playerData.levelData.Where(x => x != null && x.level == level).FirstOrDefault();

        var hasCompletedLevel = data != null;
        var hasJustGainedStar = (data != null) ? data.hasJustGainedStar : false;

        if (hasJustGainedStar)
        {
            data.hasJustGainedStar = false;

            SaveData();
        }


        return new Tuple<bool, bool>(hasCompletedLevel, hasJustGainedStar);
    }

    public bool IsLevelCompleted(int level)
    {
        var data = _playerData.levelData.Where(x => x != null && x.level == level).FirstOrDefault();

        return data != null;
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

        EventManager.AddEventListener(EventId.GAME_WON, UpdateScore);
        EventManager.AddEventListener(EventId.DATA_RESET, DeleteData);
    }

    private void UpdateScore(object obj)
    {
        var score = (int)obj;
        var data = _playerData.levelData.Where(x => x.level == _level).FirstOrDefault();
    
        if (data != null && score > data.highscore)
        {
            data.highscore = score;
        } else
        {
            data = new LevelDataModel();
            data.level = _level;
            data.highscore = score;
            data.hasJustGainedStar = true;

            _playerData.levelData.Add(data);
        }

        SaveData();
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

    private void SaveData()
    {
        Utils.WriteJson<PlayerDataModel>(_playerData, _pathPlayerData);
    }

    private void DeleteData(object obj = null)
    {
        _playerData = new PlayerDataModel();

        Utils.DeletePersistentData();
    }
}
