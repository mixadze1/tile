using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour
{

    private YandexSDK _sdk;
    [SerializeField] private Win _win;


    private void Start()
    {
        _sdk = YandexSDK.instance;
        _sdk.onRewardedAdReward += Reward;
    }

    public void Reward(string placement)
    {
        if(placement == "Coin")
        {
            _win.BonusActivate();
        }
    }
}
