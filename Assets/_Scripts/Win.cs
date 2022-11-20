using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] private int _level = 0;

    [SerializeField] private TextMeshProUGUI _levelText;

    [SerializeField] private GameObject _bonusActivate;

    private const string SAFE_LEVEL = "safeLevel";

    public int Level { get { return _level; } set { _level = value; } }

    public void InitializeLevel()
    {
        _level = PlayerPrefs.GetInt(SAFE_LEVEL);
        if (_level == 0)
        {
            _level++;
            if (_level % 3 == 0)
                YandexSDK.instance.ShowInterstitial();
        }
           
        _levelText.text = _level.ToString();
    }

    public void NextLevel()
    {
        _level = PlayerPrefs.GetInt(SAFE_LEVEL);
        if (_level == 0)
        { 
            _level++;
        }
        _level++;
        StartCoroutine(CalculateCoin());
        StartCoroutine(CalculateStar());
        Bonus();
        _levelText.text =  _level.ToString();
        PlayerPrefs.SetInt(SAFE_LEVEL, _level);
    }

    private IEnumerator CalculateCoin()
    {
        int coin = 100;
        while (coin > 0)
        {
            yield return new WaitForSeconds(0.05f);
            GUIManager._instance.Coin += 3;
            coin -= 3;
        }    
    }

    private void Bonus()
    {
        int chance = Random.Range(0, 4);
        if (chance < 1)
            _bonusActivate.SetActive(true);
    }
    
    public void BonusActivate()
    {
        _bonusActivate.SetActive(false);
        StartCoroutine(CalculateCoin());
    }

    private IEnumerator CalculateStar()
    {
        int star = 4;
        while (star > 0)
        {
            
            yield return new WaitForSeconds(0.18f);
            GUIManager._instance.Star += 1;
            star -= 1;
        }
       
    }
}
