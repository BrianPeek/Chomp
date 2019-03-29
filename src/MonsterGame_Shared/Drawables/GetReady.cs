using Microsoft.Xna.Framework;

namespace MonsterGame
{
	class GetReady : Sprite
	{
		private readonly int _scaleTime;
		private readonly SingleFrameAnimation _staticText;

		public GetReady(int scaleTime)
		{
			_scaleTime = scaleTime;
			_staticText = new SingleFrameAnimation("gfx\\text\\GetReady");

			Animation = _staticText;
			SetDefaults();
			Reset();
		}

		public void Reset()
		{
			Scale = 1.0f;
			Opacity = 1.0f;
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

		public void Hide()
		{
			Animation = new ScaleAnimation("gfx\\text\\GetReady", 1.0f, 0.0f, _scaleTime);
		}
	}
}
