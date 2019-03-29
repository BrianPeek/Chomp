namespace MonsterGame
{
	public class Dot : Sprite
	{
		public Dot()
		{
			Animation = new LoopingAnimation("gfx\\dot\\Dot{0}", 5, 150);
			SetDefaults();
		}

		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			Rotation += 0.01f;
			base.Update(gameTime);
		}
	}
}
