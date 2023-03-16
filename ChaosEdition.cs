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

namespace ChaosEdition
{
	public class ChaosEdition : Mod
	{
        public static List<Code> MenuCodes =        new List<Code>();
        public static List<Code> NpcCodes =         new List<Code>();
        public static List<Code> PlayerCodes =      new List<Code>();
        public static List<Code> TileCodes =        new List<Code>();
        public static List<Code> ItemCodes =        new List<Code>();
        public static List<Code> ProjectileCodes =  new List<Code>();
        public static List<Code> MiscCodes =        new List<Code>();

        public static List<(Code code, List<Code> list)> RemovalList = new List<(Code code, List<Code> list)>();

        public static DateTime LastCode = DateTime.Now;
        public static TimeSpan NewCodeDelay => new TimeSpan(0, 0, 15);
        public static TimeSpan RetryCodeDelay => new TimeSpan(0, 0, 10);
        public static int MaxActiveCodes = 10;
        public static int ActiveCodeCount => MenuCodes.Count + NpcCodes.Count + PlayerCodes.Count + TileCodes.Count + ItemCodes.Count + ProjectileCodes.Count + MiscCodes.Count;

        public static TimeSpan CurrentExtraDelay = new TimeSpan();

        public static TimeSpan TimeUntilNext => LastCode.Subtract(DateTime.Now.Subtract(NewCodeDelay).Subtract(CurrentExtraDelay));

        //public static List<List<Code>> ab = new List<List<Code>>();
        public static Dictionary<Type, bool> ActiveEffects = new Dictionary<Type, bool>();
        public static Type[] CodeTypes;
        public static bool Loaded = false;


        public const bool DrawActiveCodes = true;
        public const bool DrawFullCodeList = false;
        public static bool AutoSelectingCodes => false;//disble for testing


        //public static List<Detour> ActiveDetours = new List<Detour>();//for random method swapping

        public override void Load()
        {
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_RenderAllLayers += PostDrawPlayerLayer;

            IEnumerable<Type> arr = typeof(Code).Assembly.GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(typeof(Code)));
            foreach (Type type in arr)
                ActiveEffects.Add(type, false);
            CodeTypes = arr.ToArray();
            //MonoModHooks.RequestNativeAccess();//needed for detours
            Filters.Scene["ChaosEdition:Moonlord"] = new Filter(new ScreenShaderData("FilterMoonLordShake"), EffectPriority.High);
            Loaded = true;
        }

