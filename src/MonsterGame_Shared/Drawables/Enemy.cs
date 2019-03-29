using Microsoft.Xna.Framework;

namespace MonsterGame
{
	public sealed class Enemy : MonsterSprite
	{
		public Enemy()
		{
			IdleAnimation = new LoopingAnimation("gfx\\enemy\\idle\\rm_idle_{0}", 18, 35);
			WalkAnimation = new LoopingAnimation("gfx\\enemy\\walk\\rm_walk_r_{0}", 16, 35);

			Animation = IdleAnimation;
			FacingDirection = Direction.Right;
			CollisionBoundary = new Rectangle(12, 0, 48, 72);
			MoveVelocity = 0.3f;
			SetDefaults();
		}
	}
}
