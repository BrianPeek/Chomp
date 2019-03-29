using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MonsterGame
{
	public sealed class Player : MonsterSprite
	{
		private readonly SoundEffectInstance _monsterWalk;

		public Player()
		{
			IdleAnimation = new LoopingAnimation("gfx\\monster\\idle\\bm_idle_{0}", 18, 35);
			WalkAnimation = new LoopingAnimation("gfx\\monster\\walk\\bm_walk_r_{0}", 16, 35);

			_monsterWalk = AudioManager.SoundEffects[AudioManager.Cue.MonsterWalk].CreateInstance();

			Animation = IdleAnimation;
			FacingDirection = Direction.Right;
			CollisionBoundary = new Rectangle(12, 0, 48, 72);
			MoveVelocity = 0.5f;
			SetDefaults();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			ScaleAnimation sa = (Animation as ScaleAnimation);
			if(sa != null)
			{
				Scale = sa.Scale;
				Opacity = sa.Scale;
				Rotation = MathHelper.ToRadians(sa.Scale * 360.0f);
			}
		}

		public void EatDot()
		{
			if(_monsterWalk.State != SoundState.Playing)
				_monsterWalk.Play();
		}

		public void EatIcon()
		{
			AudioManager.SoundEffects[AudioManager.Cue.MonsterEat].Play();
		}

		public void Die()
		{
			Velocity = Vector2.Zero;
			Animation = new ScaleAnimation("gfx\\monster\\idle\\bm_idle_1", 1.0f, 0.0f, 300);
		}
	}
}
