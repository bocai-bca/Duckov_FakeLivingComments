using FakeLivingComments.Config;
using UnityEngine;
using UnityEngine.UI;

namespace FakeLivingComments
{
	/// <summary>
	/// 弹幕类
	/// </summary>
	public class RealtimeComment
	{
		/// <summary>
		/// 更新当前实例所代表的弹幕GameObject
		/// </summary>
		public void Update()
		{
			if (CommentGameObject == null || _text == null)
			{
				return;
			}
			float lifePercentage = Mathf.Clamp(1f - (Time.time - _spawnTime) / ConfigHolder.ConfigData.CommentStaySeconds, 0f, 1f); //生命周期百分比，刚生成时为1f，结束时为0f
			CommentGameObject.transform.position = new Vector3(_text.preferredWidth / -2f + Mathf.Lerp(0f, Screen.width + _text.preferredWidth / 2f, lifePercentage), CommentGameObject.transform.position.y);
			if (lifePercentage <= 0f)
			{
				Destroy();
			}
		}
		/// <summary>
		/// 销毁该弹幕，调用此方法后IsAlive将返回false，便于此实例的引用持有者释放引用
		/// </summary>
		public void Destroy()
		{
			if (CommentGameObject != null)
			{
				Object.Destroy(CommentGameObject);
			}
		}
		/// <summary>
		/// 该弹幕的GameObject
		/// </summary>
		public GameObject? CommentGameObject;
		/// <summary>
		/// 返回该弹幕实例是否仍然可用，相当于this.CommentGameObject != null，在调用Destroy()后返回false
		/// </summary>
		public bool IsAlive
		{
			get
			{
				return CommentGameObject != null;
			}
		}
		/// <summary>
		/// 该弹幕的文本
		/// </summary>
		public string CommentText
		{
			get
			{
				return _text != null ? _text.text : string.Empty;
			}
			set
			{
				if (_text != null)
				{
					_text.text = value;
				}
			}
		}

		/// <summary>
		/// 构造一个弹幕类并创建新的GameObject
		/// </summary>
		/// <param name="commentText">该条弹幕的文本</param>
		/// <param name="parent">新弹幕实例GameObject附加到的父RectTransform</param>
		public RealtimeComment(string commentText, RectTransform parent)
		{
			ConfigStruct configStruct = ConfigHolder.ConfigData;
			CommentGameObject = new GameObject("LivingComment")
			{
				layer = LayerMask.NameToLayer("UI")
			};
			RectTransform rectTransform = CommentGameObject.AddComponent<RectTransform>();
			rectTransform.SetParent(parent);
			CommentGameObject.hideFlags = HideFlags.HideAndDontSave;
			rectTransform.position = new Vector3(0f, Random.Range(configStruct.CommentLowestHeight * Screen.height, Screen.height - configStruct.CommentFontSize / 2f));
			_canvasRenderer = CommentGameObject.AddComponent<CanvasRenderer>();
			_canvasRenderer.SetAlpha(configStruct.CommentAlpha);
			_text = CommentGameObject.AddComponent<Text>();
			_text.text = commentText;
			_text.fontSize = configStruct.CommentFontSize;
			_spawnTime = Time.time;
		}
		private Text? _text;
		private CanvasRenderer? _canvasRenderer;
		private float _spawnTime;
	}
}
