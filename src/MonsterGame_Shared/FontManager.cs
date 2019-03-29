using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public enum Alignment
	{
		Left,
		Centered,
		Right
	}

	public enum FontSize
	{
		Regular,
		Large
	}

	public static class FontManager
	{
		// start at this position, write one line at a time below
		public static int YPosition;

		private static readonly Vector2 ShadowOffset = new Vector2(3,3);

		private static Dictionary<FontSize,SpriteFont> _fontMap = new Dictionary<FontSize,SpriteFont>()
		{
			{ FontSize.Regular, MonsterGame.Instance.Content.Load<SpriteFont>("Font") },
			{ FontSize.Large, MonsterGame.Instance.Content.Load<SpriteFont>("LargeFont") },
		};

		static FontManager()
		{
		}

		public static Vector2 MeasureString(FontSize size, string text)
		{
			return _fontMap[size].MeasureString(text);
		}

		public static void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
		{
			spriteBatch.DrawString(_fontMap[FontSize.Regular], text, position, color);
		}


		public static void DrawString(SpriteBatch spriteBatch, FontSize size, string text, Vector2 position, Vector2 origin, float scale, Color color)
		{
			spriteBatch.DrawString(_fontMap[size], text, position, color, 0, origin, scale, SpriteEffects.None, 0);
		}

		public static void DrawShadowString(SpriteBatch spriteBatch, FontSize size, string text, Vector2 position, Vector2 origin, float scale, Color color, Color shadow)
		{
			DrawString(spriteBatch, size, text, position + ShadowOffset, origin, scale, shadow);
			DrawString(spriteBatch, size, text, position, origin, scale, color);
		}

		public static void DrawLine(SpriteBatch spriteBatch, FontSize size, string text, float x, float scale, Color color)
		{
			DrawLine(spriteBatch, size, text, x, scale, 0, color);
		}

		public static void DrawLine(SpriteBatch spriteBatch, FontSize size, string text, float x, float scale, float rotation, Color color)
		{
			DrawLine(spriteBatch, size, text, new Vector2(x, YPosition), scale, rotation, color, Alignment.Left);
		}

		public static void DrawLine(SpriteBatch spriteBatch, FontSize size, string text, Vector2 pos, float scale, float rotation, Color color, Alignment align)
		{
			Vector2 alignPos = Vector2.Zero;
			Vector2 stringSize = _fontMap[size].MeasureString(text);

			switch(align)
			{
				case Alignment.Left:
					alignPos = Vector2.Zero;
					break;
				case Alignment.Centered:
					alignPos = stringSize/2;
					break;
				case Alignment.Right:
					alignPos = new Vector2(MonsterGame.ScreenWidth - stringSize.X, YPosition);
					break;
			}

			// draw the string and increment the vertical position
			spriteBatch.DrawString(_fontMap[size], text, pos, color, rotation, alignPos, scale, SpriteEffects.None, 1.0f);
			YPosition += (int)stringSize.Y;
		}

		public static void Reset()
		{
			YPosition = 0;
		}
	}
}
