using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;

namespace UnityFramework.AD
{
    public class RewardedInterstitialAdController : RewardableAdController
    {

#if UNITY_ANDROID
        private const string adUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
        private const string adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
        private const string adUnitId = "unused";
#endif

        private RewardedInterstitialAd rewardedInterstitialAd;

        public override void Create(in AdOptions? adOptions = null)
        {
            if (rewardedInterstitialAd != null)
            {
                Destroy();
            }

            ADLog("Loading rewarded interstitial ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            RewardedInterstitialAd.Load(adUnitId, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    // If the operation failed with a reason.
                    if (error != null)
                    {
                        ADLogError($"Rewarded interstitial ad failed to load an ad with error : {error}");
                        return;
                    }
                    // If the operation failed for unknown reasons.
                    // This is an unexpexted error, please report this bug if it happens.
                    if (ad == null)
                    {
                        ADLogError("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                        return;
                    }

                    // The operation completed successfully.
                    ADLog($"Rewarded interstitial ad loaded with response : {ad.GetResponseInfo()}");
                    rewardedInterstitialAd = ad;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers();
                    OnLoaded?.Invoke(this);
                });

        }

        public override void Destroy()
        {
            if (rewardedInterstitialAd != null)
            {
                ADLog("Destroying rewarded interstitial ad.");
                rewardedInterstitialAd.Destroy();
                rewardedInterstitialAd = null;
            }

        }

        public override void Hide()
        {

        }

        public override bool IsLoaded()
        {
            if (rewardedInterstitialAd == null)
                return false;
            return rewardedInterstitialAd.CanShowAd();
        }

        public override void Load()
        {
            Create();
        }

        public override void LogResponseInfo()
        {
            if (rewardedInterstitialAd != null)
            {
                var responseInfo = rewardedInterstitialAd.GetResponseInfo();
                ADLog(responseInfo);
            }
        }

        public override void Show()
        {
            if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
            {
                rewardedInterstitialAd.Show(ExecuteRewarded);
            }
            else
            {
                ADLogError("Rewarded interstitial ad is not ready yet.");
            }
        }

        protected override void RegisterEventHandlers()
        {
            // Raised when the ad is estimated to have earned money.
            rewardedInterstitialAd.OnAdPaid += AdPaid;
            // Raised when an impression is recorded for an ad.
            rewardedInterstitialAd.OnAdImpressionRecorded += AdImpressionRecorded;
            // Raised when a click is recorded for an ad.
            rewardedInterstitialAd.OnAdClicked += AdClicked;
            // Raised when an ad opened full screen content.
            rewardedInterstitialAd.OnAdFullScreenContentOpened += AdFullScreenContentOpened;
            // Raised when the ad closed full screen content.
            rewardedInterstitialAd.OnAdFullScreenContentClosed += AdFullScreenContentClosed;
            // Raised when the ad failed to open full screen content.
            rewardedInterstitialAd.OnAdFullScreenContentFailed += AdFullScreenContentFailed;
        }
    }
}
