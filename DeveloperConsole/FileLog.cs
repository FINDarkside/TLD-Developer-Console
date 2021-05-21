using Harmony;
using MelonLoader;
using System.IO;

namespace DeveloperConsole {
	internal static class FileLog {
		private static int numNullReference = 0;
		private const string NULL_REFERENCE_TEXT = "NullReferenceException: Object reference not set to an instance of an object.";
		internal const string FILE_NAME = "DeveloperConsole.log";

		private static string GetFilePath() => Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\Mods\" + FILE_NAME);

		internal static void CreateLogFile() => File.Create(GetFilePath());

		private static void Log(string text) {
			if (text is null) return;
			if (text == NULL_REFERENCE_TEXT) {
				numNullReference++;
				return;
			} else {
				MaybeLogNullReference();
				File.AppendAllLines(GetFilePath(), new string[] { text });
			}
		}

		internal static void MaybeLogNullReference() {
			if (numNullReference > 0) {
				File.AppendAllLines(GetFilePath(), new string[] { "(" + numNullReference + ") " + NULL_REFERENCE_TEXT });
				numNullReference = 0;
			}
		}

		[HarmonyPatch(typeof(uConsoleLog), "Add")]
		internal class UConsoleLog_Add {
			private static void Postfix(string text) {
				Log(text);
			}
		}
	}
}
