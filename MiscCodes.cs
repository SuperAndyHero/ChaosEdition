﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoMod.RuntimeDetour;
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
using Terraria.ID;
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
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override int? NextExtraDelaySeconds => -25;

        public override float SelectionWeight => 1.1f;

        [NetSync]
        public int index = Main.rand.Next(Main.tileCut.Length);

        bool ran = false;

        public override void Update()
        {
            if (!ran)
            {
                Main.tileCut[index] = !Main.tileCut[index];
                ran = true;
            }
        }
    }

    //TODO: sync change amount and maybe a seed fort he random, just made sure all are synced/determainate
    public class MakeRandomTilesBouncy : MiscCode
    {
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override float NextExtraDelay => -1;

        public override float SelectionWeight => 1.1f;

        [NetSync]
        public int index = Main.rand.Next(Main.tileBouncy.Length);

        bool ran = false;
        public override void Update()
        {
            if (!ran)
            {
                //int tileChangeCount = Main.rand.Next(1, 6);
                //for (int i = 0; i < tileChangeCount; i++)
                //{
                    //int index = Main.rand.Next(Main.tileBouncy.Length);
                    Main.tileBouncy[index] = true;
                //}
                ran = true;
            }
        }
    }
    #endregion

    #region screen effects
    public class InvertScreen : MiscCode
    {
        public override float MaxLength => 0.8f;
        public override float MinLength => 0.5f;

        public override float NextExtraDelay => -0.45f;

        public override float SelectionWeight => 0.85f;
    }

    public class ScreenGameboy : MiscCode
    {
        public override int? MaxLengthSeconds => 40;
        public override int? MinLengthSeconds => 15;

        public override int? NextExtraDelaySeconds => -12;

        public override float SelectionWeight => 0.85f;
    }

    public class ScreenRed : MiscCode
    {
        public override int? MaxLengthSeconds => 60;
        public override int? MinLengthSeconds => 15;

        public override int? NextExtraDelaySeconds => -15;
    }

    public class ScreenMoonlord : MiscCode
    {
        public override int? MaxLengthSeconds => 30;
        public override int? MinLengthSeconds => 15;

        public override int? NextExtraDelaySeconds => -5;
    }
    #endregion

    public class GravityFlip : MiscCode
    {
        public override int? MaxLengthSeconds => 60;
        public override int? MinLengthSeconds => 10;

        public override int? NextExtraDelaySeconds => 20;

        public override float SelectionWeight => 0.9f;
    }

    public class ChangeAnglerQuest : MiscCode
    {
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override int? NextExtraDelaySeconds => -15;

        public override float SelectionWeight => 0.8f;

        bool ran = false;
        public override void Update()
        {
            if (!ran)
            {
                Main.AnglerQuestSwap();//has internal netmode check
                ran = true;
            }
        }
    }

    //public class SwapChests : MiscCode//todo: use null check to see if chest exists
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
        public override int? MaxLengthSeconds => 60;
        public override int? MinLengthSeconds => 10;

        public override int? NextExtraDelaySeconds => -15;

        bool ran = false;
        Vector2[] vec2array;

        [NetSync]
        public int arrayLength = Main.rand.Next(20, 121);//not used on server, but synced for all clients to share the same

        public override void UpdateCamera(ref CameraInfo cameraInfo)
        {
            if (Main.netMode == NetmodeID.Server)//unsure if UpdateCamena runs on server
                return;

            if (!ran)
            {
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
        public override int? MaxLengthSeconds => 8;
        public override int? MinLengthSeconds => 2;


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

    public class EnemyColors : MiscCode
    {
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override int? NextExtraDelaySeconds => -30;
        //public override float SelectionWeight => 0.9f;

        [NetSync]
        public int seed = Main.rand.Next(10000000);

        bool ran = false;
        Random rand;

        public override void Update()
        {
            if (!ran)
            {
                Random rand = new Random(seed);
                foreach (NPC npc in Main.npc)
                {
                    npc.color = new Color(rand.Next(32, 256), rand.Next(32, 256), rand.Next(32, 256));
                }
                ran = true;
            }
        }
    }

    public class InvertPlayerVelocty : MiscCode
    {
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override float NextExtraDelay => -1.2f ;

        public override float SelectionWeight => 0.75f;

        bool ran = false;

        public override void Update()
        {
            if (!ran)
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active)
                        continue;

                    player.velocity = -player.velocity;
                }
                ran = true;
            }
        }
    }

    public class InvertProjectileVelocty : MiscCode
    {
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override float NextExtraDelay => -1.2f;

        public override float SelectionWeight => 0.75f;

        bool ran = false;

        public override void Update()
        {
            if (!ran)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (!proj.active)
                        continue;

                    proj.velocity = -proj.velocity;
                }
                ran = true;
            }
        }
    }

    public class InvertNpcVelocity : MiscCode
    {
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override float NextExtraDelay => -1.2f;

        public override float SelectionWeight => 0.75f;

        bool ran = false;

        public override void Update()
        {
            if (!ran)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active)
                        continue;

                    npc.velocity = -npc.velocity;
                }
                ran = true;
            }
        }
    }

    public class InvertAllVelocity : MiscCode
    {
        public override int? MaxLengthSeconds => 1;
        public override int? MinLengthSeconds => 1;

        public override float NextExtraDelay => -1.2f;

        public override float SelectionWeight => 0.75f;

        bool ran = false;

        public override void Update()
        {
            if (!ran)
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active)
                        continue;

                    player.velocity = -player.velocity;
                }

                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active)
                        continue;

                    npc.velocity = -npc.velocity;
                }

                foreach (Projectile proj in Main.projectile)
                {
                    if (!proj.active)
                        continue;

                    proj.velocity = -proj.velocity;
                }
                ran = true;
            }
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
