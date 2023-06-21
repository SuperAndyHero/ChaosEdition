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
    public class ChaosEditionGlobalItem : GlobalItem
    {
        public override void DrawArmorColor(EquipType type, int slot, Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
        {
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                code.DrawArmorColor(type, slot, drawPlayer, shadow, ref color, ref glowMask, ref glowMaskColor);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                code.ModifyTooltips(item, tooltips);
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            bool DrawTooltip = base.PreDrawTooltip(item, lines, ref x, ref y);
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                DrawTooltip = DrawTooltip && code.PreDrawTooltip(item, lines, ref x, ref y);
            return DrawTooltip;
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            bool DrawTooltipLine = base.PreDrawTooltipLine(item, line, ref yOffset);
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                DrawTooltipLine = DrawTooltipLine && code.PreDrawTooltipLine(item, line, ref yOffset);
            return DrawTooltipLine;
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                code.Update(item, ref gravity, ref maxFallSpeed);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                code.UpdateInventory(item, player);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            bool DoDraw = base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                DoDraw = DoDraw && code.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            return DoDraw;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            bool DoDraw = base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
            foreach (ItemCode code in ChaosEdition.ActiveItemCodes)
                DoDraw = DoDraw && code.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
            return DoDraw;
        }
    }
}
