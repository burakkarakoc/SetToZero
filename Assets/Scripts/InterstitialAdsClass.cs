using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdsClass : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{

    public InterstitialAdsClass Instance;

    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;

    private float lastAdShownTime = float.MinValue; // Initialize with the min value.


    void Awake()
    {
        // Enforce a singleton pattern in case more than one AdManager is in the scene.
        if (gameObject)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent AdManager from being destroyed.
        }
        else
        {
            Destroy(gameObject); // Ensure there is only one AdManager.
        }

#if UNITY_IOS
        _adUnitId = _iOsAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
        // Get the Ad Unit ID for the current platform:
        //_adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
        //    ? _iOsAdUnitId
        //    : _androidAdUnitId;
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        if (Time.time - lastAdShownTime >= 120f) // 120 seconds have passed
        {
            // Note that if the ad content wasn't previously loaded, this method will fail
            Debug.Log("Showing Ad: " + _adUnitId);
            Advertisement.Show(_adUnitId, this);
        }
        else
        {
            Debug.Log("Ad not ready — waiting for cooldown.");
            Debug.Log("Remaining: " + (120f - (Time.time - lastAdShownTime)) + " seconds...");
        }
    }

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string _adUnitId)
    {
        lastAdShownTime = Time.time; // Update the last ad shown time.
    }
    public void OnUnityAdsShowClick(string _adUnitId) { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        lastAdShownTime = Time.time; // Update the last ad shown time.
    }
}