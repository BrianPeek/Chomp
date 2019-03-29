using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class LoopingAnimation : IAnimation
	{
		protected Texture2D[] Frames { get; set; }
		protected float FrameTime { get; set; }
		protected int FrameCount { get; set; }
		protected int FrameIndex { get; set; }

		protected double Time { get; set; }

		public LoopingAnimation(string name, int frameCount, float frameTime)
		{
			FrameCount = frameCount;
			FrameTime = frameTime;
			Frames = new Texture2D[frameCount];
			for(int i = 1; i <= frameCount; i++)
				Frames[i-1] = MonsterGame.Instance.Content.Load<Texture2D>(string.Format(name, i));
			CurrentFrame = Frames[0];
		}

		public Texture2D CurrentFrame { get; set; }

		public void Reset()
		{
			Time = 0;
			FrameIndex = 0;
		}

		public virtual void Update(GameTime gameTime)
		{
			// count number of milliseconds
			Time += gameTime.ElapsedGameTime.TotalMilliseconds;

			// if we're over the time for the next frame, move on
			if(Time > FrameTime)
			{
				FrameIndex++;
				Time -= FrameTime;
			}

			// if we're past the # of frames, start back at 0
			if(FrameIndex > FrameCount-1)
				FrameIndex = 0;

			CurrentFrame = Frames[FrameIndex];
		}
	}
}
