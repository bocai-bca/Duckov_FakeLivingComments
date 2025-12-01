using System.Collections.Generic;
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
		/// 
		/// </summary>
		public static List<RealtimeCommentReserve> RealtimeCommentReserves
		{
			get
			{
				lock (realtimeCommentReserves)
				{
					return realtimeCommentReserves;
				}
			}
			set
			{
				lock (realtimeCommentReserves)
				{
					realtimeCommentReserves = value;
				}
			}
		}
		private static List<RealtimeCommentReserve> realtimeCommentReserves = new List<RealtimeCommentReserve>();
		
		/// <summary>
		/// 本mod的初始化，在加载时调用
		/// </summary>
		public static void Init()
		{
			if (ConfigHolder.ReadFromFile())
			{
				RealtimeComments.Capacity = ConfigHolder.ConfigData.CommentMaxCount;
				RealtimeCommentReserves.Capacity = ConfigHolder.ConfigData.CommentMaxCount;
				ReserveANewComment("已加载配置文件", Time.time + 2f);
			}
			ConfigHolder.SaveToFile();
			CreateUI();
			SignalTriggerHandler.Load();
			ReserveANewComment("假弹幕模组已加载完毕", Time.time + 3f);
		}
		/// <summary>
		/// 取消加载本mod
		/// </summary>
		public static void Unload()
		{
			DestroyUI();
			SignalTriggerHandler.Unload();
			HarmonyInstance.UnpatchAll(MOD_NAME);
		}
		public static void Update()
		{
			// 弹幕对象更新与回收引用
			for (int i = RealtimeComments.Count - 1; i >= 0; i--)
			{
				RealtimeComment thisRealtimeComment = RealtimeComments[i];
				thisRealtimeComment.Update();
				if (!thisRealtimeComment.IsAlive)
				{
					RealtimeComments.RemoveAt(i);
				}
			}
			// 新弹幕对象发送
			for (int i = RealtimeCommentReserves.Count - 1; i >= 0; i--)
			{
				if (Time.time >= RealtimeCommentReserves[i].SendTime)
				{
					SendANewComment(RealtimeCommentReserves[i].Text);
					RealtimeCommentReserves.RemoveAt(i);
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
			if (RealtimeCommentReserves.Count >= ConfigHolder.ConfigData.ReserveMaxCount)
			{
				return false;
			}
			RealtimeCommentReserves.Add(new RealtimeCommentReserve(text, sendTime));
			return true;
		}
		/// <summary>
		/// 立即发送出一条弹幕，可在调试时手动调用。可能因当前存在的弹幕数量达到上限或UI不存在而被丢弃，届时本方法会返回false
		/// </summary>
		/// <param name="text">要发送的文本</param>
		/// <returns>弹幕是否成功发送</returns>
		public static bool SendANewComment(string text)
		{
			if (RealtimeComments.Count >= ConfigHolder.ConfigData.CommentMaxCount || UITransform == null)
			{
				return false;
			}
			RealtimeComments.Add(new RealtimeComment(text, UITransform));
			return true;
		}
		/// <summary>
		/// 创建UI
		/// </summary>
		public static void CreateUI()
		{
			if (UITransform != null)
			{
				return;
			}
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
			if (UITransform == null)
			{
				return;
			}
			Object.Destroy(UITransform.gameObject);
		}
	}
}
