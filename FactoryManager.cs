using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FakeLivingComments.Config;
using FakeLivingComments.Factory;
using UnityEngine;
using Random = UnityEngine.Random;

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
				if (Path.GetExtension(current_path) != ".json") continue;
				string fileContent = File.ReadAllText(current_path);
				FactoryData data = JsonUtility.FromJson<FactoryData>(fileContent);
				if (data != null) FactoryDataLoaded.Merge(data);
			}
			SignalToFilters.Clear(); //清空信号to过滤器列表
			foreach (string filtersUID in FactoryDataLoaded.filters.Keys) //按键名(过滤器UID)遍历所有过滤器
			{
				Filter thisFilter = FactoryDataLoaded.filters[filtersUID]; //缓存当前遍历到达的过滤器
				foreach (string filterScribedTriggerUID in thisFilter.scribe_triggers) //遍历该过滤器订阅的所有触发器
				{
					Trigger thisTrigger = FactoryDataLoaded.triggers[filterScribedTriggerUID]; //缓存当前遍历当前遍历到达的过滤器到达的触发器
					if (thisTrigger.type != TriggerType.Signal) continue;
					if (SignalToFilters.ContainsKey(filterScribedTriggerUID)) SignalToFilters[filterScribedTriggerUID].Add(filtersUID); //如果信号to过滤器列表含有当前遍历到的被订阅触发器，将当前遍历到的过滤器UID添加到信号to过滤器
					else SignalToFilters.Add(filterScribedTriggerUID, new List<string> { filtersUID }); //否则新建值并记录当前过滤器UID
				}
			}
			return true;
		}
		/// <summary>
		/// 广播一个触发器信号，允许外部调用以触发一个触发器
		/// </summary>
		/// <param name="signal">信号名</param>
		public static void EmitTriggerSignal(string signal)
		{
			try
			{
				if (FactoryDataLoaded == null || FactoryPipelineThread == null) return;
				if (SignalToFilters.TryGetValue(signal, out List<string>? filterUIDs))
				{
					foreach (string filterUID in filterUIDs)
					{
						if (FactoryDataLoaded.filters.TryGetValue(filterUID, out Filter? filter)) factoryFilterTaskQueue.Enqueue(filter);
					}
					if (!FactoryPipelineThread.IsAlive) FactoryPipelineStart();
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
				Thread.Sleep(32);
				if (!factoryFilterTaskQueue.TryDequeue(out Filter currentFilterTask)) break;
				FactoryPipeline_ExecuteFilter(currentFilterTask);
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
		private static void FactoryPipeline_ExecuteFilter(Filter filter)
		{
			int commandPointer = 0; //声明局部变量存储命令指针
			bool lastIfResult = false; //记录上一次if命令结果
			for (int executionTTL = ConfigHolder.ConfigData.FilterExecutionTTL; executionTTL > 0; executionTTL--) //启动循环
			{
				if (commandPointer >= filter.commands.Length) break; //如果当前命令指针超出命令条数，就结束过滤器执行
				string[] currentCommand = filter.commands[commandPointer].Split(' '); //取得当前命令并分段
				switch (currentCommand[0]) //匹配检查主命令
				{
					case "ret":
						if (currentCommand.Length > 1)
						{
							switch (currentCommand[1])
							{
								case "if":
									if (lastIfResult) goto ExecutionReturn;
									break;
								case "else":
									if (!lastIfResult) goto ExecutionReturn;
									break;
								default:
									Debug.LogError("过滤器命令出错-未知的子命令，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
									return;
							}
						}
						else goto ExecutionReturn;
						break;
					case "goto":
						if (currentCommand.Length < 2)
						{
							Debug.LogError("过滤器命令出错-段落缺失，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
							return;
						}
						if (int.TryParse(currentCommand[1], out int gotoLine))
						{
							if (currentCommand.Length == 3)
							{
								switch (currentCommand[2])
								{
									case "if":
										if (lastIfResult) commandPointer = gotoLine;
										break;
									case "else":
										if (!lastIfResult) commandPointer = gotoLine;
										break;
									default:
										Debug.LogError("过滤器命令出错-未知的子命令，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
										return;
								}
							}
							else if (currentCommand.Length > 3)
							{
								Debug.LogError("过滤器命令出错-未知的额外段落，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
								return;
							}
							break;
						}
						Debug.LogError("过滤器命令出错-数字参数解析失败，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
						return;
					case "if":
						if (currentCommand.Length <= 2)
						{
							Debug.LogError("过滤器命令出错-段落缺失，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
							return;
						}
						switch (currentCommand[2])
						{
							case "rdb":
								if (currentCommand.Length == 4)
								{
									if (float.TryParse(currentCommand[3], out float chance))
									{
										float random = Random.Range(0f, 1f);
										if (random <= chance && random != 0) lastIfResult = true;
										else lastIfResult = false;
									}
									else
									{
										Debug.LogError("过滤器命令出错-数字参数解析失败，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
										return;
									}
								}
								else if (currentCommand.Length > 4)
								{
									Debug.LogError("过滤器命令出错-未知的额外段落，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
									return;
								}
								else
								{
									Debug.LogError("过滤器命令出错-段落缺失，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
									return;
								}
								break;
							case "call":
								if (currentCommand.Length == 4)
								{
									
								}
								else if (currentCommand.Length > 4)
								{
									Debug.LogError("过滤器命令出错-未知的额外段落，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
									return;
								}
								else
								{
									Debug.LogError("过滤器命令出错-段落缺失，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
									return;
								}
								break;
							default:
								Debug.LogError("过滤器命令出错-未知的子命令，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
								return;
						}
						break;
					case "":
						break;
					default:
						Debug.LogError("未知的过滤器命令，行号=" + commandPointer + "，内容=" + filter.commands[commandPointer]);
						return;
				}
				commandPointer++;
			}
			ExecutionReturn: ;
		}
	}
}