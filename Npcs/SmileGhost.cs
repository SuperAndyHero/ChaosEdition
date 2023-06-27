using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition.Npcs
{
	public class SmileGhost : ModNPC
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Smile Ghost");
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 300;
            NPC.defense = 100;
            NPC.lifeMax = 2000;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[0] = Main.rand.Next(25 * 20, 150 * 20);
        }
        const float maxSpeed = 2.5f;
        const int fadeoutTime = 180;
        public override void AI()
        {
            NPC.ai[1]++;

            NPC.TargetClosestUpgraded(false);
            //NPC.frame = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)(NPC.Size.X), (int)(NPC.Size.Y));
            NPC.rotation = (float)Math.Sin((Main.GameUpdateCount / 100f) + ((float)NPC.whoAmI * 2)) * 0.3f;
            if (NPC.HasValidTarget)
            {
                float maxSpeed = 2.5f;
                Vector2 targetPos = NPC.targetRect.Center.ToVector2();
                float velrot = (float)Math.Sin((NPC.ai[1] / 75f) + Math.Pow((float)NPC.whoAmI, 2)) * 0.8f;
                NPC.velocity += Vector2.Normalize(targetPos - NPC.Center).RotatedBy(velrot) * 0.03f;

                if (NPC.velocity.Length() > maxSpeed)
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * maxSpeed;
            }
            else
            {
                if(NPC.ai[0] > fadeoutTime)
                    NPC.ai[0] = fadeoutTime;
            }

            if(NPC.ai[0] <= fadeoutTime)
                NPC.Opacity = (NPC.ai[0] / (fadeoutTime * 2));
            else
                NPC.Opacity = 0.5f;

            if (NPC.ai[0] <= 0)
                NPC.active = false;

            NPC.ai[0]--;
        }

        public override void DrawEffects(ref Color drawColor)
        {
            drawColor = new Color(drawColor.R + 25, drawColor.G + 25, drawColor.B + 25, drawColor.A);
        }
    }
}