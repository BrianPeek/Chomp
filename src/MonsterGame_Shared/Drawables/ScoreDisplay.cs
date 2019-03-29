using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class ScoreDisplay : Sprite
	{
		private int _score;
		private string _name = "Player 1";

		public void Update(GameTime gameTime, string name, int score)
		{
			if(!string.IsNullOrEmpty(name))
				_name = name;
			_score = score;
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if(_name != null)
				FontManager.DrawShadowString(spriteBatch, FontSize.Regular, _name, Position, Vector2.Zero, 1.0f, Color.White, Color.Black);
			FontManager.DrawString(spriteBatch,  _score.ToString(), Position + new Vector2(0, 30), Color.DarkBlue);
		}
	}
}
