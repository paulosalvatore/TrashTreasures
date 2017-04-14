#if UNITY_EDITOR
// allows to use internal methods from the editor code (Prefs editor window)
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Assembly-CSharp-Editor-firstpass")] // thx Daniele! ;)
#endif

namespace CodeStage.AntiCheat.Common
{
	internal class Constants
	{
		internal const string LOG_PREFIX = "[ACTk] ";
	}
}