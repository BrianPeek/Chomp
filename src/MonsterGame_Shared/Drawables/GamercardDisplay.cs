using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	class GamercardDisplay : Sprite
	{
		private UserProfile _profile;

		public void SetUserProfile(UserProfile profile)
		{
			_profile = profile;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if(_profile == null)
				return;

			if(!string.IsNullOrEmpty(_profile.Gamertag))
				FontManager.DrawString(spriteBatch, _profile.Gamertag, Position + new Vector2(110, 24), Color.White);

			if(_profile.Gamerpic != null)
				spriteBatch.Draw(_profile.Gamerpic, new Rectangle((int)Position.X, (int)Position.Y, 100, 100), Color.White);

			base.Draw(gameTime, spriteBatch);
		}
	}
}
