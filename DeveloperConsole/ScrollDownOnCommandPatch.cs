using Harmony;

namespace DeveloperConsole {

    [HarmonyPatch(typeof(uConsoleGUI), "InputFieldClearText")]
    internal static class ScrollDownOnCommandPatch {

        private static void Postfix() {
            uConsole.m_GUI.ScrollLogDownMax();
        }
    }
}
