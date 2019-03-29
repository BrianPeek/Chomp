using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class LeaderboardScreen : Screen
	{
		private readonly Texture2D _background;
		private readonly SpriteBatch _spriteBatch;
		private LeaderboardData _leaderboardData;
		private bool _loaded = false;

		public LeaderboardScreen(Game game) : base(game)
		{
			_spriteBatch = new SpriteBatch(game.GraphicsDevice);
			_background = game.Content.Load<Texture2D>("gfx\\title\\LeaderboardScreen");
		}

		public override async void Update(GameTime gameTime)
		{
			if(!_loaded && XboxLiveManager.SignedIn)
			{
				_loaded = true;
				_leaderboardData = await XboxLiveManager.GetLeaderboardAsync("ScoreLeaderboard");
			}

			if(InputManager.CurrentState.Back || InputManager.CurrentState.Start)
				MonsterGame.Instance.SetState(GameState.TitleScreen);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			Vector2 linePos = Vector2.Zero;

			_spriteBatch.Begin();
				_spriteBatch.Draw(_background, Vector2.Zero, Color.White);
				if(_leaderboardData == null)
					FontManager.DrawShadowString(_spriteBatch, FontSize.Large, "Loading...", new Vector2(800, 320), Vector2.Zero, 1.0f, Color.White, Color.Black);
				else
				{
					foreach(LeaderboardItem li in _leaderboardData.Items)
					{
						FontManager.DrawString(_spriteBatch, FontSize.Large, li.Rank.ToString(), new Vector2(420, 320) + linePos, Vector2.Zero, 1.0f, Color.White);
						FontManager.DrawString(_spriteBatch, FontSize.Large, li.Gamertag, new Vector2(600, 320) + linePos, Vector2.Zero, 1.0f, Color.White);
						FontManager.DrawString(_spriteBatch, FontSize.Large, li.Value, new Vector2(1360, 320) + linePos, Vector2.Zero, 1.0f, Color.White);
						linePos += new Vector2(0, 80);
					}
				}
			_spriteBatch.End();
		}
	}
}
