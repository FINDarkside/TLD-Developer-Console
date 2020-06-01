using System.Reflection;
using UnityEngine;
using Scene = UnityEngine.SceneManagement;
using Harmony;
using MelonLoader;

namespace DeveloperConsole {

    internal class DeveloperConsole : MelonMod {

        public override void OnApplicationStart() {
            AddConsoleCommands();
            HarmonyInstance.Create(InfoAttribute.Name).PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static void AddConsoleCommands() {
            uConsole.RegisterCommand("scene_name", new System.Action(() => uConsoleLog.Add(Scene.SceneManager.GetActiveScene().name)));

            uConsole.RegisterCommand("pos", new System.Action(GetPosition));

            uConsole.RegisterCommand("tp", new System.Action(Teleport));
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
    }
}
