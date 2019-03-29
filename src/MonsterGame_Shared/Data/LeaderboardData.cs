using System.Collections.Generic;

namespace MonsterGame
{
	class LeaderboardData
	{
		public List<LeaderboardItem> Items { get; set; }

		public LeaderboardData()
		{
			Items = new List<LeaderboardItem>();
		}
	}
}
