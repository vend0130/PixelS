using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMob : MonoBehaviour
{
    private InterstitialAd interstitial;//межстраничное
    private RewardedAd rewardedAd;//с вознаграждением
    private bool flagReward;

    private const string rewardAdId = "ca-app-pub-9460220176491279/9787898489";//ca-app-pub-3940256099942544/5224354917
    private const string interstitialId = "ca-app-pub-9460220176491279/5479341590";//ca-app-pub-3940256099942544/1033173712

    private void Awake()
    {
        if(I.adMob != null)
        {
            Destroy(gameObject);
            return;
        }
        I.adMob = this;
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

        RequestRewardedAd();
        RequestInterstitial();
    }

    public void LookInterstitialAD()
    {
        if (this.interstitial.IsLoaded() && !flagReward)
        {
            this.interstitial.Show();
        }
        flagReward = false;
    }

    public void LookRewardAD()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
            flagReward = true;
        }
    }

    public bool CheckRewardADIsLoaded()
    {
        return this.rewardedAd.IsLoaded();
    }

    #region RewardAD
    private void RequestRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = rewardAdId;
#else
        string adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestRewardedAd();
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        I.gm.GoodRewardAd();
    }
    #endregion

    #region InterstitialADS
    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = interstitialId;
#else
        string adUnitId = "unexpected_platform";
#endif
        this.interstitial = new InterstitialAd(adUnitId);
        
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }
    
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        interstitial.Destroy();
        RequestInterstitial();
    }
    #endregion


}
