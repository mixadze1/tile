using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _starText;

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
            _coinText.text = _coin.ToString();
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
        }
    }
}
