using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;

namespace UnityFramework.AD
{
    public class RewardedAdController : RewardableAdController
    {
#if UNITY_ANDROID
        private const string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        private const string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        private const string adUnitId = "unused";
#endif

        private RewardedAd rewardedAd;


        /// <summary>
        /// 광고 생성
        /// </summary>
        public override void Create(in System.Nullable<AdOptions> adOptions = null)
        {
            if (rewardedAd != null)
                Destroy();

            // 광고를 로드하는 데 사용되는 요청을 생성합니다.
            var adRequest = new AdRequest();

            // 광고를 로드하기 위한 요청을 보냅니다.
            RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    ADLogError($"Rewarded ad failed to load an ad with error : {error}");
                    return;
                }
                // 알 수 없는 이유로 작업이 실패한 경우.
                if (ad == null)
                {
                    ADLogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                    return;
                }

                ADLog($"Rewarded ad loaded with response : {ad.GetResponseInfo()}");
                rewardedAd = ad;

                // 기능을 확장하려면 광고 이벤트에 등록하세요.
                RegisterEventHandlers();

                OnLoaded?.Invoke(this);
            });
        }

        /// <summary>
        /// 광고 삭제
        /// </summary>
        public override void Destroy()
        {
            if (rewardedAd != null)
            {
                ADLog("Destroying rewarded ad.");
                rewardedAd.Destroy();
                rewardedAd = null;
            }
        }

        public override void Hide()
        {

        }

        public override bool IsLoaded()
        {
            if (rewardedAd == null)
                return false;
            return rewardedAd.CanShowAd();
        }

        /// <summary>
        /// 광고 로드
        /// </summary>
        public override void Load()
        {
            Create();
        }

        /// <summary>
        /// 로그
        /// </summary>
        public override void LogResponseInfo()
        {
            if (rewardedAd != null)
            {
                var responseInfo = rewardedAd.GetResponseInfo();
                ADLog(responseInfo);
            }
        }

        /// <summary>
        /// 광교 Show
        /// </summary>
        public override void Show()
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                ADLog("Showing rewarded ad.");
                //Reward -> 애드몹 콘솔에서 저장됨
                // Tpye => "Coin" 등 콘솔에서 저장된 타입이 string으로 나옴
                // Amount => 수량이 double 나옴
                rewardedAd.Show(ExecuteRewarded);
            }
            else
            {
                ADLogError("Rewarded ad is not ready yet.");
            }
        }

        protected override void RegisterEventHandlers()
        {
            // 광고로 인해 수익이 발생할 것으로 추정될 때 발생합니다
            rewardedAd.OnAdPaid += AdPaid;
            // 광고에 대한 노출이 기록될 때 발생합니다.
            rewardedAd.OnAdImpressionRecorded += AdImpressionRecorded;
            // 광고에 대한 클릭이 기록될 때 발생합니다.
            rewardedAd.OnAdClicked += AdClicked;
            // 광고가 전체 화면으로 열릴 때 발생합니다.
            rewardedAd.OnAdFullScreenContentOpened += AdFullScreenContentOpened;
            // 광고가 전체 화면 콘텐츠를 닫을 때 발생합니다.
            rewardedAd.OnAdFullScreenContentClosed += AdFullScreenContentClosed;
            // 광고가 전체 화면 콘텐츠를 열지 못할 때 발생합니다.
            rewardedAd.OnAdFullScreenContentFailed += AdFullScreenContentFailed;
        }
    }

}