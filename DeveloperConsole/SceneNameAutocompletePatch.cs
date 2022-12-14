using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.AddressableAssets;
using Il2CppTLD.Scenes;
using UnityEngine.ResourceManagement.ResourceLocations;
using Il2CppCollection = Il2CppSystem.Collections.Generic;

namespace DeveloperConsole {

    [HarmonyPatch(typeof(BootUpdate), "Start")]
    internal static class SceneNameAutocompletePatch {

        private static void Postfix() {
            Il2CppCollection.List<IResourceLocation> scenes = AssetHelper.FindAllAssetsLocations<SceneSet>().Cast<Il2CppCollection.List<IResourceLocation>>();
            Il2CppCollection.List<string> sceneParamaters = new Il2CppCollection.List<string>();

            foreach (IResourceLocation sceneResource in scenes) {
                sceneParamaters.Add(sceneResource.PrimaryKey);
                sceneParamaters.Add(sceneResource.PrimaryKey.ToLowerInvariant());
            }
            sceneParamaters.Sort();

            uConsoleAutoComplete.CreateCommandParameterSet("scene", sceneParamaters);
        }
    }
}
