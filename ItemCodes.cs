using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace ChaosEdition
{
    //TODO: reset item colors at the end
    public class ItemColors : ItemCode 
    {
        public override int? MaxLengthSeconds => 120;

        public override int? NextExtraDelaySeconds => -10;

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) 
        {
            ItemColorShift(item);
            return true;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            ItemColorShift(item);
            return true;
        }

        private void ItemColorShift(Item item)
        {
            int r = Main.rand.Next(-10, 11);
            int g = Main.rand.Next(-10, 11);
            int b = Main.rand.Next(-10, 11);
            if (item.color != Color.Transparent)
                item.color = new Color(item.color.R + r, item.color.G + g, item.color.B + b);
            else
                item.color = Color.White;//not sure why not transparent
        }
    }

    public class ItemsResized : ItemCode
    {
        public override int? MaxLengthSeconds => 120;

        public override int? NextExtraDelaySeconds => -10;

        public override float SelectionWeight => 0.93f;

        [NetSync]
        public float itemscale = Main.rand.NextFloat(0.33f, 1.66f);

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor, 0f, origin - (TextureAssets.Item[item.type].Size() * itemscale), scale * itemscale, default, default);
            return false;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[item.type].Value, item.Center - Main.screenPosition, null, lightColor, rotation, item.Size * 0.5f, scale * itemscale, default, default);
            return false;
        }
    }

    public class GroundItemRotation : ItemCode
    {
        public override int? MaxLengthSeconds => 90;

        public override int? NextExtraDelaySeconds => -5;

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Random rand = new Random(whoAmI);
            scale = ((float)rand.Next(7, 16) * 0.1f);
            int dir = rand.Next(0, 2);
            rotation += (Main.GameUpdateCount * ((float)rand.Next(1, 21) * 0.01f) * ((item.velocity.Length() * 0.001f) + 1)) * (dir == 0 ? -1 : 1);
            return true;
        }
    }

    public class ItemScaleChange : ItemCode
    {
        public override int? MaxLengthSeconds => 70;

        public override int? MinLengthSeconds => 20;

        public override int? NextExtraDelaySeconds => -5;

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            item.scale = (float)Math.Sin(Main.GameUpdateCount * 0.02f) * 1.5f;
            return true;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            item.scale = (float)Math.Sin(Main.GameUpdateCount * 0.02f) * 1.5f;
            return true;
        }
    }
}
