using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace ChaosEdition
{
    public class ChaosEditionGlobalTile : GlobalTile
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            foreach (TileCode code in ChaosEdition.TileCodes)
                code.RandomUpdate(i, j, type);
        }
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            foreach (TileCode code in ChaosEdition.TileCodes)
                code.NearbyEffects(i, j, type, closer);
        }
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            bool DrawTile = base.PreDraw(i, j, type, spriteBatch);
            foreach (TileCode code in ChaosEdition.TileCodes)
                DrawTile = DrawTile && code.PreDraw(i, j, type, spriteBatch);
            return DrawTile;
        }
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            foreach (TileCode code in ChaosEdition.TileCodes)
                code.DrawEffects(i, j, type, spriteBatch, ref drawData);
        }
        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            foreach (TileCode code in ChaosEdition.TileCodes)
                code.PostDraw(i, j, type, spriteBatch);
        }
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            foreach (TileCode code in ChaosEdition.TileCodes)
                code.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);
        }
    }

    public class ChaosEditionGlobalWall : GlobalWall
    {
        public override void KillWall(int i, int j, int type, ref bool fail)
        {
            foreach (TileCode code in ChaosEdition.TileCodes)
                code.KillWall(i, j, type, ref fail);
        }
    }
}
