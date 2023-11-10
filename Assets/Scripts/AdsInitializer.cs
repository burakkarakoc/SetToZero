using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iosGameId;
    [SerializeField] bool _testMode = false;
    private string _gameId;

    //private InterstitialAdsClass interstitialAds;


    //[SerializeField] RewardedAdsButton rewardedAdsButton;

    //[SerializeField] RewardedAdsButton rewardedAdsButton2;


    //[SerializeField] InterstitialAdsButton interstitialAdsButton;


    void Awake()
    {
        InitializeAds();

        //interstitialAds = FindObjectOfType<InterstitialAdsClass>();
    }

    public void InitializeAds()
    {

#if UNITY_IOS
        _gameId = _iosGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#endif
        //_gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iosGameId : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }


    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        //rewardedAdsButton.LoadAd();

        //interstitialAds.LoadAd();
        //rewardedAdsButton2.LoadAd();
        //interstitialAdsButton.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}