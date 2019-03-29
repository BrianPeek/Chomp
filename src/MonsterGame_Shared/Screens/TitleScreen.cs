using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class TitleScreen : Screen
	{
		private enum TitleState
		{
			Select,
			Start,
		};
		
		private readonly List<OnOffMenuItem> _menuItems;
		private readonly Texture2D _titleScreen;
		private readonly SpriteBatch _spriteBatch;
		private int _selectedIndex;
		private TitleState _state = TitleState.Select;
		private readonly GamercardDisplay _gamerCard;

		private static readonly Vector2 MenuPosition = new Vector2(630, 160);

		public TitleScreen(Game game) : base(game)
		{
			_spriteBatch = new SpriteBatch(game.GraphicsDevice);

			_titleScreen = game.Content.Load<Texture2D>("gfx\\title\\TitleScreen");
			_gamerCard = new GamercardDisplay { Position = new Vector2(1500, 20) };

			_menuItems = new List<OnOffMenuItem>()
			{
				new OnOffMenuItem { Off = game.Content.Load<Texture2D>("gfx\\title\\StartBtnOff"),        On = game.Content.Load<Texture2D>("gfx\\title\\StartBtnOn") },
				//new OnOffMenuItem { Off = game.Content.Load<Texture2D>("gfx\\title\\OptionsBtnOff"),      On = game.Content.Load<Texture2D>("gfx\\title\\OptionsBtnOn") },
				new OnOffMenuItem { Off = game.Content.Load<Texture2D>("gfx\\title\\AchievementsBtnOff"), On = game.Content.Load<Texture2D>("gfx\\title\\AchievementsBtnOn") },
				new OnOffMenuItem { Off = game.Content.Load<Texture2D>("gfx\\title\\LeaderboardBtnOff"),  On = game.Content.Load<Texture2D>("gfx\\title\\LeaderboardBtnOn") },
			};

			AudioManager.SoundEffectInstances[AudioManager.Cue.TitleScreenMusic].Play();

			for(int i = 0; i < _menuItems.Count; i++)
			{
				Vector2 position = MenuPosition + new Vector2(0, (i*_menuItems[i].On.Height));
				_menuItems[i].Position = new Rectangle((int)position.X, (int)position.Y, _menuItems[i].On.Width, _menuItems[i].On.Height);
			}
		}

		public override void Update(GameTime gameTime)
		{
			for(int i = 0; i < _menuItems.Count; i++)
			{
				if(_menuItems[i].Position.Contains(MonsterGame.Instance.Scaler.ScaleInput(InputManager.CurrentState.Position.ToVector2()).ToPoint()))
					_selectedIndex = i;
			}

			switch(_state)
			{
				case TitleState.Select:
					if(InputManager.CurrentState.Start)
					{
						switch(_selectedIndex)
						{
							case 0:
								AudioManager.SoundEffectInstances[AudioManager.Cue.TitleScreenMusic].Stop();
								AudioManager.SoundEffectInstances[AudioManager.Cue.Start].Play();
								_state = TitleState.Start;
								break;
							case 1:
								MonsterGame.Instance.SetState(GameState.Achievements);
								break;
							case 2:
								MonsterGame.Instance.SetState(GameState.Leaderboard);
								break;
							default:
								AudioManager.SoundEffects[AudioManager.Cue.MenuSelect].Play();
								break;
						}
					}

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
					break;
				case TitleState.Start:
					if(AudioManager.SoundEffectInstances[AudioManager.Cue.Start].State != SoundState.Playing)
						MonsterGame.Instance.SetState(GameState.GameScreen);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			_gamerCard.SetUserProfile(XboxLiveManager.Profile);
			_gamerCard.Update(gameTime);

			FontManager.YPosition = 500;
		}

		public override void Draw(GameTime gameTime)
		{
			Game.GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin();
				_spriteBatch.Draw(_titleScreen, Vector2.Zero, Color.White);
				for(int i = 0; i < _menuItems.Count; i++)
					_spriteBatch.Draw((_selectedIndex == i) ? _menuItems[i].On : _menuItems[i].Off, MenuPosition + new Vector2(0, (i*_menuItems[i].On.Height)), Color.White);

				_gamerCard.Draw(gameTime, _spriteBatch);
			_spriteBatch.End();
		}
	}
}
