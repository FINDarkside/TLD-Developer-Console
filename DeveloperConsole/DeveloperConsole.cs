using Il2Cpp;
using Il2CppTLD.AddressableAssets;
using Il2CppTLD.Scenes;
using MelonLoader;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using Il2CppCollection = Il2CppSystem.Collections.Generic;
using Scene = UnityEngine.SceneManagement;

namespace DeveloperConsole {

    internal class DeveloperConsole : MelonMod {

        public override void OnInitializeMelon() {
            FileLog.CreateLogFile();
            AddConsoleCommands();
            Debug.Log($"[{Info.Name}] version {Info.Version} loaded!");
        }

        public override void OnApplicationQuit() {
            FileLog.MaybeLogNullReference();
            base.OnApplicationQuit();
        }

        private static void AddConsoleCommands() {
            uConsole.RegisterCommand("scene_name", new Action(() => uConsoleLog.Add(Scene.SceneManager.GetActiveScene().name)));

            uConsole.RegisterCommand("scene_list", new Action(ListScenes));

            uConsole.RegisterCommand("parameter_list", new Action(ListParameters));

            uConsole.RegisterCommand("pos", new Action(GetPosition));

            uConsole.RegisterCommand("tp", new Action(Teleport));

            uConsole.RegisterCommand("gear_list", new Action(ListGear));

            uConsole.RegisterCommand("gear_search", new Action(SearchGear));
        }

        private static void ListScenes() {
            Il2CppCollection.List<IResourceLocation> scenes = AssetHelper.FindAllAssetsLocations<SceneSet>().Cast<Il2CppCollection.List<IResourceLocation>>();

            List<string> sceneNames = new List<string>(scenes.Count);
            foreach (IResourceLocation sceneResource in scenes) {
                sceneNames.Add(sceneResource.PrimaryKey);
            }
            sceneNames.Sort();

            foreach (string sceneName in sceneNames) {
                uConsoleLog.Add(sceneName);
            }
        }

        private static void ListParameters() {
            if(uConsole.GetNumParameters() != 1) {
                uConsoleLog.Add("'parameter_list' takes exactly one parameter.");
                return;
            }

            string commandName = uConsole.GetString();
            List<string> parameters = new List<string>();
            foreach(var parameterSet in uConsoleAutoComplete.m_CommandParameterSets) {
                if (parameterSet.m_Commands.Contains(commandName)) parameters.AddRange(parameterSet.m_AllowedParameters.ToArray());
            }

            if(parameters.Count == 0) {
                uConsoleLog.Add($"'{commandName}' has no registered parameters to display.");
            } else {
                parameters.Sort();
                uConsoleLog.Add("");
                uConsoleLog.Add("Registered Parameters:");
                foreach (string parameter in parameters){
                    uConsoleLog.Add(parameter);
                }
            }
        }

        private static void GetPosition() {
            Vector3 pos = GameManager.GetVpFPSPlayer().transform.position;
            uConsoleLog.Add($"[{pos.x:F2} / {pos.y:F2} / {pos.z:F2}]");
        }

        private static void Teleport() {
            Vector3 target;

            if (uConsole.GetNumParameters() < 2) {
                uConsoleLog.Add("Usage: tp x z    or    tp x y z.\nExample: tp 123 890");
                return;
            } else if (uConsole.GetNumParameters() == 2) {
                float x = uConsole.GetFloat();
                float z = uConsole.GetFloat();

                Vector3 start = new Vector3(x, 10000f, z);
                if (Physics.Raycast(start, Vector3.down, out RaycastHit raycastHit, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask | 1048576 | 134217728)) {
                    target = raycastHit.point + new Vector3(0, 0.01f, 0);
                } else {
                    target = new Vector3(x, 0, z);
                }
            } else {
                float x = uConsole.GetFloat();
                float y = uConsole.GetFloat();
                float z = uConsole.GetFloat();
                target = new Vector3(x, y, z);
            }

            Quaternion rot = GameManager.GetVpFPSCamera().transform.rotation;
            GameManager.GetPlayerManagerComponent().TeleportPlayer(target, rot);
            GameManager.GetPlayerManagerComponent().StickPlayerToGround();
        }

        private static void ListGear() {
            SortedSet<string> sortedUniqueGear = new SortedSet<string>();
            foreach (string gearName in ConsoleManager.m_SearchStringToGearNames.Values) {
                if (!gearName.StartsWith("GEAR_")) continue;
                sortedUniqueGear.Add(gearName.Substring("GEAR_".Length));
            }

            foreach (string gearName in sortedUniqueGear) {
                uConsoleLog.Add(gearName);
            }
        }

        private static void SearchGear() {
            if (uConsole.GetNumParameters() != 1) {
                uConsoleLog.Add("Usage: search_gear name");
                return;
            }

            Il2CppCollection.Dictionary<string, string> gearNames = ConsoleManager.m_SearchStringToGearNames;
            string term = uConsole.GetString().ToLowerInvariant();
            SortedSet<string> results = new SortedSet<string>();

            foreach (Il2CppCollection.KeyValuePair<string, string> entry in gearNames) {
                if (!entry.Value.StartsWith("GEAR_")) continue;
                string value = entry.Value.Substring("GEAR_".Length);
                string key = entry.Key.StartsWith("gear_") ? entry.Key.Substring("gear_".Length) : entry.Key;

                if (key.ToLowerInvariant().Contains(term) || value.ToLowerInvariant().Contains(term)) {
                    results.Add(value);
                }
            }

            if (results.Count > 0) {
                foreach (string result in results) {
                    uConsole.Log(result);
                }
            } else {
                uConsoleLog.Add("No gear names containing '" + term + "' found");
            }
        }
    }
}
