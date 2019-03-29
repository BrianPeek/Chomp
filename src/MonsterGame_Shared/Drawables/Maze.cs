using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class Maze : Sprite
	{
		private enum TileType
		{
			PathWithDot,
			PathWithoutDot,
			BlueWall,
			GrayWall,
			PlayerStart,
			PathWithIcon,
			EnemyStart,
		}

		public static int TileWidth = 72;
		public static int TileHeight = 72;

		public const int NumRows = 13;
		public const int NumColumns = 21;
		private const int BlueWallCount = 5;
		private const int GrayWallCount = 5;

		public static readonly Vector2 MazeOffset = new Vector2(
				(MonsterGame.ScreenWidth  - (NumColumns * TileWidth))  / 2.0f, 
				(MonsterGame.ScreenHeight - (NumRows    * TileHeight)) / 2.0f);

		public Vector2 PlayerStart { get; private set; }
		public Vector2 EnemyStart { get; private set; }

		private readonly Texture2D[] _blueWalls = new Texture2D[BlueWallCount];
		private readonly Texture2D[] _grayWalls = new Texture2D[GrayWallCount];
		private readonly Texture2D _pathTile;
		private readonly Dot _dot = new Dot();

		private MazeTile[,] _mazeTiles;

		private static readonly Vector2 CenterTile = new Vector2(TileWidth/2.0f, TileHeight/2.0f);

		public Maze(Game game, int[,] data)
		{
			for(int i = 1; i <= BlueWallCount; i++)
				_blueWalls[i-1] = game.Content.Load<Texture2D>("gfx\\maze\\WallBlue" + i);
			for(int i = 1; i <= GrayWallCount; i++)
				_grayWalls[i-1] = game.Content.Load<Texture2D>("gfx\\maze\\WallGray" + i);

			_pathTile = game.Content.Load<Texture2D>("gfx\\maze\\Wallway");

			CreateMaze(data);
		}

		private void CreateMaze(int[,] data)
		{
			Random r = new Random();
			_mazeTiles = new MazeTile[data.GetLength(0), data.GetLength(1)];

			for(int row = 0; row < data.GetLength(0); row++)
			{
				for(int col = 0; col < data.GetLength(1); col++)
				{
					MazeTile mt = new MazeTile();
					TileType tt = (TileType)data[row,col];
					switch(tt)
					{
						case TileType.PathWithDot:
							mt.DotType = MazeTile.DotTileType.Dot;
							mt.Tile = _pathTile;
							mt.TileType = MazeTile.MazeTileType.Path;
							break;
						case TileType.PlayerStart:
						case TileType.EnemyStart:
						case TileType.PathWithoutDot:
							mt.DotType = MazeTile.DotTileType.None;
							mt.Tile = _pathTile;
							mt.TileType = MazeTile.MazeTileType.Path;
							if(tt == TileType.PlayerStart)
								PlayerStart = new Vector2(col, row);
							if(tt == TileType.EnemyStart)
								EnemyStart = new Vector2(col, row);
							break;
						case TileType.BlueWall:
							mt.DotType = MazeTile.DotTileType.None;
							mt.Tile = _blueWalls[r.Next(_blueWalls.Length)];
							mt.TileType = MazeTile.MazeTileType.Wall;
							break;
						case TileType.GrayWall:
							mt.DotType = MazeTile.DotTileType.None;
							mt.Tile = _grayWalls[r.Next(_grayWalls.Length)];
							mt.TileType = MazeTile.MazeTileType.Wall;
							break;
						case TileType.PathWithIcon:
							mt.DotType = MazeTile.DotTileType.Icon;
							mt.Tile = _pathTile;
							mt.TileType = MazeTile.MazeTileType.Path;
							mt.Icon = new Icon();
							break;
					}
					_mazeTiles[row,col] = mt;
				}
			}
		}

		public MazeTile GetTile(int row, int col)
		{
			if(row < 0 || row >= _mazeTiles.GetLength(0))
				return null;
			if(col < 0 || col >= _mazeTiles.GetLength(1))
				return null;

			return _mazeTiles[row, col];
		}

		public bool IsComplete
		{
			get
			{
				foreach(MazeTile mt in _mazeTiles)
				{
					if(mt.DotType != MazeTile.DotTileType.None)
						return false;
				}
				return true;
			}
		}

		public override void Update(GameTime gameTime)
		{
			for(int row = 0; row < _mazeTiles.GetLength(0); row++)
			{
				for(int col = 0; col < _mazeTiles.GetLength(1); col++)
				{
					MazeTile mt = _mazeTiles[row,col];
					if(mt.DotType == MazeTile.DotTileType.Icon)
						mt.Icon.Update(gameTime);
				}
			}

			_dot.Update(gameTime);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			for(int row = 0; row < _mazeTiles.GetLength(0); row++)
			{
				for(int col = 0; col < _mazeTiles.GetLength(1); col++)
				{
					MazeTile mt = _mazeTiles[row,col];
					Vector2 pos = new Vector2(col * TileWidth, row * TileHeight) + MazeOffset;

					spriteBatch.Draw(_mazeTiles[row,col].Tile, pos, Color.White);

					switch(mt.DotType)
					{
						case MazeTile.DotTileType.None:
							break;
						case MazeTile.DotTileType.Dot:
							_dot.Position = pos + CenterTile;
							_dot.Draw(gameTime, spriteBatch);
							break;
						case MazeTile.DotTileType.Icon:
							mt.Icon.Position = pos + CenterTile;
							mt.Icon.Draw(gameTime, spriteBatch);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}

			base.Draw(gameTime, spriteBatch);
		}
	}
}
