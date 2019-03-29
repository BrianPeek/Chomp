#if !XBOX_LIVE
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	static class XboxLiveManager
	{
		public static UserProfile Profile { get; set; }
		public static bool SignedIn { get { return true; } }

		public static void Initialize(GraphicsDevice graphicsDevice)
		{
			Profile = new UserProfile
			{
				Gamertag = "Gankman",
				Gamerpic = GetEmptyTexture2D(Color.Red)
			};
		}

		public static Task<bool> SignInAsync()
		{
			return Task.FromResult(false);
		}

		public static Task<LeaderboardData> GetLeaderboardAsync(string leaderboard)
		{
			LeaderboardData ld = new LeaderboardData();
			ld.Items.Add(new LeaderboardItem { Rank = 1, Gamertag = "Gankman", Value = "12345" });
			ld.Items.Add(new LeaderboardItem { Rank = 2, Gamertag = "LineJumpers", Value = "2345" });
			ld.Items.Add(new LeaderboardItem { Rank = 3, Gamertag = "Ghetto Llama", Value = "345" });
			return Task.FromResult(ld);
		}

		public static Task<List<AchievementData>> GetAchievementsAsync()
		{
			List<AchievementData> ad = new List<AchievementData>();
			ad.Add(new AchievementData { Name = "Achievement Name", Description = "Achievement Description", IsAchieved = false, Score = "50", Image = GetEmptyTexture2D(Color.Green) });
			ad.Add(new AchievementData { Name = "Achievement Name", Description = "Achievement Description", IsAchieved = false, Score = "50", Image = GetEmptyTexture2D(Color.Orange) });
			ad.Add(new AchievementData { Name = "Achievement Name", Description = "Achievement Description", IsAchieved = false, Score = "50", Image = GetEmptyTexture2D(Color.Yellow) });
			ad.Add(new AchievementData { Name = "Achievement Name", Description = "Achievement Description", IsAchieved = false, Score = "50", Image = GetEmptyTexture2D(Color.Purple) });
			ad.Add(new AchievementData { Name = "Achievement Name", Description = "Achievement Description", IsAchieved = false, Score = "50", Image = GetEmptyTexture2D(Color.Black) });
			ad.Add(new AchievementData { Name = "Achievement Name", Description = "Achievement Description", IsAchieved = false, Score = "50", Image = GetEmptyTexture2D(Color.Red) });
			return Task.FromResult(ad);
		}

		private static Texture2D GetEmptyTexture2D(Color color)
		{
			Texture2D t = new Texture2D(MonsterGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			t.SetData(new[] { color });
			return t;
		}
	}
}
#endif