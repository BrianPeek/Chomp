using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	class PauseDialog : Sprite
	{
		private readonly List<OnOffMenuItem> _menuItems;
		private int _selectedIndex;
		private bool _isVisible;

		public PauseDialog(Game game)
		{
			Animation = new SingleFrameAnimation("gfx\\dialog\\DialogBox_Empty");

			_menuItems = new List<OnOffMenuItem>
			{
				new OnOffMenuItem { Off = game.Content.Load<Texture2D>("gfx\\dialog\\ResumeBtnOff"), On = game.Content.Load<Texture2D>("gfx\\dialog\\ResumeBtnOn") },
				new OnOffMenuItem { Off = game.Content.Load<Texture2D>("gfx\\dialog\\ExitBtnOff"),   On = game.Content.Load<Texture2D>("gfx\\dialog\\ExitBtnOn") },
			};

			SetDefaults();
		}

		public void Show()
		{
			_isVisible = true;
		}

		public void Hide()
		{
			_isVisible = false;
		}

		public override void Update(GameTime gameTime)
		{
			if(_isVisible)
			{
				if(InputManager.CurrentState.Up)
				{
					_selectedIndex = MathHelper.Clamp(_selectedIndex-1, 0, _menuItems.Count-1);
					AudioManager.SoundEffects[AudioManager.Cue.MenuUpDown].Play();
				}
				if(InputManager.CurrentState.Down)
				{
					_selectedIndex = MathHelper.Clamp(_selectedIndex+1, 0, _menuItems.Count-1);
					AudioManager.SoundEffects[AudioManager.Cue.MenuUpDown].Play();
				}

				if(InputManager.CurrentState.Start)
				{
					switch(_selectedIndex)
					{
						case 0:
							Hide();
							break;
						case 1:
							AudioManager.StopAll();
							MonsterGame.Instance.SetState(GameState.TitleScreen);
							break;
					}

					AudioManager.SoundEffects[AudioManager.Cue.MenuSelect].Play();
					MonsterGame.Instance.IsPaused = !MonsterGame.Instance.IsPaused;
				}

				base.Update(gameTime);
			}
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (_isVisible)
			{
				base.Draw(gameTime, spriteBatch);
				for(int i = 0; i < _menuItems.Count; i++)
				{
					Vector2 position = Position + new Vector2(0, (i * _menuItems[i].On.Height - 70));
					Vector2 origin = new Vector2(_menuItems[i].On.Width / 2.0f, _menuItems[i].On.Height / 2.0f);

					spriteBatch.Draw(
						(_selectedIndex == i) ? _menuItems[i].On : _menuItems[i].Off,
						position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0f);
				}
			}
		}
	}
}
