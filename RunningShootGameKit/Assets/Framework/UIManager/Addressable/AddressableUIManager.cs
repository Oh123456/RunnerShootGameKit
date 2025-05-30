﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityFramework.Addressable;
using UnityFramework.UI;
using UnityEngine.AddressableAssets;



#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;
#endif


namespace UnityFramework.UI
{
    public partial class UIManager
    {
        private interface IUIAddressalbeHandle
        {
            T GetResource<T>() where T : MainUIBase;
            void Release();
        }
        private class UIAddressalbeHandle<T> : IUIAddressalbeHandle where T : MainUIBase
        {
            readonly AddressableResourceHandle<GameObject> addressableResourceHandle;
            T uiBase;

            public UIAddressalbeHandle(in AddressableResourceHandle<GameObject> addressableResourceHandle)
            {
                this.addressableResourceHandle = addressableResourceHandle;
                uiBase = addressableResourceHandle.GetResource().GetComponent<T>();
            }

            public TUIBase GetResource<TUIBase>() where TUIBase : MainUIBase
            {
                return uiBase as TUIBase;
            }

            public void Release()
            {
                uiBase = null;
                addressableResourceHandle.Release();
            }
        }

        private Dictionary<string, IUIAddressalbeHandle> unsafeLoads = new Dictionary<string, IUIAddressalbeHandle>();

        enum LoadType
        {
            Safe,
            UnSafe,
        }

        public async void ShowAddressableSceneUI<T>(object key, System.Action<T> showComplete = null, int sortOrder = 0) where T : MainUIBase
        {
            T ui = await GetCachedAddressableUI<T>(key, LoadType.Safe);
            ExecuteUIContoller(ui);
            showComplete?.Invoke(ui);
            return;
        }

#if USE_ADDRESSABLE_TASK
        public async Task<T>
#else
        public async UniTask<T>
#endif
        ShowAddressableSceneUI<T>(object key, int sortOrder = 0) where T : MainUIBase
        {
            T ui = await GetCachedAddressableUI<T>(key, LoadType.Safe);
            ExecuteUIContoller(ui);
            return ui;
        }


        public async void ShowAddressableUI<T>(object key, System.Action<T> showComplete = null, int sortOrder = 0) where T : MainUIBase
        {
            T ui = await GetCachedAddressableUI<T>(key, LoadType.UnSafe);
            ExecuteUIContoller(ui);
            showComplete?.Invoke(ui);
        }

#if USE_ADDRESSABLE_TASK
        public async Task<T>
#else
        public async UniTask<T>
#endif
        ShowAddressableUI<T>(object key, int sortOrder = 0) where T : MainUIBase
        {
            T ui = await GetCachedAddressableUI<T>(key, LoadType.UnSafe);
            var uIController = ExecuteUIContoller(ui);
            return ui;
        }

#if USE_ADDRESSABLE_TASK
        public async Task<T>
#else
        public async UniTask<T>
#endif
        ShowAddressableUnmanagedUI<T>(object key, int sortOrder = 0) where T : MainUIBase
        {
            return await GetCachedAddressableUI<T>(key, LoadType.UnSafe);
        }

#if USE_ADDRESSABLE_TASK
        private async Task<T>
#else
        private async UniTask<T>
#endif
        GetCachedAddressableUI<T>(object key, LoadType loadType) where T : MainUIBase
        {
            T ui = null;
            if (!TryGetCachedUI(out ui))
            {
                T prb = await GetPrefab<T>(key, loadType);
                ui = GameObject.Instantiate<T>(prb);
                uis[typeof(T)] = ui;
            }

            return ui;
        }

        private UIController ExecuteUIContoller<T>(T ui) where T : MainUIBase
        {

            UIController uIController = GetUIController();
            uIController.Initialize(ui);
            uIController.Show();
            showUIStack.Push(uIController);

            return uIController;
        }

#if USE_ADDRESSABLE_TASK
        private async Task<T>
#else
        private async UniTask<T>
#endif
        GetPrefab<T>(object key, LoadType loadType) where T : MainUIBase
        {
            if (loadType == LoadType.Safe)
            {
                AddressableResource<GameObject> addressableResource = AddressableManager.Instance.LoadAsset<GameObject>(key);
                await addressableResource.Task;
                GameObject gameObject = addressableResource.GetResource();
                return gameObject.GetComponent<T>();
            }
            else
            {
                string loadKey = (key is IKeyEvaluator evaluator ? evaluator.RuntimeKey : key) as string;
                if (!unsafeLoads.TryGetValue(loadKey, out IUIAddressalbeHandle uiAddressalbeHandle))
                {
                    AddressableResourceHandle<GameObject> addressableResourceHandle = AddressableManager.UnsafeLoadAsset<GameObject>(key);
                    await addressableResourceHandle.Task;
                    uiAddressalbeHandle = new UIAddressalbeHandle<T>(in addressableResourceHandle);
                    if (loadKey != null)
                        unsafeLoads.Add(loadKey, uiAddressalbeHandle);
                }

                return uiAddressalbeHandle.GetResource<T>();
            }
        }


        public void ReleaseUnsafeUI(object key)
        {
            string stringkey = (string)(key is IKeyEvaluator keyEvaluator ? keyEvaluator.RuntimeKey : key);
            if (unsafeLoads.TryGetValue(stringkey, out IUIAddressalbeHandle uIAddressalbeHandle))
            {
                uIAddressalbeHandle.Release();
                unsafeLoads.Remove(stringkey);
            }
        }
    }

}