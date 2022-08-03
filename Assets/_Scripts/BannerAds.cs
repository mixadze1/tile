using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAds : MonoBehaviour
{
    [SerializeField] BannerPosition bannerPosition;
    [SerializeField] private string AndroidId = "Banner_Android";
    [SerializeField] private string IosId = "Banner_IOS";

    private string adId;

    private void Awake()
    {
        adId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? IosId : AndroidId;
    }

    private void Start()
    {
        Advertisement.Banner.SetPosition(bannerPosition);
        StartCoroutine(LoadAdBanner());
    }

    private IEnumerator LoadAdBanner()
    {
        yield return new WaitForSeconds(5f);
        LoadBanner();
    }

    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.Load(adId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("baner loaded");
        ShowBannerAd();
    }

    private void OnBannerError(string message)
    {
        Debug.Log($"banner Error:{ message}");
    }

    public void ShowBannerAd()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHiden,
            showCallback = OnBannerShown
        };
        Advertisement.Banner.Show(adId, options);
    }

    private void OnBannerClicked() { }

   private void OnBannerHiden() { }
    
    private void OnBannerShown() { }
}
