using UnityEngine;
using UnityEngine.Advertisements;

public class FaramiraAds : MonoBehaviour, IUnityAdsListener
{
    bool testMode = true;

#if UNITY_ANDROID
    string gameId = "4208967";
    string InterstitialId = "Interstitial_Android";
    string RewardId = "Rewarded_Android";
#elif UNITY_IOS
    string InterstitialId = "Interstitial_iOS";
#else
    string InterstitialId = "";
#endif

    public delegate void RewardsAdReadyDelegate(string placementId);
    public RewardsAdReadyDelegate onRewardAdsReady;

    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
    }

    public void ShowInterstitialAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show(InterstitialId);
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }

    public void ShowRewardedAd()
    {
        Advertisement.Show(RewardId);
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == RewardId)
        {
            onRewardAdsReady?.Invoke(placementId);
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        throw new System.NotImplementedException();
    }
}
