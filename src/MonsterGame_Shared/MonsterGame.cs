#if WINDOWS_UAP
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.Foundation;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public enum GameState
	{
		TitleScreen,
		GameScreen,
		Achievements,
		Leaderboard
	};

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class MonsterGame : Game
	{
		public static MonsterGame Instance;

		public static int ScreenWidth = 1920;
		public static int ScreenHeight = 1080;

		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private Screen _screen;
		internal RenderTargetScaler Scaler { get; set; }

		public bool DebugMode { get; set; }
		public bool IsPaused { get; set; }

		public MonsterGame()
		{
			Instance = this;

			_graphicsDeviceManager = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = ScreenWidth,
				PreferredBackBufferHeight = ScreenHeight
			};

			IsMouseVisible = true;
			Window.AllowUserResizing = true;

			_graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override async void Initialize()
		{
			Scaler = new RenderTargetScaler(this, _graphicsDeviceManager, ScreenWidth, ScreenHeight);

#if WINDOWS
			Scaler.IsEnabled = false;
#endif

			SetState(GameState.TitleScreen);

			XboxLiveManager.Initialize(_graphicsDeviceManager.GraphicsDevice);

#if WINDOWS_UAP
			if(!ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
#endif	
			{
				await XboxLiveManager.SignInAsync();
			}	
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			InputManager.Update(gameTime);

#if DEBUG
			if(InputManager.CurrentState.Debug)
				DebugMode = !DebugMode;
#endif

			if (InputManager.CurrentState.FullScreen)
				_graphicsDeviceManager.ToggleFullScreen();

			if(IsActive && !IsPaused)
				AudioManager.ResumeAll();
			else
				AudioManager.PauseAll();

			if(IsActive)
				_screen.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			if(IsActive)
			{
				Scaler.SetRenderTarget();
				_screen.Draw(gameTime);
				Scaler.Draw();
			}
		}

		public void SetState(GameState newState)
		{
			switch(newState)
			{
				case GameState.TitleScreen:
					_screen = new TitleScreen(this);
					break;
				case GameState.GameScreen:
					_screen = new GameScreen(this);
					break;
				case GameState.Achievements:
					_screen = new AchievementsScreen(this);
					break;
				case GameState.Leaderboard:
					_screen = new LeaderboardScreen(this);
					break;
			}
		}
	}
}
