using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DeveloperConsole
{

	[HarmonyPatch(typeof(BootUpdate), "Start")]
    internal static class InitializePatch {

        private static void Prefix() {
			GameObject prefab = Addressables.LoadAssetAsync<GameObject>("uConsole").WaitForCompletion();
			UnityEngine.Object.Instantiate(prefab);
            uConsole.m_Instance.m_Activate = KeyCode.F1;
        }
    }
}
