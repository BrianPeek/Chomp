using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class Sprite
	{
		public Vector2 Size { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Rotation { get; set; }
		public Vector2 Origin { get; private set; }
		public float Scale { get; set; }
		public SpriteEffects SpriteEffects { get; set; }
		public IAnimation Animation { get; set; }
		public bool DrawBorder { get; set; }
		public float Opacity { get; set; }
		public Rectangle CollisionBoundary { get; set; }
		public Color Color { get; set; }

		// bounding box of sprite...used for collisions
		private Rectangle _boundingBox;
		private Rectangle _collisionBox;

		// for debugging BoundingBox...empty class in anything but DEBUG build
		private readonly RectangleBorder _border = new RectangleBorder();

		protected void SetDefaults()
		{
			Origin = new Vector2(Animation.CurrentFrame.Width/2.0f, Animation.CurrentFrame.Height/2.0f);
			Size = new Vector2(Animation.CurrentFrame.Width, Animation.CurrentFrame.Height);
			Scale = 1.0f;
			Opacity = 1.0f;
			Color = Color.White;
		}

		public void Center()
		{
			Position = new Vector2(((MonsterGame.ScreenWidth - Animation.CurrentFrame.Width)/2.0f) + (Animation.CurrentFrame.Width/2.0f),
									((MonsterGame.ScreenHeight - Animation.CurrentFrame.Height)/2.0f) + (Animation.CurrentFrame.Height/2.0f));
		}

		public virtual void Update(GameTime gameTime)
		{
			if(Animation != null)
				Animation.Update(gameTime);

			Position += Velocity * gameTime.ElapsedGameTime.Milliseconds;
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if(Animation == null || Animation.CurrentFrame == null)
				return;

			spriteBatch.Draw(Animation.CurrentFrame, Position, null, Color * Opacity, Rotation, Origin, Scale, SpriteEffects, 0);
			if(DrawBorder)
			{
				_border.Draw(spriteBatch, BoundingBox, 1, Color.Red);
				_border.Draw(spriteBatch, CollisionBox, 1, Color.Green);
			}
		}

		public virtual Rectangle BoundingBox
		{
			get 
			{
				_boundingBox.X = (int)(Position.X - (Origin.X * Scale));
				_boundingBox.Y = (int)(Position.Y - (Origin.Y * Scale));

				_boundingBox.Width = (int)(Size.X * Scale);
				_boundingBox.Height = (int)(Size.Y * Scale);

				return _boundingBox;
			}
		}

		public virtual Rectangle CollisionBox
		{
			get 
			{
				Rectangle boundingBox = BoundingBox;

				_collisionBox.X = boundingBox.X + CollisionBoundary.X;
				_collisionBox.Y = boundingBox.Y + CollisionBoundary.Y;

				_collisionBox.Width = CollisionBoundary.Width;
				_collisionBox.Height = CollisionBoundary.Height;

				return _collisionBox;
			}
		}
	}
}
