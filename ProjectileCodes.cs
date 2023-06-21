using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition
{
    public class FancyProjectileDeath : ProjectileCode
    {
        public override int MaxLengthSeconds => 80;

        public override int NextExtraDelaySeconds => -20;

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            int dustID = 0;
            switch(Main.rand.Next(0, 5)){
                case 0:
                    dustID = DustID.Firework_Blue;
                    break;
                case 1:
                    dustID = DustID.Firework_Green;
                    break;
                case 2:
                    dustID = DustID.Firework_Pink;
                    break;
                case 3:
                    dustID = DustID.Firework_Red;
                    break;
                case 4:
                    dustID = DustID.Firework_Yellow;
                    break;
            }
            HelperMethods.DrawStar(projectile.Center, dustID, Main.rand.Next(4, 9), Main.rand.NextFloat(1.2f, 2.2f), 1.1f, 1, Main.rand.NextFloat(-0.3f, 2f), 0.5f, 1);
        }
    }

    public class PlayerProjectileHoming : ProjectileCode
    {
        public override int MaxLengthSeconds => 65;
        public override int MinLengthSeconds => 45;

        public override int NextExtraDelaySeconds => 5;

        public override void AI(Projectile projectile)
        {
            projectile.velocity += Vector2.Normalize(Main.LocalPlayer.Center - projectile.position) * 0.2f;
        }
    }

    public class PlayerProjectileRepel : ProjectileCode
    {
        public override int MaxLengthSeconds => 65;
        public override int MinLengthSeconds => 45;

        public override int NextExtraDelaySeconds => 5;

        public override void AI(Projectile projectile)
        {
            projectile.velocity += Vector2.Normalize(projectile.position - Main.LocalPlayer.Center) * 0.4f;
        }
    }

    public class PlayerProjectileOrbit : ProjectileCode
    {
        public override int MaxLengthSeconds => 95;
        public override int MinLengthSeconds => 45;

        public override int NextExtraDelaySeconds => 20;

        public readonly bool dir = Main.rand.NextBool();

        public override void AI(Projectile projectile)
        {
            Vector2 velAdd = Vector2.Normalize(projectile.position - Main.LocalPlayer.Center).RotatedBy(dir ? (Math.PI / 2 + 0.6f) : (-Math.PI / 2 + -0.6f)) * projectile.velocity.Length() * 0.06f;
            projectile.velocity *= 0.95f;
            projectile.velocity += velAdd;
        }
    }

    public class ProjectilesIntoCritters : ProjectileCode
    {
        public override int MaxLengthSeconds => 35;

        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => 10;

        public int critterType = NPCID.Frog;
        public int chance = Main.rand.Next(75, 500);
        public ProjectilesIntoCritters() : base()
        {
            switch (Main.rand.Next(9))
            {
                case 0:
                    critterType = NPCID.Frog;
                    break;
                case 1:
                    critterType = NPCID.Duck;
                    break;
                case 2:
                    critterType = NPCID.Bunny;
                    break;
                case 3:
                    critterType = NPCID.Grubby;
                    break;
                case 4:
                    critterType = NPCID.Worm;
                    break;
                case 5:
                    critterType = NPCID.Penguin;
                    break;
                case 6:
                    critterType = NPCID.Squirrel;
                    break;
                case 7:
                    critterType = NPCID.Bird;
                    break;
                case 8:
                    critterType = NPCID.Snail;
                    break;
            }
        }

        public override void AI(Projectile projectile)
        {
            if (Main.rand.NextBool(chance)&& projectile.active)
            {
                projectile.active = false;
                projectile.damage = 0;
                int npcindex = NPC.NewNPC(projectile.GetSource_FromThis(), (int)projectile.position.X, (int)projectile.position.Y, critterType);
                Main.npc[npcindex].velocity = projectile.velocity.RotatedBy(Main.rand.NextFloat((float)-Math.PI, (float)Math.PI));//Vector2.UnitY.RotatedBy(Main.rand.NextFloat((float)-Math.PI, (float)Math.PI)) * 2;
            }
        }
    }
}
