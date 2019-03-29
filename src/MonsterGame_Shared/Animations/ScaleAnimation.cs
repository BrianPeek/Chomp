using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonsterGame
{
	class ScaleAnimation : SingleFrameAnimation
	{
		private float _totalTime;
		private float _scaleTime;
		private float _scaleTimeReset;
		private readonly float _start;
		private readonly float _end;

		public float Scale { get; private set; }
		
		public ScaleAnimation(string name, float start, float end, int scaleTime) : base(name)
		{
			_scaleTime = scaleTime;
			_scaleTimeReset = scaleTime;
			_totalTime = scaleTime;
			_start = start;
			_end = end;
		}

		public override void Update(GameTime gameTime)
		{
			_scaleTime -= gameTime.ElapsedGameTime.Milliseconds;
			Scale = MathHelper.Clamp(MathHelper.Lerp(_start, _end, (_totalTime - _scaleTime)/_totalTime), 0.0f, 1.0f);
			base.Update(gameTime);
		}

		public override void Reset()
		{
			Scale = _start;
			_scaleTime = _scaleTimeReset;
			_totalTime = _scaleTimeReset;
		}
	}
}
