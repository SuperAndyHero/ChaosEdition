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
            foreach (NpcCode code in ChaosEdition.ActiveNpcCodes)
                RunAI = RunAI && code.PreAI(npc, npc.ModNPC);
            return RunAI;
        }

        public override void AI(NPC npc)
        {
            foreach (NpcCode code in ChaosEdition.ActiveNpcCodes)
                code.AI(npc, npc.ModNPC);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            foreach (NpcCode code in ChaosEdition.ActiveNpcCodes)
                code.DrawEffects(npc, ref drawColor, npc.ModNPC);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool DrawNpc = base.PreDraw(npc, spriteBatch, screenPos, drawColor);
            foreach (NpcCode code in ChaosEdition.ActiveNpcCodes)
                DrawNpc = DrawNpc && code.PreDraw(npc, spriteBatch, drawColor, npc.ModNPC);
            return DrawNpc;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            foreach (NpcCode code in ChaosEdition.ActiveNpcCodes)
                code.PostDraw(npc, spriteBatch, drawColor, npc.ModNPC);
        }

        public override void OnKill(NPC npc)
        {
            foreach (NpcCode code in ChaosEdition.ActiveNpcCodes)
                code.OnKill(npc);
        }

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            foreach (NpcCode code in ChaosEdition.ActiveNpcCodes)
                code.HitEffect(npc, hit);
        }
    }
}
