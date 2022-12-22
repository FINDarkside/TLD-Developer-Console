using HarmonyLib;
using Il2Cpp;

namespace DeveloperConsole {

    [HarmonyPatch(typeof(uConsoleGUI), "InputFieldClearText")]
    internal static class ScrollDownOnCommandPatch {

        private static void Postfix() {
            uConsole.m_GUI.ScrollLogDownMax();
        }
    }

    [HarmonyPatch(typeof(uConsoleAutoComplete), "DisplayStringsStartingWithMatch")]
    internal static class ScrollDownOnAutoCompletePatch {

        private static void Postfix() {
            uConsole.m_GUI.ScrollLogDownMax();
        }
    }
}
