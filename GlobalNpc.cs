using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace ChaosEdition
{
    public class ChaosEditionGlobalNpc : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            bool RunAI = base.PreAI(npc);
            foreach (NpcCode code in ChaosEdition.NpcCodes)
                RunAI = RunAI && code.PreAI(npc, npc.ModNPC);
            return RunAI;
        }

        public override void AI(NPC npc)
        {
            foreach (NpcCode code in ChaosEdition.NpcCodes)
                code.AI(npc, npc.ModNPC);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            foreach (NpcCode code in ChaosEdition.NpcCodes)
                code.DrawEffects(npc, ref drawColor, npc.ModNPC);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool DrawNpc = base.PreDraw(npc, spriteBatch, screenPos, drawColor);
            foreach (NpcCode code in ChaosEdition.NpcCodes)
                DrawNpc = DrawNpc && code.PreDraw(npc, spriteBatch, drawColor, npc.ModNPC);
            return DrawNpc;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            foreach (NpcCode code in ChaosEdition.NpcCodes)
                code.PostDraw(npc, spriteBatch, drawColor, npc.ModNPC);
        }
    }
}
