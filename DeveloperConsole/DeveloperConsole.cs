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

            uConsole.RegisterCommand("currentSceneName", () => Debug.Log(Scene.SceneManager.GetActiveScene().name));

            uConsole.RegisterCommand("currentSceneIndex", () => Debug.Log(Scene.SceneManager.GetActiveScene().buildIndex));
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
    }
}
