using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace FakeLivingComments
{
	public class RealtimeComment
	{
		public void Update()
		{
			float life_time = Time.time - spawnTime;

		}
		public GameObject? CommentGameObject
		{
			get
			{
				return gameObject;
			}
		}
		public string CommentText
		{
			get
			{
				if (textMeshPro != null)
				{
					return textMeshPro.text;
				}
				return string.Empty;
			}
			set
			{
				if (textMeshPro != null)
				{
					textMeshPro.text = value;
				}
			}
		}
		public RealtimeComment(string comment_text = "")
		{
			gameObject = new GameObject("LivingComment");
			textMeshPro = gameObject.AddComponent<TextMeshProUGUI>();
			textMeshPro.text = comment_text;
			spawnTime = Time.time;
		}
		private readonly GameObject? gameObject;
		private readonly TextMeshProUGUI? textMeshPro;
		private readonly float spawnTime;
	}
}
