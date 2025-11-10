using System;
using System.IO;
using UnityEngine;

namespace FakeLivingComments.Config
{
	public static class ConfigHolder
	{
		public static string ConfigFilePath
		{
			get
			{
				string? dir = Path.GetDirectoryName(Application.dataPath);
				return dir == null ? "" : Path.Combine(dir, "ModConfigs", FakeLivingComments.MOD_CONFIG_DIR, "config.json");
			}
		}
		public static ConfigStruct ConfigData = MakeDefault();

		public static bool ReadFromFile()
		{
			if (!File.Exists(ConfigFilePath))
			{
				Logger.Log(Logger.LogLevel.Warning, "配置文件不存在");
				return false;
			}
			string configContent = File.ReadAllText(ConfigFilePath);
			if (string.IsNullOrEmpty(configContent))
			{
				Logger.Log(Logger.LogLevel.Error, "配置文件内容未空或读取错误");
				return false;
			}
			ConfigData = JsonUtility.FromJson<ConfigStruct>(configContent);
			return true;
		}

		public static bool SaveToFile()
		{
			try
			{
				string json = JsonUtility.ToJson(ConfigData, true);
				Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath)!);
				File.WriteAllText(ConfigFilePath, json);
			}
			catch (Exception)
			{
				Logger.Log(Logger.LogLevel.Error, "保存配置文件时发生问题");
				throw;
			}
			return true;
		}
		
		public static ConfigStruct MakeDefault()
		{
			ConfigStruct result = new ConfigStruct
			{
				CommentStaySeconds = 10.0f,
				CommentAlpha = 0.5f,
				CommentFontSize = 10.0f,
				CommentMaxCount = 30,
				CommentLowestHeight = 0.3f,
			};
			return result;
		}
	}
}
