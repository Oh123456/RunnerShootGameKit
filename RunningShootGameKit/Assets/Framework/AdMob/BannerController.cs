using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;

namespace UnityFramework.AD
{
    public class BannerController : AdController
    {

#if UNITY_ANDROID
        //private const string adUnitId = "ca-app-pub-8211238575744446~8939783062";
        private const string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        private const string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        private const string adUnitId = "unused";
#endif

        private BannerView bannerView;

        public override void Create(in System.Nullable<AdOptions> adOptions = null)
        {

            ADLog("Creating banner view.");

            if (bannerView != null)
            {
                Destroy();
            }

            AdSize adSize = AdSize.Banner;
            AdPosition adPosition = AdPosition.Bottom;
            if (adOptions != null)
            {
                AdOptions options = adOptions.Value;
                adSize = options.adSize;
                adPosition = options.adPosition;
            }


            // Create a 320x50 banner at top of the screen.
            bannerView = new BannerView(adUnitId, adSize, adPosition);


            // 이벤트 구독
            RegisterEventHandlers();

            ADLog("Banner view created.");
        }

        public override void Destroy()
        {
            if (bannerView != null)
            {
                ADLog("Destroying banner view.");
                bannerView.Destroy();
                bannerView = null;
            }

        }

        public override void Hide()
        {
            if (bannerView != null)
            {
                ADLog("Hiding banner view.");
                bannerView.Hide();
            }
        }

        public override bool IsLoaded()
        {
            return bannerView != null;
        }

        public override void Load()
        {
            if (bannerView == null)
            {
                Create();
            }

            ADLog("Loading banner ad.");
            bannerView.LoadAd(new AdRequest());
            OnLoaded?.Invoke(this);
        }

        public override void LogResponseInfo()
        {
            if (bannerView != null)
            {
                var responseInfo = bannerView.GetResponseInfo();
                if (responseInfo != null)
                {
                    ADLog(responseInfo);
                }
            }
        }

        public override void Show()
        {
            if (bannerView != null)
            {
                ADLog("Showing banner view.");
                bannerView.Show();
            }
        }


        protected override void RegisterEventHandlers()
        {
            // Raised when an ad is loaded into the banner view.
            bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + bannerView.GetResponseInfo());

            };
            // Raised when an ad fails to load into the banner view.
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : " + error);
            };
            // Raised when the ad is estimated to have earned money.
            bannerView.OnAdPaid += AdPaid;
            // Raised when an impression is recorded for an ad.
            bannerView.OnAdImpressionRecorded += AdImpressionRecorded;
            // Raised when a click is recorded for an ad.
            bannerView.OnAdClicked += AdClicked;
            // Raised when an ad opened full screen content.
            bannerView.OnAdFullScreenContentOpened += AdFullScreenContentOpened;
            // Raised when the ad closed full screen content.
            bannerView.OnAdFullScreenContentClosed += AdFullScreenContentClosed;
        }
    }

}