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
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using static ChaosEdition.ChaosEdition;
using Terraria.Graphics.CameraModifiers;

namespace ChaosEdition
{
    public abstract class Code
    {
        public DateTime TimeCreatedAt = DateTime.Now;
        public TimeSpan TimeActiveSpan;

        //todo: this may need to store a seed from the server, that way nothing should need to be synced after creation

        public virtual int MaxLengthSeconds => 40;//this gets scaled based on adjusted time delay, this could be replaced by a float where default delay = 1.0
        public virtual int MinLengthSeconds => (int)(MaxLengthSeconds * 0.5f);
        public virtual int NextExtraDelaySeconds => 0;
        public abstract List<Code> ContainingList { get; }

        protected Dictionary<Type, bool> EffectBools => ChaosEdition.ActiveEffects;

        public Code()
        {
            Type type = GetType();
            if (!EffectBools[GetType()])
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)//server sends these values beforehand(or singleplayer)
                {
                    TimeActiveSpan = new TimeSpan(0, 0, (int)(Main.rand.Next(MinLengthSeconds, MaxLengthSeconds + 1) * TimeDelayScale));

                    ChaosEdition.TimeLastCodeSelected = DateTime.Now;
                    //this is added to support multiple being added at once, since it should be reset to zero before a new code instance is created, could cause issues if many are created
                    ChaosEdition.CurrentExtraDelay += new TimeSpan(0, 0, (int)(NextExtraDelaySeconds * TimeDelayScale));
                }

                if(Main.netMode == NetmodeID.Server)//sync new code and current timer to mp clients
                {
                    ModPacket modpacket = GetInstance<ChaosEdition>().GetPacket();
                    //modpacket.Write(256);//needs to be here for server(test without)

                    modpacket.WriteTime(0, true);
                    modpacket.WriteNewCode(type, TimeActiveSpan);
                    modpacket.Send();
                }


                EffectBools[GetType()] = true;
                ContainingList.Add(this);
            }
        }

        public virtual void OnRemove() { }

        public void Remove()
        {
            EffectBools[GetType()] = false;
            RemovalQueue.Enqueue((this, ContainingList));
            //if ((ContainingList).Contains(this))
            //    ContainingList.Remove(this);
            OnRemove();//maybe should be moved to when this is actually dequeued, but likely does not matter
        }

        public bool CheckActive()
        {
            if (TimeActiveSpan < DateTime.Now.Subtract(TimeCreatedAt))
            {
                Remove();
                return true;
            }
            return false;
        }

        //internal void SyncValue(ref dynamic value)
        //{
        //    if (Main.netMode == NetmodeID.Server)
        //    {
        //        ChaosEdition.SyncValueQueue.Enqueue(value);
        //    }
        //    else if (Main.netMode == NetmodeID.MultiplayerClient)
        //    {
        //        value = ChaosEdition.SyncValueQueue.Dequeue();
        //    }
        //    else
        //        throw new Exception("Multiplayer code running in singleplayer, report to mod dev.");
        //}
    }

    //done
    public abstract class MenuCode : Code
    {
        public sealed override List<Code> ContainingList => ChaosEdition.ActiveMenuCodes;
        public virtual void PostDrawInterface(SpriteBatch sb) { }
        public virtual void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) { }

        //public virtual void Update() { }
    }

    //done
    public abstract class NpcCode : Code
    {
        public sealed override List<Code> ContainingList => ChaosEdition.ActiveNpcCodes;
        public virtual void DrawEffects(NPC npc, ref Color drawColor, ModNPC modNpc = null) { }
        public virtual bool PreDraw(NPC npc, SpriteBatch sb, Color drawColor, ModNPC modNpc = null) { return true; }
        public virtual void PostDraw(NPC npc, SpriteBatch sb, Color drawColor, ModNPC modNpc = null) { }
        public virtual bool PreAI(NPC npc, ModNPC modNpc = null) { return true; }
        public virtual void AI(NPC npc, ModNPC modNpc = null) { }
    }

    //done
    public abstract class PlayerCode : Code
    {
        public sealed override List<Code> ContainingList => ChaosEdition.ActivePlayerCodes;
        public virtual void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) { }
        //public virtual void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers) { }
        public virtual void ModifyDrawInfo(ref PlayerDrawSet drawInfo) { }
        public virtual void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions) { }

        public virtual void PreDrawPlayer(SpriteBatch sb, Player player, ModPlayer modPlayer = null) { }
        public virtual void PostDrawPlayer(SpriteBatch sb, Player player, ModPlayer modPlayer = null) { }
        public virtual void PreUpdatePlayer(Player player, ModPlayer modPlayer = null) { }
        public virtual void PostUpdatePlayer(Player player, ModPlayer modPlayer = null) { }
        //public virtual void UpdateBiomeVisuals(Player player, ModPlayer modPlayer = null) { }
    }

    //done
    public abstract class TileCode : Code
    {

        public sealed override List<Code> ContainingList => ChaosEdition.ActiveTileCodes;

        public virtual void RandomUpdate(int i, int j, int type) { }

        public virtual void NearbyEffects(int i, int j, int type, bool closer) { }

        public virtual bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch) { return true; }

        public virtual void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData) { }

        public virtual void PostDraw(int i, int j, int type, SpriteBatch spriteBatch) { }

        public virtual void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) { }
        public virtual void KillWall(int i, int j, int type, ref bool fail) { }
    }

    //done
    public abstract class ItemCode : Code
    {
        public sealed override List<Code> ContainingList => ChaosEdition.ActiveItemCodes;
        public virtual void DrawArmorColor(EquipType type, int slot, Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) { }

        public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips) { }

        public virtual bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) { return true; }

        public virtual bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset) { return true; }

        public virtual void Update(Item item, ref float gravity, ref float maxFallSpeed) { }

        public virtual void UpdateInventory(Item item, Player player) { }

        public virtual bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) { return true; }

        public virtual bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) { return true; }
    }

    //
    public abstract class ProjectileCode : Code
    {
        public sealed override List<Code> ContainingList => ChaosEdition.ActiveProjectileCodes;

        public virtual void SetDefaults(Projectile projectile) { }

        public virtual bool PreAI(Projectile projectile) { return true; }

        public virtual void AI(Projectile projectile) { }

        public virtual bool PreDraw(Projectile projectile, ref Color lightColor) { return true; }

        public virtual void PostDraw(Projectile projectile, Color lightColor) { }

        public virtual void OnKill(Projectile projectile, int timeLeft) { }

    }

    //done
    public abstract class MiscCode : Code
    {
        public sealed override List<Code> ContainingList => ChaosEdition.ActiveMiscCodes;
        public virtual void Draw(SpriteBatch sb) { }
        public virtual void Update() { }
        public virtual void UpdateCamera(ref CameraInfo cameraInfo) { }
    }
}