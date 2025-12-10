using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using FakeLivingComments.Config;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FakeLivingComments
{
	/// <summary>
	/// FakeLivingComments主类
	/// </summary>
	public static class FakeLivingComments
	{
		/// <summary>
		/// MOD人类易读名称，通用于大多数场合
		/// </summary>
		public const string MOD_NAME = "FakeLivingComments";
		/// <summary>
		/// MOD的配置文件目录名
		/// </summary>
		public const string MOD_CONFIG_DIR = "BCASoft.FakeLivingComments";
		/// <summary>
		/// 本mod使用的Harmony实例
		/// </summary>
		public static Harmony HarmonyInstance = new Harmony(MOD_NAME);
		/// <summary>
		/// 记录当前存在的所有弹幕实例对象
		/// </summary>
		public static List<RealtimeComment> RealtimeComments = new List<RealtimeComment>();
		public static RectTransform? UITransform;
		/// <summary>
		/// 预备弹幕队列
		/// </summary>
		public static ConcurrentQueue<RealtimeCommentReserve> RealtimeCommentReserves = new ConcurrentQueue<RealtimeCommentReserve>();
		/// <summary>
		/// 本mod的初始化，在加载时调用
		/// </summary>
		public static void Init()
		{
			if (ConfigHolder.ReadFromFile())
			{
				RealtimeComments.Capacity = ConfigHolder.ConfigData.CommentMaxCount;
				SendANewComment("已加载配置文件");
			}
			else if (!File.Exists(ConfigHolder.ConfigFilePath))
			{
				RealtimeComments.Capacity = ConfigHolder.ConfigData.CommentMaxCount;
				SendANewComment("未能加载配置文件，配置文件不存在");
			}
			else
			{
				RealtimeComments.Capacity = ConfigHolder.ConfigData.CommentMaxCount;
				SendANewComment("加载配置文件时发生问题，配置文件内容未空或读取错误");
			}
			if (ConfigHolder.SaveToFile()) ReserveANewComment("已写入配置文件", Time.time + 1f);
			else ReserveANewComment("写入配置文件时发生问题", Time.time + 1f);
			CreateUI();
			SignalTriggerHandler.Load();
			LoadResult loadResult = FactoryManager.LoadData();
			switch (loadResult)
			{
				case LoadResult.NO_DATA:
					ReserveANewComment("未找到可供加载的弹幕内容数据，除非装有专门的外部模组直接调用本模组，否则本模组可能不会出现任何弹幕", Time.time + 2f);
					break;
				case LoadResult.SUCCESS_PARTLY:
					ReserveANewComment("部分弹幕内容数据加载失败，可从游戏日志中查看详细错误信息", Time.time + 2f);
					break;
				case LoadResult.SUCCESS_ALL:
					ReserveANewComment("所有弹幕内容数据均加载成功", Time.time + 2f);
					break;
				case LoadResult.FAILURE_ALL:
					ReserveANewComment("所有弹幕内容数据均加载失败，可从游戏日志中查看详细错误信息", Time.time + 2f);
					break;
			}
			ReserveANewComment("全部加载流程完毕", Time.time + 4f);
		}
		/// <summary>
		/// 取消加载本mod
		/// </summary>
		public static void Unload()
		{
			DestroyUI();
			SignalTriggerHandler.Unload();
			FactoryManager.FactoryPipelineStop();
			HarmonyInstance.UnpatchAll(MOD_NAME);
		}
		public static void Update()
		{
			// 弹幕对象更新与回收引用
			for (int i = RealtimeComments.Count - 1; i >= 0; i--)
			{
				RealtimeComment thisRealtimeComment = RealtimeComments[i];
				thisRealtimeComment.Update();
				if (!thisRealtimeComment.IsAlive) RealtimeComments.RemoveAt(i);
			}
			// 新弹幕对象发送
			for (int tryCounter = RealtimeCommentReserves.Count; tryCounter > 0; tryCounter--)
			{
				if (RealtimeCommentReserves.TryDequeue(out RealtimeCommentReserve commentReserve))
				{
					if (Time.time >= commentReserve.SendTime) SendANewComment(commentReserve.Text);
					else RealtimeCommentReserves.Enqueue(commentReserve);
				}
			}
		}
		/// <summary>
		/// 添加一条预备弹幕，不可在调试时手动调用。可能因当前存在的预备弹幕数量达到上限而被丢弃，届时本方法会返回false
		/// </summary>
		/// <param name="text">要发送的文本</param>
		/// <param name="sendTime">预定弹幕发送的绝对时间(不是相对倒计时)，绝对时间使用UnityEngine.Time.time比较</param>
		/// <returns>弹幕是否成功添加到预备</returns>
		public static bool ReserveANewComment(string text, float sendTime)
		{
			if (RealtimeCommentReserves.Count >= ConfigHolder.ConfigData.ReserveMaxCount) return false;
			RealtimeCommentReserves.Enqueue(new RealtimeCommentReserve(text, sendTime));
			return true;
		}
		/// <summary>
		/// 立即发送出一条弹幕，可在调试时手动调用。可能因当前存在的弹幕数量达到上限或UI不存在而被丢弃，届时本方法会返回false
		/// </summary>
		/// <param name="text">要发送的文本</param>
		/// <returns>弹幕是否成功发送</returns>
		public static bool SendANewComment(string text)
		{
			if (RealtimeComments.Count >= ConfigHolder.ConfigData.CommentMaxCount || UITransform == null) return false;
			RealtimeComments.Add(new RealtimeComment(text, UITransform));
			return true;
		}
		/// <summary>
		/// 创建UI
		/// </summary>
		public static void CreateUI()
		{
			if (UITransform != null) return;
			GameObject uiGameObject = new GameObject("FakeLivingCommentsUI")
			{
				layer = LayerMask.NameToLayer("UI")
			};
			Object.DontDestroyOnLoad(uiGameObject);
			uiGameObject.hideFlags = HideFlags.HideAndDontSave;
			UITransform = uiGameObject.AddComponent<RectTransform>();
			Canvas canvas = uiGameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 28000;
		}
		/// <summary>
		/// 销毁UI
		/// </summary>
		public static void DestroyUI()
		{
			if (UITransform == null) return;
			Object.Destroy(UITransform.gameObject);
		}
	}
}
