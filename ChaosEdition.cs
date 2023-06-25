using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static ChaosEdition.ChaosEdition;
using System.IO;
using Terraria.Graphics.CameraModifiers;

namespace ChaosEdition
{
	public class ChaosEdition : Mod
	{
        public static List<Code> ActiveMenuCodes =        new List<Code>();
        public static List<Code> ActiveNpcCodes =         new List<Code>();
        public static List<Code> ActivePlayerCodes =      new List<Code>();
        public static List<Code> ActiveTileCodes =        new List<Code>();
        public static List<Code> ActiveItemCodes =        new List<Code>();
        public static List<Code> ActiveProjectileCodes =  new List<Code>();
        public static List<Code> ActiveMiscCodes =        new List<Code>();

        public static Queue<(Code CodeToRemove, List<Code> ListToRemoveFrom)> RemovalQueue = new Queue<(Code CodeToRemove, List<Code> ListToRemoveFrom)>();
        

        public static int ActiveCodeCount => ActiveMenuCodes.Count + ActiveNpcCodes.Count + ActivePlayerCodes.Count + ActiveTileCodes.Count + ActiveItemCodes.Count + ActiveProjectileCodes.Count + ActiveMiscCodes.Count;

        #region time
        public static DateTime TimeLastCodeSelected = DateTime.Now;

        public static TimeSpan CurrentExtraDelay = new TimeSpan();

        public static TimeSpan TimeSpanUntilNext => TimeLastCodeSelected.Subtract(DateTime.Now.Subtract(NewCodeDelay).Subtract(CurrentExtraDelay));


        //config option that these 2 use
        public static TimeSpan NewCodeDelay => new TimeSpan(0, 0, ConfigDelayBetweenCodes);//time between code selection
        public static TimeSpan RetryCodeDelay => new TimeSpan(0, 0, Math.Max(1, (int)(ConfigDelayBetweenCodes * 0.6666f)));//added to extra delay if this fails to select a code
        #endregion

        //public static List<List<Code>> ab = new List<List<Code>>();
        public static Dictionary<Type, int> CodeTypeID = new Dictionary<Type, int>();
        public static Dictionary<Type, bool> ActiveEffects = new Dictionary<Type, bool>();//a bool for every type to quickly check if an effect is active
        //public static Dictionary<Type, Code> ActiveEffectInstance = new Dictionary<Type, Code>();
        public static Type[] CodeTypes;//an array of every type
        public static bool IsModLoaded = false;

        //todo: see about replacing the second delay on codes with a float delay
        public const int DefaultDelayBetweenCodes = 25;
        public static float TimeDelayScale => (float)ConfigDelayBetweenCodes / (float)DefaultDelayBetweenCodes;

        #region config options
        //clientside
        public static bool ConfigDrawCountdownTimer = true;
        public static bool ConfigDrawActiveCodeCount = true;
        public static bool ConfigDrawActiveCodes = false;
        public static bool ConfigDrawActiveTimes = false;
        public static bool ConfigDrawFullCodeList = false;

        public static int ConfigDelayBetweenCodes = DefaultDelayBetweenCodes;


        //serverside
        public static bool ConfigAutoSelectingCodes = true;//disable for testing
        public static int ConfigMaxActiveCodes = 10;
        #endregion

        //todo: split this into another mod, make swaps determainistic, make it possible to undo swaps, call it terraria corruptor or something
        //public static List<Detour> ActiveDetours = new List<Detour>();//for random method swapping

        public static CameraModifier CameraModifier;

        //public static Queue<dynamic> SyncValueQueue;

        public override void Load()
        {
            Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += PostDrawPlayerLayer;

            IEnumerable<Type> arr = typeof(Code).Assembly.GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(typeof(Code)));
            int arrID = 0;
            foreach (Type type in arr)
            {
                CodeTypeID.Add(type, arrID);
                ActiveEffects.Add(type, false);
                arrID++;
            }
            CodeTypes = arr.ToArray();

            CameraModifier = new();

            //SyncValueQueue = new Queue<dynamic>();

            //MonoModHooks.RequestNativeAccess();//needed for detours
            Filters.Scene["ChaosEdition:Moonlord"] = new Filter(new ScreenShaderData("FilterMoonLordShake"), EffectPriority.High);
            IsModLoaded = true;
        }

        public override void Unload()
        {
            //todo: other static fields
            CameraModifier = null;
        }

