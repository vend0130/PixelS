using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    private Pixels _currentPixel;
    private StateGame _stateGame;
    private int _countLevels;
    private bool _firstSwipeInLevel;
    private bool _rotation;
    [SerializeField] private LayerMask _layerBorder;
    [SerializeField] private GameObject _endPrefab;
    [SerializeField] private GameObject particale;
    [SerializeField] private Pixels[] prefabLevels;

    public Pixels currentPixels => _currentPixel;
    public StateGame stateGame => _stateGame;
    public int countLevels => _countLevels;
    public bool firstSwipeInLevel { get => _firstSwipeInLevel; set => _firstSwipeInLevel = value; }
    public bool rotation { get => _rotation; set => _rotation = value; }
    public LayerMask layerBorder => _layerBorder;
    public GameObject endPrefab => _endPrefab;

    private void Awake()
    {
        I.gm = this;
        I.Load();
        _countLevels = prefabLevels.Length;
        particale.SetActive(false);
    }

    private void Start()
    {
        _currentPixel = Instantiate(prefabLevels[I.currentLevel], transform);
        GamePlay(true);
    }

    public void Completed()
    {
        particale.SetActive(false);
        particale.SetActive(true);
        _stateGame = StateGame.EndGame;
        if (I.progress < I.currentLevel + 1)
            I.progress = I.currentLevel + 1;
        I.gui.Completed();
    }

    public void GamePlay(bool first = false)
    {
        _stateGame = StateGame.Gameplay;
        _firstSwipeInLevel = false;
        _rotation = false;
        I.gui.Gameplay(first);
    }

    public void NextLevel(int type, int goLevel = 0)//0 - next, 1 - again, 2 - select level
    {
        Destroy(_currentPixel.gameObject);
        bool next = false;
        if (type == 0 && I.currentLevel == _countLevels - 1 && !I.endGame)//type = 0/true
            I.endGame = true;
        if (I.endGame && type == 0)//случайный уровень
        {
            int random = Random.Range(0, _countLevels);
            I.currentLevel = random;
        }
        else if (type == 2 && goLevel >= 0 && goLevel < _countLevels)//выбраный
        {
            if(goLevel <= I.progress)//заблокировать не открытые уровни
            {
                I.gui.BtnSelectLevel();
                I.currentLevel = goLevel;
            }
        }
        else if (I.currentLevel < _countLevels - 1 && type == 0)//следующий
        {
            I.currentLevel++;
            next = true;
        }

        if((!next && Random.Range(0, 2) == 0) || Random.Range(0, 5) == 0)
            I.adMob.LookInterstitialAD();

        _currentPixel = Instantiate(prefabLevels[I.currentLevel], transform);
        GamePlay();
        I.Save();
    }

    public void GoodRewardAd()
    {
        _currentPixel.OpenHelp();//открываем подсказки
        I.levelHelp = I.currentLevel; //на текущем уровни позказки
        NextLevel(1);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

public enum StateGame
{
    Gameplay,
    EndGame,
    None
}