        private void PostDrawPlayerLayer(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
        {
            //Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default, default);
            foreach(Player player in Main.player)
            {
                if (player.active)
                    foreach (PlayerCode code in PlayerCodes)
                        code.PreDrawPlayer(Main.spriteBatch, player);
            }
            //Main.spriteBatch.End();

            orig(ref drawinfo);

            //Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default, default);
            foreach (Player player in Main.player)
            {
                if (player.active)
                    foreach (PlayerCode code in PlayerCodes)
                        code.PostDrawPlayer(Main.spriteBatch, player);
            }
            //Main.spriteBatch.End();
        }

        public static void ClearAllCodes()
        {
            foreach (MenuCode code in MenuCodes)
                code.Remove();

            foreach (NpcCode code in NpcCodes)
                code.Remove();

            foreach (PlayerCode code in PlayerCodes)
                code.Remove();

            foreach (TileCode code in TileCodes)
                code.Remove();

            foreach (ItemCode code in ItemCodes)
                code.Remove();

            foreach (ProjectileCode code in ProjectileCodes)
                code.Remove();

            foreach (MiscCode code in MiscCodes)
                code.Remove();
        }

        /*
        //if used add mod swaps too
        //warning: this is a very unstable method that will crash the game eventually, or even corrupt save files
        public static void SwapRandomMethods(int countToSwap)
        {
            try
            {
                //Assembly assmtest = Assembly.GetEntryAssembly();
                //Assembly assmtest2 = Assembly.GetAssembly(typeof(Terraria.ModLoader.);
                Type[] types = Assembly.GetEntryAssembly().GetTypes();

                List<MethodInfo> validMethods = new List<MethodInfo>();
                List<MethodInfo> validMethods2 = new List<MethodInfo>();
                foreach (Type type in types)
                {
                    MethodInfo[] methods1 = type.GetMethods();
                    //methods1.Shuffle();
                    foreach (MethodInfo info in methods1)
                    {
                        if (info.IsStatic && !info.IsAbstract && !info.IsGenericMethod && !(type is ModItem || type is ModNPC || type is ModProjectile || type is ModDust))
                        {
                            validMethods.Add(info);
                            validMethods2.Add(info);
                        }
                    }
                }

                validMethods.Shuffle();
                validMethods2.Shuffle();

                List<(MethodInfo, MethodInfo)> validPairs = new List<(MethodInfo, MethodInfo)>();

                int foundPairs = 0;

                foreach (MethodInfo info1 in validMethods)//checks every static methods in both classes
                {
                    if (info1.IsGenericMethod)//cannot be generic
                        continue;
                    if (info1.IsAbstract)//cannot be generic
                        continue;

                    ParameterInfo[] params1 = info1.GetParameters();
                    foreach (MethodInfo info2 in validMethods2)
                    {
                        if (info2.IsGenericMethod)//cannot be generic and returns must match and cannot be same method
                            continue;
                        if (info1.ReturnType != info2.ReturnType)//cannot be generic and returns must match and cannot be same method
                            continue;
                        if (info1 == info2 || info1.Name == info2.Name)//cannot be generic and returns must match and cannot be same method
                            continue;
                        if (info1.DeclaringType != info2.DeclaringType || info1.IsStatic != info2.IsStatic)//cannot be generic and returns must match and cannot be same method
                            continue;
                        if (info1.IsAbstract)//cannot be generic
                            continue;

                        ParameterInfo[] params2 = info2.GetParameters();
                        if (params1.Length == params2.Length)//if param length matches
                        {
                            bool match = true;
                            for (int i = 0; i < params2.Length; i++)//if every type matchs or they have no params
                            {
                                match = match && params1[i].ParameterType == params2[i].ParameterType;
                            }
                            if (match)
                            {
                                validPairs.Add((info1, info2));
                                foundPairs++;
                                break;
                            }
                        }
                    }
                    if (foundPairs >= countToSwap)
                        break;
                }

                if (validPairs.Count > 0)
                {
                    int count = Math.Min(validPairs.Count, countToSwap);
                    for (int i = 0; i < countToSwap; i++)//if every type matchs or they have no params
                    {

                        var pair = validPairs[Main.rand.Next(validPairs.Count)];
                        if (Main.rand.Next(50) == 0)
                        {
                            //ActiveDetours.Add(new Detour(pair.Item1, pair.Item2));
                            //ActiveDetours.Add(new Detour(pair.Item2, pair.Item1));
                        }
                        else
                        {
                            if (Main.rand.NextBool())
                                ActiveDetours.Add(new Detour(pair.Item1, pair.Item2));
                            else
                                ActiveDetours.Add(new Detour(pair.Item2, pair.Item1));
                        }
                        Main.NewText("Methods swapped. New count: " + ActiveDetours.Count + " From: " + pair.Item1.Name + " To: " + pair.Item2.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.NewText(ex.Message);
                foreach (Detour g in ActiveDetours)
                    g.Dispose();
                ActiveDetours.Clear();
            }
        }
        public static void ClearSwappedMethods()
        {
            foreach (MonoMod.RuntimeDetour.Detour g in ChaosEdition.ActiveDetours)//for random method swapping
                g.Dispose();
            ChaosEdition.ActiveDetours.Clear();//for random method swapping
            Main.NewText("Cleared");
        }
        */
    }

    public class ChaosEditionSystem : ModSystem
    {
        //updates and removes codes, also selects codes if timer is up
        public override void PostUpdateInput()
        {
            foreach (MenuCode code in MenuCodes)
                code.Check();

            foreach (NpcCode code in NpcCodes)
                code.Check();

            foreach (PlayerCode code in PlayerCodes)
                code.Check();

            foreach (TileCode code in TileCodes)
                code.Check();

            foreach (ItemCode code in ItemCodes)
                code.Check();

            foreach (ProjectileCode code in ProjectileCodes)
                code.Check();

            foreach (MiscCode code in MiscCodes)
                code.Check();

            foreach ((Code code, List<Code> list) pair in RemovalList)
                pair.list.Remove(pair.code);

            if (AutoSelectingCodes && ActiveCodeCount < MaxActiveCodes && TimeUntilNext.TotalSeconds <= 0 && Loaded)
            {
                CurrentExtraDelay = new TimeSpan();
                int tries = 0;
                const int maxTries = 5;
                while (tries < maxTries)
                {
                    int index = Main.rand.Next(CodeTypes.Length);
                    if (!ActiveEffects[CodeTypes[index]])
                    {
                        Activator.CreateInstance(CodeTypes[index]);
                        return;
                    }
                    else
                        tries++;
                    if (tries >= maxTries)
                    {
                        LastCode = DateTime.Now.Subtract(NewCodeDelay);//.Subtract(CodeDelay).Add(RetryCodeDelay);
                        CurrentExtraDelay += RetryCodeDelay;
                        return;
                    }
                }
            }


            foreach (MiscCode code in MiscCodes)
                code.Update();
        }

        //hook for codes that modify interface layers
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //ChaosEdition.ClearSwappedMethods();
            foreach (MenuCode code in MenuCodes)
                code.ModifyInterfaceLayers(layers);
        }

