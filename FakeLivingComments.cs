using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FakeLivingComments
{
	/// <summary>
	/// FakeLivingComments主类
	/// </summary>
	public static class FakeLivingComments
	{
		public const string MOD_NAME = "FakeLivingComments";
		public const string MOD_CONFIG_DIR = "BCASoft.FakeLivingComments";
		public static List<RealtimeComment> RealtimeComments = new List<RealtimeComment>();
		public static RectTransform? UITransform;
		/// <summary>
		/// 本mod的初始化，在加载时调用
		/// </summary>
		public static void Init()
		{
			CreateUI();
			SendANewComment("直播弹幕模拟已加载");
		}
		/// <summary>
		/// 取消加载本mod
		/// </summary>
		public static void Unload()
		{
			DestroyUI();
		}
		public static void Update()
		{
			List<int> freeIndexList = new List<int>();
			for (int i = 0; i < RealtimeComments.Count; i++)
			{
				RealtimeComment thisRealtimeComment = RealtimeComments[i];
				thisRealtimeComment.Update();
				if (!thisRealtimeComment.IsAlive)
				{
					freeIndexList.Add(i);
				}
			}
			for (int freeIndex = freeIndexList.Count - 1; freeIndex >= 0; freeIndex--)
			{
				RealtimeComments.RemoveAt(freeIndexList[freeIndex]);
			}
		}
		/// <summary>
		/// 发送出一条弹幕，可在调试时手动调用。可能因当前存在的弹幕数量达到上限或UI不存在而被丢弃，届时本方法会返回false
		/// </summary>
		/// <param name="text">要发送的文本</param>
		/// <returns>弹幕是否成功发送</returns>
		public static bool SendANewComment(string text)
		{
			if (RealtimeComments.Count >= Config.ConfigHolder.ConfigData.CommentMaxCount || UITransform == null)
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
