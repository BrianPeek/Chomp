using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	class RenderTargetScaler
	{
		private readonly RenderTarget2D _drawBuffer;
		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private readonly int _screenWidth;
		private readonly int _screenHeight;
		private readonly Game _game;
		private readonly SpriteBatch _spriteBatch;

		public bool IsEnabled { get; set; }

		public RenderTargetScaler(Game game, GraphicsDeviceManager graphicsDeviceManager, int width, int height)
		{
			_game = game;
			_graphicsDeviceManager = graphicsDeviceManager;
			_screenWidth = width;
			_screenHeight = height;

			_spriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);

			PresentationParameters pp = graphicsDeviceManager.GraphicsDevice.PresentationParameters;

			// create a surface to draw on which is then scaled to the screen size on the PC
			_drawBuffer = new RenderTarget2D(graphicsDeviceManager.GraphicsDevice,
											width, height, false,
											SurfaceFormat.Color,
											DepthFormat.None,
											pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

			IsEnabled = true;
		}

		public void SetFullScreen(bool fullScreen)
		{
			if(fullScreen)
			{
				// going fullscreen, use desktop resolution to minimize display mode changes
				// this also has the nice effect of working around some displays that lie about 
				// supporting 1280x720
				GraphicsAdapter adapter = _graphicsDeviceManager.GraphicsDevice.Adapter;
				_graphicsDeviceManager.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
				_graphicsDeviceManager.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
			}
			else
			{   // going windowed
				_graphicsDeviceManager.PreferredBackBufferWidth = _screenWidth;
				_graphicsDeviceManager.PreferredBackBufferHeight = _screenHeight;
			}

			if(fullScreen != _graphicsDeviceManager.IsFullScreen)
				_graphicsDeviceManager.ToggleFullScreen();
		}

		public void SetRenderTarget()
		{
			if(!IsEnabled)
				return;

			_graphicsDeviceManager.GraphicsDevice.SetRenderTarget(_drawBuffer);
		}

		public void Draw()
		{
			if(!IsEnabled)
				return;

			PresentationParameters presentation = _graphicsDeviceManager.GraphicsDevice.PresentationParameters;

			float outputAspect = _game.Window.ClientBounds.Width / (float)_game.Window.ClientBounds.Height;
			float preferredAspect = _screenWidth / (float)_screenHeight;

			Rectangle dst;

			if (outputAspect <= preferredAspect)
			{
				// output is taller than it is wider, bars on top/bottom
				int presentHeight = (int)((_game.Window.ClientBounds.Width / preferredAspect) + 0.5f);
				int barHeight = (_game.Window.ClientBounds.Height - presentHeight) / 2;

				dst = new Rectangle(0, barHeight, _game.Window.ClientBounds.Width, presentHeight);
			}
			else
			{
				// output is wider than it is tall, bars left/right
				int presentWidth = (int)((_game.Window.ClientBounds.Height * preferredAspect) + 0.5f);
				int barWidth = (_game.Window.ClientBounds.Width - presentWidth) / 2;

				dst = new Rectangle(barWidth, 0, presentWidth, _game.Window.ClientBounds.Height);
			}

			_graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

			// clear to get black bars
			_graphicsDeviceManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

			// draw a quad to get the draw buffer to the back buffer
			_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
				_spriteBatch.Draw(_drawBuffer, dst, Color.White);
			_spriteBatch.End();
		}

		public Vector2 ScaleInput(Vector2 input)
		{
			float preferredAspect = _screenWidth/(float)_screenHeight;

			int presentHeight = (int)((_game.Window.ClientBounds.Width / preferredAspect) + 0.5f);
			int barHeight = (int)MathHelper.Clamp((_game.Window.ClientBounds.Height - presentHeight) / 2.0f, 0, _screenHeight);

			int presentWidth = (int)((_game.Window.ClientBounds.Height * preferredAspect) + 0.5f);
			int barWidth = (int)MathHelper.Clamp((_game.Window.ClientBounds.Width - presentWidth) / 2.0f, 0, _screenWidth);

			Vector2 scale = new Vector2((float)(_game.Window.ClientBounds.Width - (barWidth * 2)) / _screenWidth,
										(float)(_game.Window.ClientBounds.Height - (barHeight * 2)) / _screenHeight);

			return ((input - new Vector2(barWidth, barHeight)) / scale);
		}
	}
}
