using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject endGame;
    [SerializeField] private GameObject completedtLevel;
    [SerializeField] private GameObject completedGame;
    [SerializeField] private GameObject AllLevels;
    [SerializeField] private GameObject watchAd;
    [SerializeField] private Transform poolLevelsPanel;
    [SerializeField] private Text textLevel;
    [SerializeField] private Image audioIcon0;//gameplay
    [SerializeField] private Image audioIcon1;//endgame
    [SerializeField] private Sprite audioOn;
    [SerializeField] private Sprite audioOff;
    [SerializeField] private Image jiggleIcon0;//gameplay
    [SerializeField] private Image jiggleIcon1;//endgame
    [SerializeField] private Sprite jiggleOn;
    [SerializeField] private Sprite jiggleOff;
    [SerializeField] private GameObject textYHCAL;
    [SerializeField] private Text nextText;
    [SerializeField] private GameObject learningBlock;
    [SerializeField] private Text learningText;
    [SerializeField] private GameObject buttonHelp;
    [SerializeField] private RectTransform[] herts;

    private bool menu;
    private Animator animator;
    private Coroutine[] animatedHears;
    private GM gm;
    private List<PoolLevels> poolLevels = new List<PoolLevels>();
    private Coroutine coroutineUrl;
    private bool appStatus = true;

    private void Awake()
    {
        I.gui = this;
        animator = GetComponent<Animator>();
        animatedHears = new Coroutine[herts.Length];
    }

    private void Start()
    {
        gm = I.gm;
        foreach (Transform _image in poolLevelsPanel)
        {
            PoolLevels _pl = new PoolLevels();
            _pl.image = _image.gameObject.GetComponent<Image>();
            _pl.text = _image.GetComponentInChildren<Text>();
            poolLevels.Add(_pl);
        }
        LockUnlockLevels();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BtnAudioPlay();
            if (AllLevels.activeSelf)
            {
                BtnSelectLevel();
            }
            else if (watchAd.activeSelf)
            {
                BtnHelp(false);
            }
            else if(gm.stateGame == StateGame.Gameplay)
            {
                BtnMenu();
            }
        }
    }

    public void Gameplay(bool first)
    {
        if (first)
        {
            endGame.SetActive(false);
            AllLevels.SetActive(false);
            Audio();
            Jiggle();
        }
        else if (animator.GetInteger("EndGame") == 1)
            animator.SetInteger("EndGame", 2);

        if (!first)
            BtnMenu(true);


        if (I.endGame || I.currentLevel != gm.countLevels - 1)
            textLevel.text = "LEVEL " + (I.currentLevel + 1).ToString();
        else
            textLevel.text = "LAST LEVEL";

        LearningToPlay();
    }

    public void LearningToPlay(bool _closeLearning = false)
    {
        if (_closeLearning && !I.endGame && (I.currentLevel == 1 || I.currentLevel == 0))
        {
            if(I.currentLevel == 0 && gm.currentPixels.direction != Vector2.zero || I.currentLevel == 1)
                animator.SetBool("Learning", false);
        }
        else
        {
            if (I.currentLevel == 0 && !I.endGame)
            {
                learningText.text = "SWIPE TO MOVE";
                animator.SetBool("Learning", true);
                animator.SetInteger("TypeHandLearning", 1);
            }
            else if (I.currentLevel == 1 && !I.endGame)
            {
                learningText.text = "TAP TO ROTATION";
                animator.SetBool("Learning", true);
                animator.SetInteger("TypeHandLearning", 2);
            }
            else if (learningBlock.activeSelf)
            {
                learningBlock.SetActive(false);
            }
        }
    }

    public void Completed()
    {
        if (I.currentLevel == gm.countLevels - 1 && !I.endGame)//при завершении всей игры
        {
            completedtLevel.SetActive(false);
            completedGame.SetActive(true);
        }
        else if (!completedtLevel.activeSelf || completedGame.activeSelf)
        {
            completedtLevel.SetActive(true);
            completedGame.SetActive(false);
        }

        animator.SetInteger("EndGame", 1);
        if (I.endGame)
        {
            nextText.text = "RANDOM";
            textYHCAL.SetActive(true);
        }
        else
        {
            nextText.text = "NEXT";
            textYHCAL.SetActive(false);
        }
        BtnMenu(true);
    }

    public void BtnNextLevel(int type)//0 - next, 1 - again
    {
        Flag(false);
        gm.NextLevel(type);
    }

    public void BtnMenu(bool close = false)
    {
        if (!menu && !close)
        {
            menu = true;
            animator.SetInteger("Menu", 1);
        }
        else if (menu || (close && menu))
        {
            menu = false;
            animator.SetInteger("Menu", 2);
        }
    }

    public void BtnAudio()
    {
        if (I.audio)
            I.audio = false;
        else
        {
            I.audio = true;
            I.audioManager.Play("button");
        }
        I.Save();
        Audio();
    }

    public void BtnJiggle()
    {
        I.jiggle = !I.jiggle;
        I.Save();
        Jiggle();
    }

    public void BtnHelp(bool state)//true - открыть; false - закрыть
    {
        if (gm.currentPixels.helpActive)
            return;

        if (state)
        {
            if (I.adMob.CheckRewardADIsLoaded())
                I.adMob.LookRewardAD();
            else
            {
                BtnMenu(true);
                animator.SetInteger("WatchAD", 1);
            }

        }
        else
            animator.SetInteger("WatchAD", 2);
    }

    public void BtnInstagram()
    {
        if (coroutineUrl != null)
            return;

        coroutineUrl = StartCoroutine(CoroutineUrl());
    }

    public void BtnAudioPlay()
    {
        I.audioManager.Play("button");
    }

    public void BtnSelectLevel()
    {
        if (AllLevels.activeSelf)
        {
            animator.SetInteger("SelectLevel", 2);
        }
        else
        {
            animator.SetInteger("SelectLevel", 1);
            LockUnlockLevels();
        }
    }
    public void LockUnlockLevels()
    {
        int plusI = 1;
        if (I.progress == 0)
            plusI = 1;
        else
            plusI = 1;
        for (int i = 0; i < I.progress + plusI; i++)
        {
            if (i >= gm.countLevels)
                return;
            poolLevels[i].image.color = new Color(0, 0, 0, 1);
            poolLevels[i].text.color = new Color(0, 0, 0, 1);
        }
        for (int i = I.progress + plusI; i < poolLevels.Count; i++)
        {
            if (i >= gm.countLevels)
                return;
            poolLevels[i].image.color = new Color(0, 0, 0, 0f);
            poolLevels[i].text.color = new Color(0, 0, 0, .65f);
        }
    }

    public void Flag(bool state_flag)
    {
        if (!state_flag)
            animator.SetBool("Flag", false);
        else if(!animator.GetBool("Flag") && buttonHelp.activeSelf)
            animator.SetBool("Flag", true);
    }

    public void BtnHeartEnter(int id)
    {
        if(animatedHears[id] != null)
        {
            StopCoroutine(animatedHears[id]);
        }
        animatedHears[id] = StartCoroutine(HeartCoroutine(id, 0));
    }
    public void BtnHeartExit(int id)
    {
        if (animatedHears[id] != null)
        {
            StopCoroutine(animatedHears[id]);
        }
        animatedHears[id] = StartCoroutine(HeartCoroutine(id, 1));
    }
    //Canvas/CurrentLevel
    public void BtnClearSave()
    {
        I.ClearSave();
        gm.LoadScene();
    }

    public void Audio()
    {
        if (I.audio)
        {
            audioIcon0.sprite = audioOn;
            audioIcon1.sprite = audioOn;
        }
        else
        {
            audioIcon0.sprite = audioOff;
            audioIcon1.sprite = audioOff;
        }
    }

    public void Jiggle()
    {
        if (I.jiggle)
        {
            jiggleIcon0.sprite = jiggleOn;
            jiggleIcon1.sprite = jiggleOn;
        }
        else
        {
            jiggleIcon0.sprite = jiggleOff;
            jiggleIcon1.sprite = jiggleOff;
        }
    }

    private void OnApplicationPause(bool _pause)
    {
        if (_pause)
            appStatus = false;
    }

    private IEnumerator CoroutineUrl()
    {
        Application.OpenURL(I.instagramAppLink);
        Debug.Log("Open app...");
        yield return new WaitForSeconds(1f);
        if (appStatus)
        {
            Debug.Log("Failed open app. Open Browser...");
            Application.OpenURL(I.instagramWebLink);
        }
        appStatus = true;
        coroutineUrl = null;
    }

    private IEnumerator HeartCoroutine(int id, int type)
    {
        float speedAnim = 500f;
        Vector2 currentSize = herts[id].sizeDelta;
        if(type == 0)
        {
            while (currentSize.x < 125)
            {
                currentSize.x += Time.deltaTime * speedAnim;
                currentSize.y = currentSize.x;
                herts[id].sizeDelta = currentSize;
                yield return null;
            }
        }
        else
        {
            while (currentSize.x > 85)
            {
                currentSize.x -= Time.deltaTime * speedAnim;
                currentSize.x = Mathf.Clamp(currentSize.x, 85, 125);
                currentSize.y = currentSize.x;
                herts[id].sizeDelta = currentSize;
                yield return null;
            }
        }
    }
}

[System.Serializable]
public class PoolLevels
{
    public Image image;
    public Text text;
}