using Microsoft.Xna.Framework;

namespace MonsterGame
{
	public class ReversingAnimation : LoopingAnimation
	{
		private int _animationDirection = 1;

		public ReversingAnimation(string name, int frameCount, float frameTime) : base(name, frameCount, frameTime)
		{
		}

		public override void Update(GameTime gameTime)
		{
			Time += gameTime.ElapsedGameTime.TotalMilliseconds;
			if(Time > FrameTime)
			{
				Time -= FrameTime;

				if(FrameIndex == 0)
					_animationDirection = 1;
				else if(FrameIndex == FrameCount-1)
					_animationDirection = -1;

				FrameIndex += _animationDirection;
			}

			CurrentFrame = Frames[FrameIndex];
		}
	}
}
