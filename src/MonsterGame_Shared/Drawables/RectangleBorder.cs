//
// Drawn from code at http://bluelinegamestudios.com/posts/drawing-a-hollow-rectangle-border-in-xna-4-0/
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	class RectangleBorder
	{
#if DEBUG
		private readonly Texture2D _pixel;

		public RectangleBorder()
		{
			_pixel = new Texture2D(MonsterGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			_pixel.SetData(new[] { Color.White });
		}
#endif
		public void Draw(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
		{
#if DEBUG
			// Draw top line
			spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);

			// Draw left line
			spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);

			// Draw right line
			spriteBatch.Draw(_pixel, new Rectangle((rect.X + rect.Width - thickness), rect.Y, thickness, rect.Height), color);

			// Draw bottom line
			spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
#endif
		}
	}
}
