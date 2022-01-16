using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;

    [SerializeField]
    private LevelsData _levels;

    private const string _pathPlayerData = "player_data.json";
    private static PlayerDataModel _playerData;

    #region Properties
    public static int LevelsCount
    {
        get
        {
            return _instance._levels.data.Length;
        }
    }

    public static int Level { get; set; }
    
    public static int LevelHighscore
    {
        get
        {
            var data = _playerData.levelData.Where(x => x.level == Level).FirstOrDefault();

            return (data != null) ? data.highscore : 0;
        }
    }
    #endregion
    
    #region Current Level Getters
    public static string GetLevelPath()
    {
        return $"{_instance._levels.folder}/{_instance._levels.data[Level].layoutFiles}";
    }

    public static int GetLevelTilesCount()
    {
        return _instance._levels.data[Level].tilesData.sprites.Length;
    }

    public static Sprite GetLevelSprite(int id)
    {
        return _instance._levels.data[Level].tilesData.sprites[id];
    }

    public static Color GetLevelSelectedColor() {
        return _instance._levels.data[Level].tilesData.selectedColor;
    }
    public static Color GetLevelUnselectedColor()
    {
        return _instance._levels.data[Level].tilesData.unselectedColor;
    }
    #endregion

    #region Menu Getters
    public static Tuple<bool, bool> GetLevelData(int level)
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

    public static bool IsLevelCompleted(int level)
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

        EventManager.AddEventListener(EventId.GAME_WON, UpdateHighscore);
        EventManager.AddEventListener(EventId.DATA_RESET, DeleteData);
    }

    private void UpdateHighscore(object obj)
    {
        var score = (int)obj;
        var data = _playerData.levelData.Where(x => x.level == Level).FirstOrDefault();
    
        if (data != null && score > data.highscore)
        {
            data.highscore = score;
        } else
        {
            data = new LevelDataModel();
            data.level = Level;
            data.highscore = score;
            data.hasJustGainedStar = true;

            _playerData.levelData.Add(data);
        }

        SaveData();
    }

    #region Persistent Data
    private void LoadData()
    {
        _playerData = Utils.ReadJson<PlayerDataModel>(_pathPlayerData);
        if (_playerData == null)
        {
            _playerData = new PlayerDataModel();
            _playerData.levelData = new List<LevelDataModel>();
        }
    }

    private static void SaveData()
    {
        Utils.WriteJson<PlayerDataModel>(_playerData, _pathPlayerData);
    }

    private static void DeleteData(object obj = null)
    {
        _playerData = new PlayerDataModel();
        _playerData.levelData = new List<LevelDataModel>();

        Utils.DeletePersistentData();
    }
    #endregion
}
