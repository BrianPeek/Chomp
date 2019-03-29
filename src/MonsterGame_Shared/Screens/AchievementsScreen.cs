using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class AchievementsScreen : Screen
	{
		private readonly Texture2D _background;
		private readonly SpriteBatch _spriteBatch;
		private IList<AchievementData> _achievementData;
		private bool _loaded;
		private Vector2 _achievementSize = new Vector2(384,216);
		private List<AchievementDisplay> _achievementDisplay = new List<AchievementDisplay>();

		public AchievementsScreen(Game game) : base(game)
		{
			_spriteBatch = new SpriteBatch(game.GraphicsDevice);
			_background = game.Content.Load<Texture2D>("gfx\\title\\AchievementsScreen");

		}

		private readonly Vector2 _offset = new Vector2(300, 340);

		public override async void Update(GameTime gameTime)
		{
			if(!_loaded && XboxLiveManager.SignedIn)
			{
				_loaded = true;
				_achievementData = await XboxLiveManager.GetAchievementsAsync();
				for(int index = 0; index < _achievementData.Count; index++)
				{
					AchievementData ad = _achievementData[index];
					AchievementDisplay display = new AchievementDisplay(MonsterGame.Instance, ad);
					display.Size = _achievementSize;
					if(index % 2 == 0)
						display.Position = _offset + new Vector2(((_achievementSize.X+20) * index/2), 0);
					else
						display.Position = _offset + new Vector2(((_achievementSize.X+20) * (index-1)/2), (_achievementSize.Y + 20));

					_achievementDisplay.Add(display);
				}
			}

			if(InputManager.CurrentState.Back || InputManager.CurrentState.Start)
				MonsterGame.Instance.SetState(GameState.TitleScreen);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			_spriteBatch.Begin();
				_spriteBatch.Draw(_background, Vector2.Zero, Color.White);
				if(_achievementData == null)
					FontManager.DrawShadowString(_spriteBatch, FontSize.Large, "Loading...", new Vector2(800, 320), Vector2.Zero, 1.0f, Color.White, Color.Black);
				else
				{
					foreach(AchievementDisplay ad in _achievementDisplay)
						ad.Draw(gameTime, _spriteBatch);
				}
			_spriteBatch.End();
		}
	}
}
