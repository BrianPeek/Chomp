using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace MonsterGame
{
	public static class AudioManager
	{
		public enum Cue
		{
			MenuUpDown,
			MenuSelect,
			Start,

			MonsterWalk,
			MonsterEat,

			TitleScreenMusic,
			GetReadyMusic,
			GameplayMusic,
			PlayerEaten,
			LevelCompleted,
			GameOver,
		};

		public static Dictionary<Cue,SoundEffect> SoundEffects;
		public static Dictionary<Cue,SoundEffectInstance> SoundEffectInstances;

		static AudioManager()
		{
			SoundEffects = new Dictionary<Cue,SoundEffect>
			{
				{ Cue.Start,        MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\StartButton") },
				{ Cue.MenuSelect,	MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\MenuSelect")  },
				{ Cue.MenuUpDown,	MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\MenuUpDown")  },

				{ Cue.MonsterWalk,	MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\MonsterWalk") },
				{ Cue.MonsterEat,	MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\MonsterEat") },
				{ Cue.PlayerEaten,  MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\PlayerEaten") },

				{ Cue.TitleScreenMusic, MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\music\\TitleScreen") },
				{ Cue.GetReadyMusic,	MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\music\\GetReady")    },
				{ Cue.GameplayMusic,    MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\music\\Gameplay")    },
				{ Cue.LevelCompleted,   MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\music\\LevelComplete")},
				{ Cue.GameOver,         MonsterGame.Instance.Content.Load<SoundEffect>("sfx\\music\\GameOver")    },
			};

			SoundEffectInstances = new Dictionary<Cue,SoundEffectInstance>
			{
				{ Cue.TitleScreenMusic, SoundEffects[Cue.TitleScreenMusic].CreateInstance() },
				{ Cue.GetReadyMusic, SoundEffects[Cue.GetReadyMusic].CreateInstance() },
				{ Cue.GameplayMusic, SoundEffects[Cue.GameplayMusic].CreateInstance() },
				{ Cue.PlayerEaten, SoundEffects[Cue.PlayerEaten].CreateInstance() },
				{ Cue.LevelCompleted, SoundEffects[Cue.LevelCompleted].CreateInstance() },
				{ Cue.GameOver, SoundEffects[Cue.GameOver].CreateInstance() },
				{ Cue.Start, SoundEffects[Cue.Start].CreateInstance() },
			};
		}

		public static void PauseAll()
		{
			foreach(var item in SoundEffectInstances)
			{
				if(item.Value.State == SoundState.Playing)
					item.Value.Pause();
			}
		}

		public static void ResumeAll()
		{
			foreach(var item in SoundEffectInstances)
			{
				if(item.Value.State == SoundState.Paused)
					item.Value.Resume();
			}
		}

		public static void StopAll()
		{
			foreach(var item in SoundEffectInstances)
				item.Value.Stop();
		}
	}
}
