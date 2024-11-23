using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishMenu : MonoBehaviour, ILevelObserver
{
    public static FinishMenu instance;
    public PlayerState playerState;
    public GameConfig gameConfig;

    public Text scoreText;
    public Text levelText;
    public Text newBestText;

    public Text ballsCountText;
    public Text ballsText;

    public GameObject twentyBalls;

    //public Text BonusText;
    public GameObject VideoIcon;
    public GameObject NoThankYouButton;
    public GameObject NonBonus;

    public GameObject LevelsPanel;
    public GameObject LevelsContainer;
    public LevelButton levelButtonPrefab;

    bool bonus;

    public void OnLevelComplete()
    {
        DOTween.Sequence().AppendInterval(RemoteSettings.GetFloat("FinishMenuDelay", 1.5f)).AppendCallback(() =>
        {
            bool isNewBest = playerState.score > playerState.best;
            if (isNewBest)
            {
                newBestText.gameObject.SetActive(true);

                playerState.best = playerState.score;
                playerState.Save();
            }

            scoreText.text = (playerState.score).ToString();
            levelText.text = (playerState.level + 1).ToString();
            ballsCountText.text = "+" + RemoteSettings.GetInt("ballsPerLevel", 15);

            bool isExtraBalls = false;
            if (playerState.level % RemoteSettings.GetInt("ExtraBallsLevel", 3) == 0)
            {
                twentyBalls.SetActive(true);
                isExtraBalls = true;
            }

            gameObject.SetActive(true);

            if (!isExtraBalls && Random.value < RemoteSettings.GetFloat("BonusLevelChance", 0.3f) && playerState.level > RemoteSettings.GetInt("AdsMinimumLevel", 10))
            {
                bonus = true;
            }
            else
            {
            }

            NoThankYouButton.SetActive(bonus);
            VideoIcon.SetActive(bonus);
            //BonusText.gameObject.SetActive(bonus);
            NonBonus.SetActive(!bonus);
        });
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
        LevelsPanel.SetActive(false);

        playerState.levelObservers.Add(this);

        twentyBalls.SetActive(false);
        newBestText.gameObject.SetActive(false);

        var levelsCount = gameConfig.labyrinthsStr.Length / gameConfig.wavesCount;
        for (var i = 0; i < levelsCount; i++)
        {
            var theme = gameConfig.levelThemeOrder.Length > 0 ? gameConfig.levelThemeOrder[i % gameConfig.levelThemeOrder.Length] : i;
            
            var levelButton = Instantiate(levelButtonPrefab.gameObject, LevelsContainer.transform).GetComponent<LevelButton>();
            levelButton.Image.color = gameConfig.colors[theme];
            levelButton.Text.text = (i + 1).ToString();
            levelButton.Text.color = gameConfig.capColors[theme];
            
            var level = i;
            levelButton.Button.onClick.AddListener(() =>
            {
                playerState.level = level;
                playerState.Save();
                
                SceneManager.LoadScene(1);
            });
        }
    }

    private void OnDestroy()
    {
        playerState.levelObservers.Remove(this);
    }

    public void NoThankYou()
    {
        Go();
    }

    void Go()
    {
        playerState.level++;
        playerState.Save();

        if (Level.instance.CanShowAds())
        {
            Level.instance.ShowAds((bool result) => {
                SceneManager.LoadScene(1);
            });
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void Next()
    {
        if (bonus)
        {
            if (AdsAnaliticsManager.instance.CanShowRewarded())
            {
                AdsAnaliticsManager.instance.ShowRewarded((bool result) =>
                    {
                        if (result)
                        {
                            PlayerPrefs.SetInt("BonusLevel", 1);
                            PlayerPrefs.SetInt("SkipAds", 1);
                            Go();
                        }
                    }
                );
            }
        }
        else
        {
            Go();
        }
    }

    public void OnLevelStart()
    {
    }
}
