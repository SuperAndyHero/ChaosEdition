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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosEdition
{
    //public class MiscTest : MiscCode
    //{
    //    public override void Draw(SpriteBatch sb)
    //    {
    //        sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, 30, 30), Color.Red);
    //    }

    //    public override void Update()
    //    {
    //        Main.NewText("update");
    //    }

    //    public MiscTest() : base()
    //    {
    //        Main.NewText("start");
    //    }

    //    public override int MaxLengthSeconds => 0;

    //    public override int NextExtraDelaySeconds => 0;
    //}

    #region tile effects
    public class InvertRandomTileCut : MiscCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => -25;
        bool ran = false;
        public override void Update()
        {
            if (!ran)
            {
                int index = Main.rand.Next(Main.tileCut.Length);
                Main.tileCut[index] = !Main.tileCut[index];
                ran = true;
            }
        }
    }

    public class MakeRandomTilesBouncy : MiscCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => -30;
        bool ran = false;
        public override void Update()
        {
            if (!ran)
            {
                int tileChangeCount = Main.rand.Next(1, 6);
                for (int i = 0; i < tileChangeCount; i++)
                {
                    int index = Main.rand.Next(Main.tileBouncy.Length);
                    Main.tileBouncy[index] = true;
                }
                ran = true;
            }
        }
    }
    #endregion

    #region screen effects
    public class InvertScreen : MiscCode
    {
        public override int MaxLengthSeconds => 20;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -10;
    }

    public class ScreenGameboy : MiscCode
    {
        public override int MaxLengthSeconds => 40;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -12;
    }

    public class ScreenRed : MiscCode
    {
        public override int MaxLengthSeconds => 60;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -15;
    }

    public class ScreenMoonlord : MiscCode
    {
        public override int MaxLengthSeconds => 30;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -5;
    }
    #endregion

    public class GravityFlip : MiscCode
    {
        public override int MaxLengthSeconds => 60;
        public override int MinLengthSeconds => 10;

        public override int NextExtraDelaySeconds => 20;
    }

    public class ChangeAnglerQuest : MiscCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => -15;
        bool ran = false;
        public override void Update()
        {
            if (!ran)
            {
                Main.AnglerQuestSwap();
                ran = true;
            }
        }
    }

    //public class SwapChests : MiscCode
    //{
    //    public override int MaxLengthSeconds => 1;
    //    public override int MinLengthSeconds => 1;

    //    public override int NextExtraDelaySeconds => -15;
    //    bool ran = false;
    //    public override void Update()
    //    {
    //        if (!ran)
    //        {
    //            int index
    //            Item[] orig = 
    //            Main.chest[0].
    //            Main.maxChests
    //            ran = true;
    //        }
    //    }
    //}

    public class DelayCameraPosition : MiscCode
    {
        public override int MaxLengthSeconds => 60;
        public override int MinLengthSeconds => 10;

        public override int NextExtraDelaySeconds => -15;

        bool ran = false;
        Vector2[] vec2array;
        int arrayLength;

        public override void UpdateCamera(ref CameraInfo cameraInfo)
        {
            if (!ran)
            {
                arrayLength = Main.rand.Next(20, 121);
                vec2array = Enumerable.Repeat(cameraInfo.CameraPosition, arrayLength).ToArray();
                ran = true;
            }

            vec2array[0] = cameraInfo.CameraPosition;

            for (int i = (arrayLength - 1); i > 0; i--)
            {
                vec2array[i] = vec2array[i - 1];
            }

            cameraInfo.CameraPosition = vec2array[arrayLength - 1];
        }
    }

    public class FreezeCameraPosition : MiscCode
    {
        public override int MaxLengthSeconds => 8;
        public override int MinLengthSeconds => 2;


        bool ran = false;
        Vector2 pos;

        public override void UpdateCamera(ref CameraInfo cameraInfo)
        {
            if (!ran)
            {
                pos = cameraInfo.CameraPosition;
                ran = true;
            }

            cameraInfo.CameraPosition = pos;
        }
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
