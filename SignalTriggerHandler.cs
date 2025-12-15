using System.Linq;
using UnityEngine;

namespace FakeLivingComments
{
	/// <summary>
	/// 信号触发器处理类，负责手动实现对接游戏部分内容，为触发器提供信号来源
	/// </summary>
	public static class SignalTriggerHandler
	{
		/// <summary>
		/// Boss名单
		/// </summary>
		private static readonly string[] Bosses = {
			"EnemyPreset_Boss_ShortEagle"
		};
		/// <summary>
		/// 射击精准度计时器，用于精准度触发器。用于计时当前计数器达到可以触发精准度触发器的时间，防止子弹飞行速度过久从而未能触发
		/// </summary>
		private static float PrecisionTimer = 0f;
		/// <summary>
		/// 射击精准度计数器，用于精准度触发器。用于记录衡量射击精度的数值
		/// </summary>
		private static float PrecisionCounter = 0f;
		/// <summary>
		/// 加载时调用
		/// </summary>
		internal static void Load()
		{
			Health.OnDead += onHealthDead;
			ItemAgent_Gun.OnMainCharacterShootEvent += onPlayerShoot;
		}
		/// <summary>
		/// 卸载时调用
		/// </summary>
		internal static void Unload()
		{
			Health.OnDead -= onHealthDead;
			ItemAgent_Gun.OnMainCharacterShootEvent -= onPlayerShoot;
		}
		internal static void Update()
		{
			FactoryManager.EmitTriggerSignal("Tick");
			if (LevelManager.Instance.MainCharacter != null)
			{
				CharacterMainControl playerCharacter = LevelManager.Instance.MainCharacter;
				if (playerCharacter.Health.CurrentHealth / playerCharacter.Health.MaxHealth <= 0.35f)
				{
					FactoryManager.EmitTriggerSignal("TickPlayerHealthLow");
				}
				if (playerCharacter.Health.CurrentHealth / playerCharacter.Health.MaxHealth <= 0.15f)
				{
					FactoryManager.EmitTriggerSignal("TickPlayerHealthVeryLow");
				}
			}
		}
		private static void onHealthDead(Health health, DamageInfo damageInfo)
		{
			if (health.IsMainCharacterHealth)
			{
				FactoryManager.EmitTriggerSignal("OnPlayerDeath");
				FactoryManager.EmitTriggerSignal("OnPlayerDeath_from=" + damageInfo.fromCharacter.characterPreset.Name);
			}
			if (!health.IsMainCharacterHealth && damageInfo.fromCharacter.IsMainCharacter)
			{
				FactoryManager.EmitTriggerSignal("OnPlayerKillEnemy");
				string enemyNameKey = health.TryGetCharacter().characterPreset.Name;
				FactoryManager.EmitTriggerSignal("OnPlayerKillEnemy_enemy=" + enemyNameKey);
				if (Bosses.Contains(enemyNameKey))
				{
					FactoryManager.EmitTriggerSignal("OnPlayerKillBoss");
				}
				else
				{
					FactoryManager.EmitTriggerSignal("OnPlayerKillMooks");
				}
			}
		}
		private static void onPlayerShoot(ItemAgent_Gun itemAgent_Gun)
		{
			FactoryManager.EmitTriggerSignal("OnPlayerShoot");
			
		}
	}
}