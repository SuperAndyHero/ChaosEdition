using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ChaosEdition.Npcs
{
	public class SmileGhost : ModNPC
	{
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 300;
            NPC.defense = 100;
            NPC.lifeMax = 2000;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;

            PostSetDefaults();

            if (Main.hardMode)
            {
                NPC.lifeMax *= 15;
                NPC.defense *= 10;
                NPC.damage = 800;
            }
        }

        public virtual void PostSetDefaults() { }

        public virtual string musicstring => "ChaosEdition/Npcs/smileghost";
        public virtual int AdditionalTimeMinimum => 0;

        bool OnSpawnMultiplayerCheck = false;

        //public override void OnSpawn(IEntitySource source)
        //{
        //    NPC.ai[0] = Main.rand.Next((25 + AdditionalTimeMinimum) * 20, 150 * 20);
        //    Terraria.Chat.ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral(musicstring), Color.AliceBlue);
        //   soundStyleIgniteLoop = new SoundStyle(musicstring)
        //    {
        //        IsLooped = true,
        //        SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
        //        Volume = 0.65f,
        //        MaxInstances = 0
        //    };
        //    OnSpawnMultiplayerCheck = true;
        //    Terraria.Chat.ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral(soundStyleIgniteLoop.SoundPath), Color.AntiqueWhite);
        //}

        const float maxSpeed = 2.5f;
        const int fadeoutTime = 180;

        SlotId soundSlot;
        //bool played = false;
        SoundStyle soundStyleIgniteLoop;
        public class NPCAudioTracker
        {
            private int _expectedType;
            private int _expectedIndex;

            public NPCAudioTracker(NPC npc)
            {
                _expectedIndex = npc.whoAmI;
                _expectedType = npc.type;
            }

            public bool IsActiveAndInGame()
            {
                if (Main.gameMenu)
                    return false;

                NPC npc = Main.npc[_expectedIndex];
                if (npc.active)
                    return npc.type == _expectedType;

                return false;
            }
        }
        public virtual float maxspeed => 2.5f;
        public virtual float velrotinc => 0.8f;
        public virtual float speedinc => 0.03f;

        public override void AI()
        {
            if(OnSpawnMultiplayerCheck == false)
            {
                NPC.ai[0] = Main.rand.Next((75 + AdditionalTimeMinimum) * 20, 200 * 20);
                soundStyleIgniteLoop = new SoundStyle(musicstring)
                {
                    IsLooped = true,
                    SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
                    Volume = 0.65f,
                    MaxInstances = 0
                };
                OnSpawnMultiplayerCheck = true;
            }

            //if (Main.netMode != NetmodeID.Server && soundStyleIgniteLoop.SoundPath != null) {
            if (!SoundEngine.TryGetActiveSound(soundSlot, out var _))
            {
                var tracker = new NPCAudioTracker(NPC);
                soundSlot = SoundEngine.PlaySound(soundStyleIgniteLoop, NPC.position, soundInstance =>
                {
                    // The SoundUpdateCallback can be inlined if desired, such as in this example. Otherwise, LoopedSoundAdvanced shows the other approach
                    soundInstance.Position = NPC.position;
                    return tracker.IsActiveAndInGame();
                });

                //soundSlot = SoundEngine.PlaySound(soundStyleIgniteLoop, NPC.position);
            }
            //}

            NPC.ai[1]++;//used for tilt

            NPC.TargetClosestUpgraded(false);
            //NPC.frame = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)(NPC.Size.X), (int)(NPC.Size.Y));
            NPC.rotation = (float)Math.Sin((Main.GameUpdateCount / 100f) + ((float)NPC.whoAmI * 2)) * 0.3f;
            if (NPC.HasValidTarget)
            {
                Vector2 targetPos = NPC.targetRect.Center.ToVector2();
                float velrot = (float)Math.Sin((NPC.ai[1] / 75f) + Math.Pow((float)NPC.whoAmI, 2)) * velrotinc;
                NPC.velocity += Vector2.Normalize(targetPos - NPC.Center).RotatedBy(velrot) * speedinc;

                if (NPC.velocity.Length() > maxspeed)
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * maxSpeed;
            }
            else
            {
                if(NPC.ai[0] > fadeoutTime)
                    NPC.ai[0] = fadeoutTime;
            }

            if (NPC.ai[0] <= fadeoutTime)
            {
                NPC.Opacity = (NPC.ai[0] / (fadeoutTime * 2));
                SoundEngine.TryGetActiveSound(soundSlot, out ActiveSound sound);
                sound.Volume = NPC.Opacity;
            }
            else
                NPC.Opacity = 0.65f;

            //Main.NewText(NPC.ai[0]);

            if (NPC.ai[0] <= 0)
                NPC.active = false;

            NPC.ai[0]--;//timer until it despawns. once its below 'fadeoutTime' it will also use it for opacity
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            NPC.ai[0] += (hit.Damage) * 5;
        }

        public override void DrawEffects(ref Color drawColor)
        {
            drawColor = new Color(drawColor.R + 25, drawColor.G + 25, drawColor.B + 25, drawColor.A);
        }
        public override void OnKill()
        {
            NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<Npcs.AngryGhost>());
        }
    }
}