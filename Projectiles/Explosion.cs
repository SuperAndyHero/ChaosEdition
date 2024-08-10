using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition.Projectiles
{
	public class Explosion : ModProjectile
	{
		public override void SetStaticDefaults() {

		}

		public override void SetDefaults() {
			Projectile.width = 142; // The width of projectile hitbox
			Projectile.height = 200; // The height of projectile hitbox
			Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.penetrate = -1; // Look at comments ExamplePiercingProjectile
            Projectile.timeLeft = 32; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.light = 1f; // How much light emit around the projectile
            SoundEngine.PlaySound(new SoundStyle("ChaosEdition/Projectiles/snd_badexplosion_ch1"));
        }


        public override bool PreDraw(ref Color lightColor) {
			//Main.instance.LoadProjectile(Projectile.type);
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(71, 100);

			Vector2 drawPos = (Projectile.position - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
			Color color = Projectile.GetAlpha(lightColor);
			int frame = Projectile.timeLeft / 2;

            Rectangle source = new Rectangle(((16 - frame) * 144), 0, 142, 200);
			Main.EntitySpriteDraw(texture, drawPos, source, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
	}
}