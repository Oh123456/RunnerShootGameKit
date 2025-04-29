using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;

namespace UnityFramework.AD
{
    public struct AdOptions
    {
        public AdSize adSize;
        public AdPosition adPosition;
    }

    public interface IAdController
    {
        void Load();
        void Show();
        void Hide();
        void Destroy();
        void LogResponseInfo();
        bool IsLoaded();
        void Create(in System.Nullable<AdOptions> adOptions = null);
    }

}