using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ChaosEdition
{
    public class MiscTest : MiscCode
    {
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Main.blackTileTexture, new Rectangle(0, 0, 30, 30), Color.Red);
        }

        public override void Update()
        {
            Main.NewText("update");
        }

        public MiscTest() : base()
        {
            Main.NewText("start");
        }

        public override int MaxLengthSeconds => 0;

        public override int NextExtraDelaySeconds => 0;
    }

    //public class MethodSwapCode : MiscCode
    //{
    //    public MethodSwapCode() : base()
    //    {
    //        ChaosEdition.SwapRandomMethods(1);
    //    }

    //    public override int MaxLengthSeconds => 0;

    //    public override int NextExtraDelaySeconds => 0;
    //}
}
