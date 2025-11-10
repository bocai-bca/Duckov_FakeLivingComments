using TMPro;
using UnityEngine;

namespace FakeLivingComments
{
	public class RealtimeComment
	{
		public void Update()
		{
			float lifeTime = Time.time - _spawnTime;
			
		}
		public GameObject? CommentGameObject
		{
			get
			{
				return _gameObject;
			}
		}
		public string CommentText
		{
			get
			{
				return _textMeshPro != null ? _textMeshPro.text : string.Empty;
			}
			set
			{
				if (_textMeshPro != null)
				{
					_textMeshPro.text = value;
				}
			}
		}
		public RealtimeComment(string commentText = "")
		{
			_gameObject = new GameObject("LivingComment");
			_textMeshPro = _gameObject.AddComponent<TextMeshProUGUI>();
			_textMeshPro.text = commentText;
			_spawnTime = Time.time;
		}
		private readonly GameObject? _gameObject;
		private readonly TextMeshProUGUI? _textMeshPro;
		private readonly float _spawnTime;
	}
}
