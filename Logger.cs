

namespace FakeLivingComments
{
	public static class Logger
	{
		public enum LogLevel
		{
			Info,
			Warning,
			Error
		}

		public static void Log(LogLevel level, string message)
		{
			switch (level)
			{
				case LogLevel.Info:
					UnityEngine.Debug.Log(FakeLivingComments.MOD_NAME + ":" + message);
					break;
				case LogLevel.Warning:
					UnityEngine.Debug.LogWarning(FakeLivingComments.MOD_NAME + ":" + message);
					break;
				case LogLevel.Error:
					UnityEngine.Debug.LogError(FakeLivingComments.MOD_NAME + ":" + message);
					break;
				default:
					UnityEngine.Debug.Log(FakeLivingComments.MOD_NAME + ":" + message);
					break;
			}
		}
	}
}