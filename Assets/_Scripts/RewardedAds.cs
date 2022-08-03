using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Win _win;
    [SerializeField] private Button buttonShowAd;
    [SerializeField] private string AndroidAdId = "Rewarded_Android";
    [SerializeField] private string IOSAdId = "Rewarded_iOS";

    private string AdID;

    private void Awake()
    {
        AdID = (Application.platform == RuntimePlatform.IPhonePlayer) ? IOSAdId : AndroidAdId;
        LoadAd();
    }

    private void Start()
    {
        LoadAd();
    }

    private void LoadAd()
    {
        Debug.Log("Loading Ad" + AdID);
        Advertisement.Load(AdID, this);
    }

    public void ShowAd()
    {
        Advertisement.Show(AdID, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Ad loaded" + placementId);
        if (placementId.Equals(AdID))
        {
            buttonShowAd.onClick.AddListener(ShowAd);
        }
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(AdID) && showCompletionState.
                Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _win.BonusActivate();
        }
    }
}
