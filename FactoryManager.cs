using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
		/// 按信号名作为键记录所有订阅该信号的触发器的过滤器的UID    
		/// </summary>
		public static Dictionary<string, List<string>> SignalToFilters = new Dictionary<string, List<string>>();
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
		/// 线程安全队列，用于存储工厂管线已被触发的过滤器任务
		/// </summary>
		private static ConcurrentQueue<Filter> factoryFilterTaskQueue = new ConcurrentQueue<Filter>();
		/// <summary>
		/// 工厂管线工作线程
		/// </summary>
		public static Thread? FactoryPipelineThread;
		/// <summary>
		/// 加载并合并数据至内存
		/// </summary>
		/// <returns>成功与否</returns>
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
			SignalToFilters.Clear(); //清空信号to过滤器列表
			foreach (string filtersUID in FactoryDataLoaded.filters.Keys) //按键名(过滤器UID)遍历所有过滤器
			{
				Filter thisFilter = FactoryDataLoaded.filters[filtersUID]; //缓存当前遍历到达的过滤器
				foreach (string filterScribedTriggerUID in thisFilter.scribe_triggers) //遍历该过滤器订阅的所有触发器
				{
					Trigger thisTrigger = FactoryDataLoaded.triggers[filterScribedTriggerUID]; //缓存当前遍历当前遍历到达的过滤器到达的触发器
					if (thisTrigger.type != TriggerType.Signal)
					{
						continue;
					}
					if (SignalToFilters.ContainsKey(filterScribedTriggerUID)) //如果信号to过滤器列表含有当前遍历到的被订阅触发器
					{
						SignalToFilters[filterScribedTriggerUID].Add(filtersUID); //将当前遍历到的过滤器UID添加到信号to过滤器
					}
					else
					{
						SignalToFilters.Add(filterScribedTriggerUID, new List<string> { filtersUID }); //新建值并记录当前过滤器UID
					}
				}
			}
			return true;
		}
		/// <summary>
		/// 广播一个触发器信号
		/// </summary>
		/// <param name="signal">信号名</param>
		public static void EmitTriggerSignal(string signal)
		{
			try
			{
				if (FactoryDataLoaded == null || FactoryPipelineThread == null)
				{
					return;
				}
				if (SignalToFilters.TryGetValue(signal, out List<string>? filterUIDs))
				{
					foreach (string filterUID in filterUIDs)
					{
						if (FactoryDataLoaded.filters.TryGetValue(filterUID, out Filter? filter))
						{
							factoryFilterTaskQueue.Enqueue(filter);
						}
					}
					if (!FactoryPipelineThread.IsAlive)
					{
						FactoryPipelineStart();
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		/// <summary>
		/// 用于多线程启动的工厂管线
		/// </summary>
		internal static void FactoryPipelineStart()
		{
			if (FactoryPipelineThread != null && FactoryPipelineThread.IsAlive)
			{
				Debug.LogError("工厂线程已在运作，无法重复启动工厂管线");
				return;
			}
			FactoryPipelineThread = new Thread(FactoryPipelineLoop)
			{
				IsBackground = true,
				Name = "FakeLivingComments.FactoryPipelineThread"
			};
			FactoryPipelineThread.Start();
		}
		/// <summary>
		/// 工厂管线工作流程
		/// </summary>
		private static void FactoryPipelineLoop()
		{
			while (!factoryFilterTaskQueue.IsEmpty)
			{
				Thread.Sleep(100);
				
			}
		}
		/// <summary>
		/// 中止工厂管线工作线程，调用时会一并清空过滤器队列，除非发生故障
		/// </summary>
		internal static void FactoryPipelineStop()
		{
			if (FactoryPipelineThread != null && FactoryPipelineThread.IsAlive)
			{
				if (!FactoryPipelineThread.Join(10000))
				{
					Debug.LogError("工厂管线工作线程合并失败或超时");
				}
				else
				{
					FactoryPipelineThread = null;
					factoryFilterTaskQueue.Clear();
				}
			}
		}
		/// <summary>
		/// 工厂管线方法-执行过滤器
		/// </summary>
		/// <param name="filter">要被执行的过滤器</param>
		/// <returns>该过滤器的通过与否</returns>
		private static bool FactoryPipeline_ExecuteFilter(Filter filter)
		{
			
		}
	}
}