using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition
{
    public class EnemyHoming : NpcCode
    {
        public override int MaxLengthSeconds => 85;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => 30;

        [NetSync]
        public int Timer = 0;//netsync is only used here so that people joining get the right timer

        public EnemyHoming() : base()
        {
            Main.windSpeedCurrent = 5;
            Main.numClouds += 100;
            Main.NewText("The winds begin to pick up...", new Color(200, 200, 255));
            //Main.LocalPlayer.gravity = 0.5f;??
        }


        public override void AI(NPC npc, ModNPC modNpc = null)
        {
            if (Main.netMode != NetmodeID.Server)//clientside
            {
                if (Main.rand.NextBool(3))
                    Dust.NewDustPerfect(npc.Top + new Vector2(Main.rand.Next(-(npc.width / 2), (npc.width / 2) + 1), Main.rand.Next(0, npc.height)), Terraria.ID.DustID.Smoke, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-0.5f, 0.1f)));
            }

            Timer++;

            Vector2 closetPlayerCenter = Main.netMode == NetmodeID.SinglePlayer ? //only runs slow netsafe version on multiplayer
                Main.LocalPlayer.Center :
                HelperMethods.NearestPlayerCenter(npc.position);

            if (closetPlayerCenter == Vector2.Zero)
                return;

            npc.velocity += Vector2.Normalize(closetPlayerCenter - npc.position) * (((float)Math.Sin(Timer * 0.03f) * 0.1f) + 0.35f);
            npc.velocity += new Vector2((float)Math.Sin(Timer * 0.03f) * 0.25f,  -0.15f);
        }

    }

    public class EnemyColors : NpcCode
    {
        public override int MaxLengthSeconds => 0;

        public override int NextExtraDelaySeconds => -30;

        public override void AI(NPC npc, ModNPC modNpc = null)
        {
            npc.color = new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256));
        }

    }

    public class MultNpcSpeed : NpcCode
    {
        public override int MaxLengthSeconds => 55;

        public override int NextExtraDelaySeconds => 30;

        [NetSync]
        public int multSpeed = Main.rand.NextBool(25) ? 20 : Main.rand.Next(2, 5);

        public override bool PreAI(NPC npc, ModNPC modNpc = null)
        {
            for (int i = 0; i < multSpeed; i++)
            {
                npc.VanillaAI();
                NPCLoader.AI(npc);
                NPCLoader.PostAI(npc);
            }
            return base.PreAI(npc, modNpc);
        }
    }

    //TODO: sync npc creation(?)
    public class NpcsIntoCritters : NpcCode
    {
        public override int MaxLengthSeconds => 45;

        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -10;

        //these are not synced since the client does not need them
        public int critterType = ProjectilesIntoCritters.PickRandomCritter();
        public int chance = Main.rand.Next(2000, 8000);

        public override void AI(NPC npc, ModNPC modNpc = null)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)//serverside
            {
                if (Main.rand.NextBool(chance) && npc.active && !npc.friendly && !npc.boss && !npc.dontCountMe && !npc.dontTakeDamage && !npc.immortal)
                    npc.Transform(critterType);
            }

            base.AI(npc, modNpc);
        }
    }

    public class EnemyRotation : NpcCode
    {
        public override int MaxLengthSeconds => 45;

        public override int NextExtraDelaySeconds => -20;

        [NetSync]
        public float rotationAmount = Main.rand.NextFloat(-0.4f, 0.4f);

        public override void AI(NPC npc, ModNPC modNpc = null)
        {
            npc.rotation += rotationAmount;
        }
    }
}
