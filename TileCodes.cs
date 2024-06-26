using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition
{
    //broken by update
    //public class RandomColors : TileCode
    //{
    //    [NetSync]
    //    public float a = Main.rand.NextFloat(-3f, 3f);
    //    [NetSync]
    //    public float b = Main.rand.NextFloat(-3f, 3f);
    //    [NetSync]
    //    public float c = Main.rand.NextFloat(-3f, 3f);

    //    public override int MaxLengthSeconds => 90;

    //    public override int NextExtraDelaySeconds => -15;
    //    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    //    {
    //        //Main.GameUpdateCount is different for each client, but it doesn't matter
    //        drawData.colorTint = new Color(
    //            (float)(Math.Sin(i + Main.GameUpdateCount * 0.02f * a) + 1) * 0.5f,
    //            (float)(Math.Sin(j + i + -Main.GameUpdateCount * 0.03f * b) + 1) * 0.5f,
    //            (float)(Math.Sin((i * j) + Main.GameUpdateCount * 0.02f * c) + 1) * 0.5f);
    //    }
    //}
    
    //broken by update
    //public class ScollingColors : TileCode
    //{
    //    public override int MaxLengthSeconds => 95;
    //    public override int NextExtraDelaySeconds => -12;
    //    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    //    {
    //        drawData.colorTint = new Color(
    //            (float)(Math.Sin(i * j * 0.15f) + 1) * 0.5f, 
    //            (float)(Math.Cos(i + j * j * 0.855f) + 1) * 0.5f, 
    //            (float)(Math.Sin((j ^ i) * 0.21f) + 1) * 0.5f);
    //    }
    //}

    public class RandomFramingChange : TileCode
    {
        public override int? MaxLengthSeconds => 65;
        public override int? MinLengthSeconds => 25;
        public override int? NextExtraDelaySeconds => -20;

        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            if(Main.rand.NextBool(30000))
            {
                Main.tile[i, j].TileFrameX = (short)(Main.rand.Next(16) * 18);
                Main.tile[i, j].TileFrameY = (short)(Main.rand.Next(16) * 18);
                NetMessage.SendData(MessageID.TileSquare, Main.myPlayer, -1, null, i, j, 1, 1, (int)TileChangeType.None);//doesn't use SendTileSquare since that reframes tiles, also has a internal netmode check
            }
        }
    }

    public class RandomPickTile : TileCode
    {

        public override int? MaxLengthSeconds => 95;

        public override int? NextExtraDelaySeconds => -12;
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (Main.netMode == NetmodeID.Server)//clientside
                return;

            if (Main.rand.NextBool(100000))
            {
                //Main.LocalPlayer.PickTile(i, j, 0);annoying and breaks houses, maybe re-add if a destructive mode is added
                WorldGen.KillTile(i, j, true);
            }
        }
    }

    public class PlaceRandomTorch : TileCode
    {

        public override int? MaxLengthSeconds => 30;
        public override int? MinLengthSeconds => 10;

        public override int? NextExtraDelaySeconds => -5;
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (Main.rand.NextBool(50000))
            {
                if ((WorldGen.TileEmpty(i, j - 1) || (Main.tileCut[Main.tile[i, j - 1].TileType] && Main.tile[i, j - 1].TileType != TileID.Pots)) && WorldGen.SolidTileAllowBottomSlope(i, j))
                {
                    WorldGen.PlaceTile(i, j - 1, TileID.Torches, true, false, Main.myPlayer, (Main.rand.NextBool(200) ? 14 : (Main.rand.NextBool(10) ? 12 : 0)));
                    NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1, TileChangeType.None);
                }
            }
        }
    }

    public class WormsFromDirt : TileCode
    {

        public override int? MaxLengthSeconds => 60;
        public override int? MinLengthSeconds => 30;

        public override int? NextExtraDelaySeconds => -10;
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if ((type == TileID.Dirt || type == TileID.Mud || type == TileID.Grass || type == TileID.JungleGrass) && Main.rand.NextBool(2))
            {
                Vector2 pos = new Vector2((i * 16) + 8, j * 16);
                Vector2 playerPos = HelperMethods.NearestPlayerCenter(pos);

                if(pos.Distance(playerPos) < 1000)
                {
                    bool nightcrawller = Main.rand.NextBool(100) && !Main.dayTime;
                    bool gold = Main.rand.NextBool(1000);
                    NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)pos.X, (int)pos.Y, gold ? NPCID.GoldWorm : nightcrawller ? NPCID.EnchantedNightcrawler : NPCID.Worm);
                }

            }
        }
    }

    public class SkeletonsFromGraves : TileCode
    {

        public override float MaxLength => 5;
        public override float MinLength => 2;

        public override float NextExtraDelay => -0.66f;
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (type == TileID.Tombstones && Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY == 0 && fail == false)
            {
                Vector2 pos = new Vector2((i * 16) + 8, (j * 16) - 16);
                Vector2 playerPos = HelperMethods.NearestPlayerCenter(pos);

                if (pos.Distance(playerPos) < 1000)
                {
                    NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)pos.X, (int)pos.Y, NPCID.Skeleton);
                }

            }
        }
    }

    public class GraveRobbery : TileCode
    {

        public override int? MaxLengthSeconds => 45;
        public override int? MinLengthSeconds => 25;

        public override int? NextExtraDelaySeconds => -5;
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (type == TileID.Tombstones && Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY == 0 && fail == false)
            {
                Vector2 pos = new Vector2((i * 16) + 8, (j * 16) + 8);
                Vector2 playerPos = HelperMethods.NearestPlayerCenter(pos);

                if (pos.Distance(playerPos) < 1000)
                {
                    Item.NewItem(Item.GetSource_None(), pos, ItemID.SilverCoin, Main.rand.Next(5, 110));
                }

            }
        }
    }
}
