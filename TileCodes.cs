using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace ChaosEdition
{
    public class RandomColors : TileCode
    {
        private float a;
        private float b;
        private float c;

        public RandomColors() : base()
        {
            a = Main.rand.NextFloat(-3f, 3f);
            b = Main.rand.NextFloat(-3f, 3f);
            c = Main.rand.NextFloat(-3f, 3f);
        }

        public override int MaxLengthSeconds => 90;

        public override int NextExtraDelaySeconds => -15;
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            drawColor = new Color(
                (float)(Math.Sin(i + Main.GameUpdateCount * 0.02f * a) + 1) * 0.5f,
                (float)(Math.Sin(j + i + -Main.GameUpdateCount * 0.03f * b) + 1) * 0.5f,
                (float)(Math.Sin((i * j) + Main.GameUpdateCount * 0.02f * c) + 1) * 0.5f);
        }
    }

    public class ScollingColors : TileCode
    {

        public override int MaxLengthSeconds => 95;

        public override int NextExtraDelaySeconds => -15;
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            drawColor = new Color((float)(Math.Sin(i * j * 0.15f) + 1) * 0.5f, (float)(Math.Cos(i + j * j * 0.855f) + 1) * 0.5f, (float)(Math.Sin((j ^ i) * 0.21f) + 1) * 0.5f) * (drawColor.ToVector3().Length() * 2);
        }
    }

    public class RandomFraming : TileCode
    {
        public override int MaxLengthSeconds => 65;
        public override int MinLengthSeconds => 25;

        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if(Main.rand.Next(35000) == 0)
            {
                Main.tile[i, j].frameX = (short)(Main.rand.Next(16) * 18);
                Main.tile[i, j].frameY = (short)(Main.rand.Next(16) * 18);
            }
        }
    }

    public class RandomPickTile : TileCode
    {

        public override int MaxLengthSeconds => 95;

        public override int NextExtraDelaySeconds => -15;
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (Main.rand.Next(100000) == 0)
            {
                Main.LocalPlayer.PickTile(i, j, Main.rand.Next(10, 101));
            }
        }
    }

    public class PlaceRandomTorch : TileCode
    {

        public override int MaxLengthSeconds => 30;
        public override int MinLengthSeconds => 10;

        public override int NextExtraDelaySeconds => -5;
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (Main.rand.Next(50000) == 0)
            {
                if ((WorldGen.TileEmpty(i, j - 1) || (Main.tileCut[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].type != TileID.Pots)) && WorldGen.SolidTileAllowBottomSlope(i, j))
                    WorldGen.PlaceTile(i, j - 1, TileID.Torches, true, false, Main.LocalPlayer.whoAmI, (Main.rand.Next(200) == 0 ? 14 : (Main.rand.Next(10) == 0 ? 12 : 0)));
            }
        }
    }
}
