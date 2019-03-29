using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonsterGame
{
	public class Icon : Sprite
	{
		private static readonly Random Random = new Random();
		private float _direction = 1;
		private float _colorVal;

		private const float ScalePerFrame = 0.01f;
		private const float ScaleMax = 1.2f;
		private const float ScaleMin = 0.8f;
		private const float CycleVale = 0.005f;

		private readonly string[] _icons = 
		{
			"PPAchievements",
			"PPActivity",
			"PPApp2App",
			"PPAvatars",
			"PPCastingChat",
			"PPDesktop",
			"PPFlighting",
			"PPGameDVR",
			"PPGamepad",
			"PPHeadphones",
			"PPHeadset",
			"PPInputPane",
			"PPNotifications",
			"PPOrientation",
			"PPPCGames",
			"PPSharing",
			"PPStore",
			"PPWindowing",
			"PPXboxApp",
			"PPXboxOne",
		};

		public Icon()
		{
			int index = Random.Next(_icons.Length);
			Animation = new SingleFrameAnimation("gfx\\icons\\" + _icons[index]);
			SetDefaults();
			Color = Color.Silver;
		}

		public override void Update(GameTime gameTime)
		{
			Scale += (ScalePerFrame * _direction);
			if(Scale > ScaleMax)
				_direction = -1;
			else if(Scale < ScaleMin)
				_direction = 1;

			Color = HsvToRgb(_colorVal, 1, 1, 1);
			_colorVal += CycleVale;

			base.Update(gameTime);
		}

		// HSV to RGB converter from http://stackoverflow.com/questions/17080535/hsv-to-rgb-stops-at-yellow-c-sharp
		public Color HsvToRgb(float hue, float saturation, float value, float alpha)
		{
			while (hue > 1f) { hue -= 1f; }
			while (hue < 0f) { hue += 1f; }
			while (saturation > 1f) { saturation -= 1f; }
			while (saturation < 0f) { saturation += 1f; }
			while (value > 1f) { value -= 1f; }
			while (value < 0f) { value += 1f; }
			if (hue > 0.999f) { hue = 0.999f; }
			if (hue < 0.001f) { hue = 0.001f; }
			if (saturation > 0.999f) { saturation = 0.999f; }
			if (saturation < 0.001f) { return new Color(value * 255f, value * 255f, value * 255f); }
			if (value > 0.999f) { value = 0.999f; }
			if (value < 0.001f) { value = 0.001f; }

			float h6 = hue * 6f;
			if (h6 == 6f) { h6 = 0f; }
			int ihue = (int)(h6);
			float p = value * (1f - saturation);
			float q = value * (1f - (saturation * (h6 - ihue)));
			float t = value * (1f - (saturation * (1f - (h6 - ihue))));
			switch (ihue)
			{
				case 0:
					return new Color(value, t, p, alpha);
				case 1:
					return new Color(q, value, p, alpha);
				case 2:
					return new Color(p, value, t, alpha);
				case 3:
					return new Color(p, q, value, alpha);
				case 4:
					return new Color(t, p, value, alpha);
				default:
					return new Color(value, p, q, alpha);
			}
		}
	}
}
