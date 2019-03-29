using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonsterGame
{
	class AchievementData
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Score { get; set; }
		public Texture2D Image { get; set; }
		public bool IsAchieved { get; set; }
	}
}