        private void PostDrawPlayerLayer(Terraria.DataStructures.On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
        {
            //Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default, default);
            foreach(Player player in Main.player)
            {
                if (player.active)
                    foreach (PlayerCode code in ActivePlayerCodes)
                        code.PreDrawPlayer(Main.spriteBatch, player);
            }
            //Main.spriteBatch.End();

            orig(ref drawinfo);

            //Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default, default);
            foreach (Player player in Main.player)
            {
                if (player.active)
                    foreach (PlayerCode code in ActivePlayerCodes)
                        code.PostDrawPlayer(Main.spriteBatch, player);
            }
            //Main.spriteBatch.End();
        }

        public static void ClearAllCodes()
        {
            foreach (MenuCode code in ActiveMenuCodes)
                code.Remove();

            foreach (NpcCode code in ActiveNpcCodes)
                code.Remove();

            foreach (PlayerCode code in ActivePlayerCodes)
                code.Remove();

            foreach (TileCode code in ActiveTileCodes)
                code.Remove();

            foreach (ItemCode code in ActiveItemCodes)
                code.Remove();

            foreach (ProjectileCode code in ActiveProjectileCodes)
                code.Remove();

            foreach (MiscCode code in ActiveMiscCodes)
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

        public enum PacketType
        {
            AllCodes,
            Time,
            NewCode
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            //TODO: request packet from client, send list of all active codes to client (figure out syncing custom data with codes)
            if(Main.netMode == NetmodeID.Server)
            {
                int type = reader.Read7BitEncodedInt();

                if (type == (int)PacketType.AllCodes)//inital message from joining client
                {

                }


            }
            if (Main.netMode == NetmodeID.MultiplayerClient) 
            {
                int type = reader.Read7BitEncodedInt();

                if (type == (int)PacketType.AllCodes)//receiving from server after sending inital packet
                {

                }




                if (type == (int)PacketType.Time)
                {//type, time.now offset, seconds, bool
                    TimeLastCodeSelected = DateTime.Now.Subtract(new TimeSpan(0, 0, reader.Read7BitEncodedInt()));
                    CurrentExtraDelay = new TimeSpan(0, 0, reader.Read7BitEncodedInt());

                    if(reader.ReadBoolean())//should this keep reading
                        type = reader.Read7BitEncodedInt();
                }

                if(type == (int)PacketType.NewCode)
                {//type, code type, code timespan
                    int codeTypeID = reader.Read7BitEncodedInt();
                    Code code = (Code)Activator.CreateInstance(CodeTypes[codeTypeID]);
                    code.TimeActiveSpan = new TimeSpan(0, 0, reader.Read7BitEncodedInt());

                    //if (reader.ReadBoolean())//should this keep reading
                    //    type = reader.Read7BitEncodedInt();
                }
            }
        }
    }

    public static class ModPacketHelper
    {
        public static void WriteAllCodes(this ModPacket modpacket, bool continueWriting)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                modpacket.Write7BitEncodedInt((int)PacketType.AllCodes);//ask server for all codes
                modpacket.Write(continueWriting);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                SyncValueQueue.Clear();
                List<(int typeID, int time, int valueCount)> activeCodeToSync = new List<(int typeID, int time, int valueCount)>();

                #region find all active codes
                void SyncCode(Code code) 
                {
                    Type type = code.GetType();
                    if (ActiveEffects[type])
                    {
                        type.getf
                        int oldCount = SyncValueQueue.Count;
                        code.SyncMultiplayerValues();//if there are any values to sync, they get added to SyncValueQueue

                        activeCodeToSync.Add((CodeTypeID[type], (int)code.TimeActiveSpan.TotalSeconds, SyncValueQueue.Count - oldCount));
                    }
                }

                foreach (Code code in ActiveMenuCodes)
                    SyncCode(code);
                foreach (Code code in ActiveNpcCodes)
                    SyncCode(code);
                foreach (Code code in ActivePlayerCodes)
                    SyncCode(code);
                foreach (Code code in ActiveTileCodes)
                    SyncCode(code);
                foreach (Code code in ActiveItemCodes)
                    SyncCode(code);
                foreach (Code code in ActiveProjectileCodes)
                    SyncCode(code);
                foreach (Code code in ActiveMiscCodes)
                    SyncCode(code);
                #endregion

                foreach((int typeID, int time, int valueCount) tosync in activeCodeToSync)
                {
                    modpacket.Write7BitEncodedInt((int)PacketType.NewCode);//creation of code
                    modpacket.Write7BitEncodedInt(tosync.typeID);
                    modpacket.Write7BitEncodedInt(tosync.time);
                    for(int i = 0; i < tosync.valueCount; i++)
                    {

                    }

                }
            }
        }

