using UnityEngine;
using Scene = UnityEngine.SceneManagement;
using MelonLoader;
using System;
using Il2CppStringList = Il2CppSystem.Collections.Generic.List<string>;
using StringList = System.Collections.Generic.List<string>;

namespace DeveloperConsole {

    internal class DeveloperConsole : MelonMod {

        public override void OnApplicationStart() {
            FileLog.CreateLogFile();
            AddConsoleCommands();
            AddSceneParameters();
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
            int sceneCount = Scene.SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; ++i) {
                string path = Scene.SceneUtility.GetScenePathByBuildIndex(i);
                uConsoleLog.Add(i + ": " + PathToSceneName(path));
            }
        }

        private static void ListParameters() {
            if(uConsole.GetNumParameters() != 1) {
                uConsoleLog.Add("'parameter_list' takes exactly one parameter.");
                return;
            }

            string commandName = uConsole.GetString();
            StringList parameters = new StringList();
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

        internal static void AddSceneParameters() {
            Il2CppStringList sceneParamaters = new Il2CppStringList();
            StringList forbiddenScenes = new StringList() { "<null>" , "Empty", "Boot", "MainMenu" , "Ep3OpeningCine" };

            int sceneCount = Scene.SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; ++i) {
                string path = PathToSceneName(Scene.SceneUtility.GetScenePathByBuildIndex(i));
                if (forbiddenScenes.Contains(path)) continue;
                if (path.Contains("_")) continue;
                sceneParamaters.Add(path.ToLower());
                sceneParamaters.Add(path);
            }
            sceneParamaters.Sort();
            uConsoleAutoComplete.CreateCommandParameterSet("scene", sceneParamaters);
        }

        static string PathToSceneName(string path) {
            if (string.IsNullOrEmpty(path)) return "<null>";
            path = path.Substring(path.LastIndexOf("/") + 1);
            path = path.Remove(path.Length - ".unity".Length);
            return path;
        }

        private static void GetPosition() {
            Vector3 pos = GameManager.GetVpFPSPlayer().transform.position;
            uConsoleLog.Add(string.Format("[{0:F2} / {1:F2} / {2:F2}]", pos.x, pos.y, pos.z));
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
            foreach (string gearName in GearNames.m_Names) {
                if (!gearName.StartsWith("gear_")) continue;
                uConsoleLog.Add(gearName.Substring("gear_".Length));
            }
        }

        private static void SearchGear() {
            if (uConsole.GetNumParameters() != 1) {
                uConsoleLog.Add("Usage: search_gear name");
                return;
            }

            string term = uConsole.GetString().ToLowerInvariant();
            bool foundAny = false;

            foreach (string gearName in GearNames.m_Names) {
                if (!gearName.StartsWith("gear_")) continue;
                string baseName = gearName.Substring("gear_".Length);

                if (baseName.ToLowerInvariant().Contains(term)) {
                    foundAny = true;
                    uConsoleLog.Add(baseName);
                }
            }

            if (!foundAny) {
                uConsoleLog.Add("No gear names containing '" + term + "' found");
            }
        }
    }
}
