using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	class AchievementDisplay : Sprite
	{
		private readonly AchievementData _data;
		private Texture2D _gamerscore;
		private Vector2 _size = new Vector2(384,216);

		public AchievementDisplay(Game game, AchievementData ad)
		{
			_gamerscore = game.Content.Load<Texture2D>("gfx\\title\\gicon");
			_data = ad;
			Animation = new SingleFrameAnimation(ad.Image);
			SetDefaults();
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(_data.Image, new Rectangle((int)Position.X, (int)Position.Y, (int)_size.X, (int)_size.Y), Color.White);
			FontManager.DrawShadowString(spriteBatch, FontSize.Regular, _data.Name, Position + new Vector2(10, 110), Vector2.Zero, 1.0f, Color.White, Color.Black);
			FontManager.DrawShadowString(spriteBatch, FontSize.Regular, _data.Description, Position + new Vector2(10, 140), Vector2.Zero, 1.0f, Color.White, Color.Black);
			spriteBatch.Draw(_gamerscore, Position + new Vector2(10, 180), Color.White);
			FontManager.DrawShadowString(spriteBatch, FontSize.Regular, _data.Score, Position + new Vector2(40, 174), Vector2.Zero, 1.0f, Color.White, Color.Black);
		}
	}
}
