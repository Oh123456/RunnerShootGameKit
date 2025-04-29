using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;

namespace UnityFramework.AD
{
    public abstract class AdController : IAdController
    {
        ///// <summary>
        ///// 광고가 열리다 실패했을때
        ///// </summary>
        //public event System.Action<AdError> OnAdContentFailed;
        ///// <summary>
        ///// 광고가 닫혔을떄 
        ///// </summary>
        //public event System.Action OnAdContentClosed;

        /// <summary>
        /// 광고가 로드 됬을떄
        /// </summary>
        public System.Action<AdController> OnLoaded;


        public abstract void Show();
        public abstract void Hide();
        public abstract void Load();
        public abstract void Destroy();
        public abstract bool IsLoaded();
        public abstract void LogResponseInfo();
        protected abstract void RegisterEventHandlers();
        public abstract void Create(in System.Nullable<AdOptions> adOptions = null);

        protected void AdFullScreenContentClosed()
        {
            ADLog($"{TypeName()} ad full screen content closed.");
            //OnAdContentClosed?.Invoke();
        }

        protected void AdFullScreenContentFailed(AdError error)
        {
            ADLogError($"{TypeName()} ad failed to open full screen content with error : " + error);
            //OnAdContentFailed?.Invoke(error);
        }

        protected void AdFullScreenContentOpened()
        {
            ADLog($"{TypeName()} ad full screen content opened.");
        }

        protected void AdClicked()
        {
            ADLog($"{TypeName()} ad was clicked.");
        }

        protected void AdImpressionRecorded()
        {
            ADLog($"{TypeName()} ad recorded an impression.");
        }

        protected void AdPaid(AdValue adValue)
        {
            ADLog($"{TypeName()} ad paid {adValue.Value} {adValue.CurrencyCode}.");
        }

        protected string TypeName() => GetType().Name;


        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        protected void ADLog(object msg )
        {
            Debug.Log(msg); 
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        protected void ADLogError(object msg)
        {
            Debug.LogError(msg);
        }

    }

}