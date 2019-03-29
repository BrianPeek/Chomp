using Microsoft.Xna.Framework;

namespace MonsterGame
{
	public class Screen
	{
		public Game Game { get; private set; }

		public Screen(Game game)
		{
			Game = game;
		}

		public virtual void Update(GameTime gameTime)
		{
		}

		public virtual void Draw(GameTime gameTime)
		{
		}
	}
}