        public enum Types
        {
            boolType,
            intType,
            floatType,
            stringType,
            Vector2Type
        }

        public static void WriteDynamic(this ModPacket modpacket, dynamic value)
        {
            //Type type = value.GetType();

            switch (value)//?????????????????????????
            {
                case bool boolType:
                    modpacket.Write7BitEncodedInt((int)Types.boolType);
                    modpacket.Write(boolType);
                    break;
                case int intType:
                    modpacket.Write7BitEncodedInt((int)Types.intType);
                    modpacket.Write7BitEncodedInt(intType);
                    break;
                case float floatType:
                    modpacket.Write7BitEncodedInt((int)Types.floatType);
                    modpacket.Write(floatType);
                    break;
                case string stringType:
                    modpacket.Write7BitEncodedInt((int)Types.stringType);
                    modpacket.Write(stringType);
                    break;
                case Vector2 vector2Type:
                    modpacket.Write7BitEncodedInt((int)Types.Vector2Type);
                    modpacket.WriteVector2(vector2Type);
                    break;
            }
        }

        public static void WriteTime(this ModPacket modpacket, int offset, bool continueWriting = false)
        {
            modpacket.Write7BitEncodedInt((int)PacketType.Time);//time
            modpacket.Write7BitEncodedInt(offset);//offset to time.now, which is set when this packet is received
            modpacket.Write7BitEncodedInt((int)CurrentExtraDelay.TotalSeconds);
            modpacket.Write(continueWriting);
        }

