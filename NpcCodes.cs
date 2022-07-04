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
    public class EnemyHoming : NpcCode
    {
        public override int MaxLengthSeconds => 85;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => 30;

        public EnemyHoming() : base()
        {
            Main.windSpeed = 5;
            Main.numClouds += 100;
            Main.NewText("The winds begin to pick up...", new Color(200, 200, 255));
            Main.LocalPlayer.gravity = 0.5f;
        }

        public override void AI(NPC npc, ModNPC modNpc = null)
        {
            if(Main.rand.Next(3) == 0)
                Dust.NewDustPerfect(npc.Top + new Vector2(Main.rand.Next(-(npc.width / 2), (npc.width / 2) + 1), Main.rand.Next(0, npc.height)), Terraria.ID.DustID.Smoke, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-0.5f, 0.1f)));
            npc.velocity += Vector2.Normalize(Main.LocalPlayer.position - npc.position) * (((float)Math.Sin(Main.GameUpdateCount * 0.03f) * 0.1f) + 0.35f);
            npc.velocity += new Vector2((float)Math.Sin(Main.GameUpdateCount * 0.03f) * 0.25f,  -0.15f);
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

        public int multSpeed = Main.rand.Next(25) == 0 ? 20 : Main.rand.Next(2, 5);

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

    public class NpcsIntoCritters : NpcCode
    {
        public override int MaxLengthSeconds => 45;

        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -10;

        public int critterType = NPCID.Frog;
        public int chance = Main.rand.Next(2000, 8000);
        public NpcsIntoCritters() : base()
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

        public override void AI(NPC npc, ModNPC modNpc = null)
        {
            if(Main.rand.Next(chance) == 0 && npc.active && !npc.friendly && !npc.boss && !npc.dontCountMe && !npc.dontTakeDamage && !npc.immortal)
                npc.Transform(critterType);
            base.AI(npc, modNpc);
        }
    }

    public class EnemyRotation : NpcCode
    {
        public override int MaxLengthSeconds => 45;

        public override int NextExtraDelaySeconds => -20;

        public readonly float rotationAmount = Main.rand.NextFloat(-0.4f, 0.4f);
        public override void AI(NPC npc, ModNPC modNpc = null)
        {
            npc.rotation += rotationAmount;
        }
    }
}