        //countdown timer and debug active codes list
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            foreach (MenuCode code in MenuCodes)
                code.PostDrawInterface(spriteBatch);

            foreach (MiscCode code in MiscCodes)
                code.Draw(spriteBatch);

            Utils.DrawBorderString(spriteBatch, "Time since last: " + LastCode.ToShortTimeString(), new Vector2(5, 15), Color.White * 0.75f);
            Utils.DrawBorderString(spriteBatch, "Time till next: " + Math.Max(0, (int)TimeUntilNext.TotalSeconds), new Vector2(5, 30), Color.White * 0.75f);

            if (DrawActiveCodes)
            {
                int count = 0;
                foreach (KeyValuePair<Type, bool> pair in ActiveEffects)
                {
                    if (DrawFullCodeList)
                    {
                        Color color = pair.Value ? new Color(100, 255, 100) : (count % 2 == 0 ? Color.Tomato : Color.CornflowerBlue);
                        Utils.DrawBorderString(spriteBatch, pair.Key.Name + " : " + pair.Value, new Vector2(5, 45 + (15 * count)), color);
                        count++;
                    }
                    else if (pair.Value)
                    {
                        Utils.DrawBorderString(spriteBatch, pair.Key.Name, new Vector2(5, 45 + (15 * count)), new Color(100, 255, 100));
                        count++;
                    }
                }
            }
        }
    }


    public abstract class Code
    {
        public DateTime CreatedAt = DateTime.Now;
        public TimeSpan Length;
        public virtual int MaxLengthSeconds => 40;
        public virtual int MinLengthSeconds => (int)(MaxLengthSeconds * 0.5f);
        public virtual int NextExtraDelaySeconds => 0;
        public abstract List<Code> ContainingList { get; }

        protected Dictionary<Type, bool> EffectBools => ChaosEdition.ActiveEffects;

        public Code()
        {
            if (!EffectBools[GetType()])
            {
                Length = new TimeSpan(0, 0, Main.rand.Next(MinLengthSeconds, MaxLengthSeconds + 1));
                ChaosEdition.LastCode = DateTime.Now;
                ChaosEdition.CurrentExtraDelay = new TimeSpan(0, 0, NextExtraDelaySeconds);
                EffectBools[GetType()] = true;
                ContainingList.Add(this);
            }
        }

        public virtual void OnRemove() { }

        public void Remove() 
        {
            EffectBools[GetType()] = false;
            if((ContainingList).Contains(this))
                ContainingList.Remove(this);
            OnRemove();
        }

        public bool Check()
        {
            if(Length < DateTime.Now.Subtract(CreatedAt))
            {
                Remove();
                return true;
            }
            return false;
        }
    }

    //done
    public abstract class MenuCode : Code {
        public sealed override List<Code> ContainingList => ChaosEdition.MenuCodes;
        public virtual void PostDrawInterface(SpriteBatch sb) { } 
        public virtual void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) { }

        //public virtual void Update() { }
    }

    //done
    public abstract class NpcCode : Code {
        public sealed override List<Code> ContainingList => ChaosEdition.NpcCodes;
        public virtual void DrawEffects(NPC npc, ref Color drawColor, ModNPC modNpc = null) { }
        public virtual bool PreDraw(NPC npc, SpriteBatch sb, Color drawColor, ModNPC modNpc = null) { return true; }
        public virtual void PostDraw(NPC npc, SpriteBatch sb, Color drawColor, ModNPC modNpc = null) { }
        public virtual bool PreAI(NPC npc, ModNPC modNpc = null) { return true; }
        public virtual void AI(NPC npc, ModNPC modNpc = null) { }
    }

    //done
    public abstract class PlayerCode : Code {
        public sealed override List<Code> ContainingList => ChaosEdition.PlayerCodes;
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
    public abstract class TileCode : Code {

        public sealed override List<Code> ContainingList => ChaosEdition.TileCodes;

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
        public sealed override List<Code> ContainingList => ChaosEdition.ItemCodes;
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
        public sealed override List<Code> ContainingList => ChaosEdition.ProjectileCodes;

        public virtual void SetDefaults(Projectile projectile) { }

        public virtual bool PreAI(Projectile projectile) { return true; }

        public virtual void AI(Projectile projectile) { }

        public virtual bool PreDraw(Projectile projectile, ref Color lightColor) { return true; }

        public virtual void PostDraw(Projectile projectile, Color lightColor) { }

        public virtual void Kill(Projectile projectile, int timeLeft) { }

    }

    //done
    public abstract class MiscCode : Code {
        public sealed override List<Code> ContainingList => ChaosEdition.MiscCodes;
        public virtual void Draw(SpriteBatch sb) { }
        public virtual void Update() { }
    }
}