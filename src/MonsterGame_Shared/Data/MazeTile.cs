using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class MazeTile
	{
		public enum MazeTileType { Path, Wall }
		public enum DotTileType { None, Dot, Icon }

		public MazeTileType TileType { get; set; }
		public DotTileType DotType { get; set; }
		public Texture2D Tile { get; set; }
		public Sprite Icon { get; set; }
	}
}