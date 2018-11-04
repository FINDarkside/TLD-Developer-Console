using Harmony;

namespace DeveloperConsole
{

    [HarmonyPatch(typeof(uConsole), "RunCommand")]
    internal static class ScrollDownOnCommandPatch
    {
        private static void Postfix()
        {
            uConsole.m_GUI.ScrollLogDownMax();
        }
    }
}
