using System;
using System.IO;
using UnityEngine;

namespace FakeLivingComments.Config
{
	/// <summary>
	/// 配置持有器，也负责读写配置文件
	/// </summary>
	public static class ConfigHolder
	{
		/// <summary>
		/// 配置文件路径
		/// </summary>
		public static string ConfigFilePath
		{
			get
			{
				string? dir = Path.GetDirectoryName(Application.dataPath);
				return dir == null ? "" : Path.Combine(dir, "ModConfigs", FakeLivingComments.MOD_CONFIG_DIR, "config.json");
			}
		}
		public static ConfigStruct ConfigData = MakeDefault();
		/// <summary>
		/// 从文件读取配置
		/// </summary>
		/// <returns>成功与否</returns>
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
		/// <summary>
		/// 将配置保存到文件
		/// </summary>
		/// <returns>成功与否</returns>
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
		/// <summary>
		/// 创建并返回一个包含默认值的配置结构体实例
		/// </summary>
		/// <returns>包含默认值的配置结构体实例</returns>
		public static ConfigStruct MakeDefault()
		{
			ConfigStruct result = new ConfigStruct
			{
				CommentStaySeconds = 10.0f,
				CommentAlpha = 0.5f,
				CommentFontSizeMulti = 0.04f,
				CommentMaxCount = 30,
				ReserveMaxCount = 15,
				CommentLowestHeight = 0.3f,
				CommentOutlineWidth = 0.1f,
			};
			return result;
		}
	}
}