        public static void WriteNewCode(this ModPacket modpacket, Type type, TimeSpan TimeActiveSpan, bool continueWriting = false)
        {
            modpacket.Write7BitEncodedInt((int)PacketType.NewCode);//creation of code
            modpacket.Write7BitEncodedInt(ChaosEdition.CodeTypeID[type]);
            modpacket.Write7BitEncodedInt((int)TimeActiveSpan.TotalSeconds);
            //modpacket.Write(true);
        }
    }

    //Code selection
    public class ChaosEditionSystem : ModSystem
    {
        //TODO find better hook, does not run on server

        //updates and removes codes, also selects codes if timer is up
        public override void PostUpdateEverything()//only called in-world
        {
            if (Main.netMode == NetmodeID.Server)
                UpdateCodes();
        }

        public override void PostUpdateInput()//gets called all the time
        {
            if(Main.netMode != NetmodeID.Server) //will never be called anyway on server, here just to be safe
                UpdateCodes();
        }

        public void UpdateCodes()
        {
            foreach (MenuCode code in ActiveMenuCodes)
                code.CheckActive();

            foreach (NpcCode code in ActiveNpcCodes)
                code.CheckActive();

            foreach (PlayerCode code in ActivePlayerCodes)
                code.CheckActive();

            foreach (TileCode code in ActiveTileCodes)
                code.CheckActive();

            foreach (ItemCode code in ActiveItemCodes)
                code.CheckActive();

            foreach (ProjectileCode code in ActiveProjectileCodes)
                code.CheckActive();

            foreach (MiscCode code in ActiveMiscCodes)
                code.CheckActive();


            while (RemovalQueue.Any())
            {
                var pair = RemovalQueue.Dequeue();
                pair.ListToRemoveFrom.Remove(pair.CodeToRemove);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)//server handles this
            {
                if (ConfigAutoSelectingCodes && ActiveCodeCount < ConfigMaxActiveCodes && TimeSpanUntilNext.TotalSeconds <= 0 && IsModLoaded)
                {
                    CurrentExtraDelay = new TimeSpan();

                    int tries = 0;
                    const int maxTries = 5;
                    while (tries < maxTries)
                    {
                        int index = Main.rand.Next(CodeTypes.Length);
                        if (!ActiveEffects[CodeTypes[index]])
                        {
                            Activator.CreateInstance(CodeTypes[index]);//this is synced in it's ctor
                            return;
                        }
                        else//if code type has already been selected try again
                            tries++;
                        if (tries >= maxTries)//if reached max tries
                        {
                            TimeLastCodeSelected = DateTime.Now.Subtract(NewCodeDelay);//removes code delay since nothing was selected (?) //.Subtract(CodeDelay).Add(RetryCodeDelay);
                            CurrentExtraDelay += RetryCodeDelay;//adds the shorter retry delay

                            if (Main.netMode == NetmodeID.Server)//send these values to MP clients
                            {
                                ModPacket modpacket = ModContent.GetInstance<ChaosEdition>().GetPacket();
                                modpacket.Write(256);//??

                                modpacket.Write7BitEncodedInt(0);//time
                                modpacket.Write7BitEncodedInt((int)NewCodeDelay.TotalSeconds);
                                modpacket.Write7BitEncodedInt((int)CurrentExtraDelay.TotalSeconds);
                                modpacket.Write(false);//stop reading
                            }
                            return;
                        }
                    }
                }
            }


            foreach (MiscCode code in ActiveMiscCodes)
                code.Update();
        }


        //hook for codes that modify interface layers
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //ChaosEdition.ClearSwappedMethods();
            foreach (MenuCode code in ActiveMenuCodes)
                code.ModifyInterfaceLayers(layers);
        }

        //countdown timer and debug active codes list
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            foreach (MenuCode code in ActiveMenuCodes)
                code.PostDrawInterface(spriteBatch);

            foreach (MiscCode code in ActiveMiscCodes)
                code.Draw(spriteBatch);

            //todo find a better position, better offsets, maybe support for ui scale
            //Utils.DrawBorderString(spriteBatch, "Time since last: " + LastCodeSelectedTime.ToShortTimeString(), new Vector2(5, 15), Color.White * 0.75f);
            if(ConfigDrawCountdownTimer)
                Utils.DrawBorderString(spriteBatch, "Time till next: " + Math.Max(0, (int)TimeSpanUntilNext.TotalSeconds), new Vector2(10, 70), Color.White * 0.75f);

            if (ConfigDrawActiveCodes || ConfigDrawFullCodeList || ConfigDrawActiveCodeCount)
            {
                int count = 0;
                int allCount = 0;

                if (ConfigDrawFullCodeList)
                {
                    foreach (KeyValuePair<Type, bool> pair in ActiveEffects)
                    {
                        Color color = pair.Value ? new Color(100, 255, 100) : (allCount % 2 == 0 ? Color.Tomato : Color.CornflowerBlue);
                        Utils.DrawBorderString(spriteBatch, pair.Key.Name + " : " + pair.Value, new Vector2(5, 45 + (15 * allCount)), color);
                        allCount++;
                    }
                }
                else if (ConfigDrawActiveCodes || ConfigDrawActiveCodeCount)
                {

                    void drawcode(Code code)
                    {
                        Type type = code.GetType();
                        bool active = ActiveEffects[type];

                        if (active)
                        {
                            if (ConfigDrawActiveCodes)
                                Utils.DrawBorderString(spriteBatch, type.Name + " " + (int)(code.TimeActiveSpan - DateTime.Now.Subtract(code.TimeCreatedAt)).TotalSeconds, new Vector2(10, 110 + (15 * count)), new Color(100, 255, 100));

                            count++;
                        }
                    }

                    foreach (Code code in ActiveMenuCodes)
                        drawcode(code);
                    foreach (Code code in ActiveNpcCodes)
                        drawcode(code);
                    foreach (Code code in ActivePlayerCodes)
                        drawcode(code);
                    foreach (Code code in ActiveTileCodes)
                        drawcode(code);
                    foreach (Code code in ActiveItemCodes)
                        drawcode(code);
                    foreach (Code code in ActiveProjectileCodes)
                        drawcode(code);
                    foreach (Code code in ActiveMiscCodes)
                        drawcode(code);

                    if (ConfigDrawActiveCodeCount)
                        Utils.DrawBorderString(spriteBatch, "Count: " + count, new Vector2(10, ConfigDrawCountdownTimer ? 90 : 60), Color.White * 0.75f);
                }
            }
        }

        public override void ModifyScreenPosition()
        {
            Main.instance.CameraModifiers.Add(ChaosEdition.CameraModifier);
        }
    }

    public class CameraModifier : ICameraModifier
    {

        public string UniqueIdentity => "Chaos Edition General";

        public bool Finished => false;

        public void Update(ref CameraInfo cameraPosition)
        {
            foreach (MiscCode code in ActiveMiscCodes)
                code.UpdateCamera(ref cameraPosition);
        }
    }
}