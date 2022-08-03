using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _buttonOpenLeaderBoard;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _starText;
    [SerializeField] private AudioSource _coinSound;

    private const string LEADER_BOARD = "CgkIjKfFpdQbEAIQAA";

    private int _coin;
    private int _star;
    public const string COIN = "Coin";
    public const string STAR = "Star";
    public static GUIManager _instance;


    void Awake()
    {
        _instance = GetComponent<GUIManager>();
        _coin = PlayerPrefs.GetInt(COIN);
        _star = PlayerPrefs.GetInt(STAR);
        _coinText.text = _coin.ToString();
        _starText.text = _star.ToString();

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(success =>
        {
            if (success) { }
        });
    }

    public int Coin
    {
        get
        {
            return _coin;
        }

        set
        {
            _coin = value;
            PlayerPrefs.SetInt(COIN, _coin);
            AudioSource coin = Instantiate(_coinSound);
            _coinText.text = _coin.ToString();
            Destroy(coin.gameObject, 1);
        }
    }
    public int Star
    {
        get
        {
            return _star;
        }

        set
        {
            _star = value;
            PlayerPrefs.SetInt(STAR, _star);
            _starText.text = _star.ToString();
            Social.ReportScore(_star,LEADER_BOARD, (bool success) => { });
        }
    }

    public void ShowLeaderBoard()
    {
        Social.ShowLeaderboardUI();
    }
}
