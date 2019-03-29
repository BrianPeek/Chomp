using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonsterGame;

namespace MonsterGame
{
	class LivesDisplay : Sprite
	{
		public int NumberOfLives { get; set; }

		public LivesDisplay()
		{
			Animation = new LoopingAnimation("gfx\\monster\\idle\\bm_idle_{0}", 18, 35);
			SetDefaults();
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			base.Draw(gameTime, spriteBatch);
			FontManager.DrawString(spriteBatch, "x " + NumberOfLives, Position + new Vector2(39, 3), Color.Black);
			FontManager.DrawString(spriteBatch, "x " + NumberOfLives, Position + new Vector2(36, 0), Color.White);
		}
	}
}
