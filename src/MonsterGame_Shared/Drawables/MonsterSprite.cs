using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public enum Direction { None, Up, Down, Left, Right };

	public class MonsterSprite : Sprite
	{
		public Direction FacingDirection { get; set; }
		public int Column { get; set; }
		public int Row { get; set; }

		protected LoopingAnimation IdleAnimation { get; set; }
		protected LoopingAnimation WalkAnimation { get; set; }
		protected float MoveVelocity { get; set; }

		private Rectangle _collisionBox;

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			DrawBorder = MonsterGame.Instance.DebugMode;

			Column = (int)((Position.X - Maze.MazeOffset.X)/Maze.TileWidth);;
			Row = (int)((Position.Y - Maze.MazeOffset.Y)/Maze.TileHeight);
		}

		public void SetTilePosition(int row, int col)
		{
			Position = new Vector2((col * Maze.TileWidth) + Maze.MazeOffset.X + (Maze.TileWidth/2.0f), (row * Maze.TileHeight) + (Maze.MazeOffset.Y + Maze.TileHeight/2.0f));
			Row = row;
			Column = col;
		}

		public void Move(Direction direction)
		{
			if(direction != Direction.None)
			{
				if(direction != FacingDirection)
					SetTilePosition(Row, Column);

				FacingDirection = direction;
			}

			switch(direction)
			{
				case Direction.Up:
					Velocity = new Vector2(0, -MoveVelocity);
					SpriteEffects = SpriteEffects.None;
					Rotation = MathHelper.ToRadians(-90);
					Walk();
					break;
				case Direction.Down:
					Velocity = new Vector2(0, MoveVelocity);
					SpriteEffects = SpriteEffects.None;
					Rotation = MathHelper.ToRadians(90);
					Walk();
					break;
				case Direction.Left:
					Velocity = new Vector2(-MoveVelocity, 0);
					SpriteEffects = SpriteEffects.FlipHorizontally;
					Rotation = 0;
					Walk();
					break;
				case Direction.Right:
					Velocity = new Vector2(MoveVelocity, 0);
					SpriteEffects = SpriteEffects.None;
					Rotation = 0;
					Walk();
					break;
				case Direction.None:
					Idle();
					break;
				default:
					throw new ArgumentOutOfRangeException("direction", direction, null);
			}
		}

		private void Walk()
		{
			Animation = WalkAnimation;
		}

		private void Idle()
		{
			Velocity = Vector2.Zero;
			Animation = IdleAnimation;
			WalkAnimation.Reset();
		}

		public void Reset()
		{
			Scale = 1.0f;
			Opacity = 1.0f;

			Move(Direction.Right);
			Move(Direction.None);
		}

		public override Rectangle CollisionBox
		{
			get 
			{
				Rectangle boundingBox = BoundingBox;

				switch(FacingDirection)
				{
					case Direction.None:
					case Direction.Up:
					case Direction.Down:
						_collisionBox.X = boundingBox.X + CollisionBoundary.Y;
						_collisionBox.Y = boundingBox.Y + CollisionBoundary.X;

						_collisionBox.Width = CollisionBoundary.Height;
						_collisionBox.Height = CollisionBoundary.Width;

						return _collisionBox;
					case Direction.Left:
					case Direction.Right:
						_collisionBox.X = boundingBox.X + CollisionBoundary.X;
						_collisionBox.Y = boundingBox.Y + CollisionBoundary.Y;

						_collisionBox.Width = CollisionBoundary.Width;
						_collisionBox.Height = CollisionBoundary.Height;

						return _collisionBox;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
