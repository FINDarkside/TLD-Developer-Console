using UnityEngine;
using Scene = UnityEngine.SceneManagement;

namespace DeveloperConsole
{

    internal class DeveloperConsole
    {

        public static void OnLoad()
        {
            Object.Instantiate(Resources.Load("uConsole"));
            uConsole.m_Instance.m_Activate = KeyCode.F1;
            AddConsoleCommands();
        }

        private static void AddConsoleCommands()
        {
            uConsole.RegisterCommand("load_scene", LoadScene);

            uConsole.RegisterCommand("fly", Fly);

            uConsole.RegisterCommand("save", () => GameManager.m_PendingSave = true);

            uConsole.RegisterCommand("scene_name", () => Debug.Log(Scene.SceneManager.GetActiveScene().name));

            uConsole.RegisterCommand("scene_index", () => Debug.Log(Scene.SceneManager.GetActiveScene().buildIndex));

            uConsole.RegisterCommand("pos", GetPosition);

            uConsole.RegisterCommand("tp", Teleport);
        }

        private static void LoadScene()
        {
            var ind = uConsole.GetInt();
            SceneManager.LoadScene(ind);
        }

        private static void Fly()
        {
            bool fly = !FlyMode.m_Enabled;
            if (uConsole.GetNumParameters() > 0 && uConsole.NextParameterIsBool())
                fly = uConsole.GetBool();
            if (fly == FlyMode.m_Enabled)
                return;
            if (fly)
                FlyMode.Enter();
            else
                FlyMode.TeleportPlayerAndExit();
        }

        private static void GetPosition()
        {
            Vector3 pos = GameManager.GetVpFPSPlayer().transform.position;
            Debug.LogFormat("[{0:F2} / {1:F2} / {2:F2}]", pos.x, pos.y, pos.z);
        }

        private static void Teleport()
        {
            Vector3 target;

            if (uConsole.GetNumParameters() < 2)
            {
                Debug.Log("Usage: tp x z    or    tp x y z.\nExample: tp 123 890");
                return;
            }
            else if (uConsole.GetNumParameters() == 2)
            {
                float x = uConsole.GetFloat();
                float z = uConsole.GetFloat();

                Vector3 start = new Vector3(x, 10000f, z);
                if (Physics.Raycast(start, Vector3.down, out RaycastHit raycastHit, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask | 1048576 | 134217728))
                {
                    target = raycastHit.point + new Vector3(0, 0.01f, 0);
                }
                else
                {
                    target = new Vector3(x, 0, z);
                }
            }
            else
            {
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
