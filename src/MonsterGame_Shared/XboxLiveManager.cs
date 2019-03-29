#if XBOX_LIVE
using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xbox.Services.Leaderboard;
using Microsoft.Xbox.Services.Achievements;
using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social;
using Microsoft.Xbox.Services.System;

namespace MonsterGame
{
	static class XboxLiveManager
    {
		public static bool SignedIn { get; set; }
		public static UserProfile Profile { get; private set; }
		public static object LeaderBoardResult { get; private set; }

		private static GraphicsDevice _graphicsDevice;
		private static XboxLiveUser _user;
		private static XboxLiveContext _context;
		private static XboxUserProfile _profile;

		private const string ServiceConfigId = "00000000-0000-0000-0000-000000000000";
		private const uint TitleId = 0x00000000;

        public static void Initialize(GraphicsDevice graphicsDevice)
		{
			_graphicsDevice = graphicsDevice;
			Profile = new UserProfile();
		}

		public static async Task<bool> SignInAsync()
		{
			Profile.Gamertag = "Signing in...";

			try
			{
				_user = new XboxLiveUser();
				SignInResult result = await _user.SignInAsync(Window.Current.Dispatcher);
				if(result.Status == SignInStatus.Success)
				{
					_context = new XboxLiveContext(_user);
					_profile = await _context.ProfileService.GetUserProfileAsync(_context.User.XboxUserId);
					Profile.Gamertag = _profile.Gamertag;

					SignedIn = true;

					Profile.Gamerpic = await GetTexture2dFromUriAsync(_profile.GameDisplayPictureResizeUri);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				Profile.Gamertag = "Not logged in";
				return false;
			}

			return true;
		}

		public static async Task<LeaderboardData> GetLeaderboardAsync(string leaderboard)
		{
			if(_context == null || !SignedIn)
				return null;

			LeaderboardResult lbr = await _context.LeaderboardService.GetLeaderboardAsync(ServiceConfigId, leaderboard);
			LeaderboardData ld = new LeaderboardData();
			foreach(var item in lbr.Rows)
			{
				LeaderboardItem li = new LeaderboardItem()
				{
					Rank = item.Rank,
					Gamertag = item.Gamertag,
					Value = item.Values[0]
				};
				ld.Items.Add(li);
			}
			return ld;
		}

		public static async Task<IList<AchievementData>> GetAchievementsAsync()
		{
			List<AchievementData> list = new List<AchievementData>();

			AchievementsResult ar = await _context.AchievementService.GetAchievementsForTitleIdAsync(_context.User.XboxUserId, TitleId, AchievementType.All, false, AchievementOrderBy.Default, 0, 10);
			foreach(Achievement achievement in ar.Items)
			{
				AchievementData ad = new AchievementData
				{
					Name = achievement.Name,
					Description = achievement.LockedDescription,
					Score = achievement.Rewards[0].Data,
					IsAchieved = achievement.ProgressState == AchievementProgressState.Achieved,
					Image = await GetTexture2dFromUriAsync(new Uri(achievement.MediaAssets[0].Url))
				};
				list.Add(ad);
			}
			return list;
		}

		private static async Task<Texture2D> GetTexture2dFromUriAsync(Uri uri)
		{
			try
			{
				HttpClient hc = new HttpClient();
				byte[] data = await hc.GetByteArrayAsync(uri);
				MemoryStream ms = new MemoryStream(data);
				return Texture2D.FromStream(_graphicsDevice, ms);
			}
			catch (Exception)
			{
			}
			return null;
		}
    }
}
#endif