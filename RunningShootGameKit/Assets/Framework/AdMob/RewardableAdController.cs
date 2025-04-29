using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;

namespace UnityFramework.AD
{

	public abstract class RewardableAdController : AdController
	{
        public event System.Action<Reward> OnRewarded;


        protected void ExecuteRewarded(Reward reward)
        {
            ADLog($"Rewarded ad granted a reward: {reward.Amount} {reward.Type}");
            OnRewarded?.Invoke(reward);
        }

    }
}
