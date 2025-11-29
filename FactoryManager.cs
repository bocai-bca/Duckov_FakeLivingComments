using System.IO;
using FakeLivingComments.Factory;
using UnityEngine;

namespace FakeLivingComments
{
	/// <summary>
	/// 工厂管理器
	/// </summary>
	public static class FactoryManager
	{
		/// <summary>
		/// 数据文件路径
		/// </summary>
		public static string DataDirPath
		{
			get
			{
				string? dir = Path.GetDirectoryName(Application.dataPath);
				return dir == null ? "" : Path.Combine(dir, "ModConfigs", FakeLivingComments.MOD_CONFIG_DIR, "Data");
			}
		}
		/// <summary>
		/// 内存中已合并的所有加载的工厂数据
		/// </summary>
		public static FactoryData? FactoryDataLoaded;
		/// <summary>
		/// 加载并合并数据至内存
		/// </summary>
		/// <returns></returns>
		public static bool LoadData()
		{
			FactoryDataLoaded = new FactoryData();
			foreach (string current_path in Directory.GetFiles(DataDirPath))
			{
				if (Path.GetExtension(current_path) != ".json")
				{
					continue;
				}
				string fileContent = File.ReadAllText(current_path);
				FactoryData data = JsonUtility.FromJson<FactoryData>(fileContent);
				if (data != null)
				{
					FactoryDataLoaded.Merge(data);
				}
			}
			return true;
		}
	}
}