using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DeveloperConsole {

    [HarmonyPatch(typeof(BootUpdate), "Start")]
    internal static class InitializePatch {

        private static void Prefix() {
            AsyncOperationHandle<GameObject> prefabLoadHandle = AssetHelper.SafeLoadAssetAsync<GameObject>("uConsole");
            prefabLoadHandle.WaitForCompletion();

            GameObject prefab = prefabLoadHandle.Result;
            Object.Instantiate(prefab);
            uConsole.m_Instance.m_Activate = KeyCode.F1;
        }
    }
}
