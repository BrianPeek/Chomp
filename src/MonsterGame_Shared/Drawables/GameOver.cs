using Microsoft.Xna.Framework;

namespace MonsterGame
{
	class GameOver : Sprite
	{
		private readonly int _scaleTime;
		private readonly SingleFrameAnimation _staticText;

		public GameOver(int scaleTime)
		{
			_scaleTime = scaleTime;
			_staticText = new SingleFrameAnimation("gfx\\text\\GameOver");

			Animation = _staticText;
			SetDefaults();
			Reset();
		}

		public void Reset()
		{
			Scale = 0.0f;
			Opacity = 0.0f;
			Animation = _staticText;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			ScaleAnimation sa = (Animation as ScaleAnimation);
			if(sa != null)
			{
				Scale = sa.Scale;
				Opacity = sa.Scale;
			}
		}

		public void Show()
		{
			Animation = new ScaleAnimation("gfx\\text\\GameOver", 0.0f, 1.0f, _scaleTime);
		}
	}
}
