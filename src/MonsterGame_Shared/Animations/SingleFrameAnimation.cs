using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class SingleFrameAnimation : IAnimation
	{
		private readonly Texture2D _frame;

		public SingleFrameAnimation(string name)
		{
			// load single frame
			_frame = MonsterGame.Instance.Content.Load<Texture2D>(name);
			CurrentFrame = _frame;
		}

		public SingleFrameAnimation(Texture2D texture)
		{
			// load single frame
			_frame = texture;
			CurrentFrame = _frame;
		}

		public Texture2D CurrentFrame { get; set; }

		public virtual void Reset()
		{
		}

		public virtual void Update(GameTime gameTime)
		{
		}
	}
}
