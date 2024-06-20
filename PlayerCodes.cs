using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition
{
    #region item effects
    //TODO: figure out if this breaks in multiplayer
    public class DropHeldItemRandom : PlayerCode
    {
        public override int MaxLengthSeconds => 100;
        public override int MinLengthSeconds => 40;

        public override int NextExtraDelaySeconds => 7;

        //[NetSync]// for joining clients (not used on clients)
        public int counter = 100;

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            if (counter <= 0)
            {
                if (player.itemTime == 0)
                {
                    player.inventory[player.selectedItem].noGrabDelay = Main.rand.Next(100, 400);
                    player.QuickSpawnItem(player.GetSource_DropAsItem(), player.inventory[player.selectedItem], player.inventory[player.selectedItem].stack);
                    player.inventory[player.selectedItem].TurnToAir();
                }
                counter = Main.rand.Next(550, 2000);
            }
            counter--;
            //Main.NewText(counter);
        }

    }

    //TODO: figure out if this breaks in multiplayer
    public class AlwaysDropOneSlot : PlayerCode
    {
        public override int MaxLengthSeconds => 100;
        public override int MinLengthSeconds => 30;

        public override int NextExtraDelaySeconds => 5;

        public readonly int slot = Main.rand.Next(0, 10);//server side only so not needed
        public int counter = 100;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            if (counter <= 0)
            {
                if (player.itemTime == 0)
                {
                    player.inventory[slot].noGrabDelay = Main.rand.Next(100, 400);
                    player.QuickSpawnItem(player.GetSource_DropAsItem(), player.inventory[slot], player.inventory[slot].stack);
                    player.inventory[slot].TurnToAir();
                }
                counter = Main.rand.Next(550, 2000);
            }
            counter--;
            //Main.NewText(counter);
        }

    }

    //TODO: figure out if this breaks in multiplayer
    public class DropCoinsRandom : PlayerCode
    {
        public override int MaxLengthSeconds => 120;
        public override int MinLengthSeconds => 40;

        public override int NextExtraDelaySeconds => 7;

        private int counter = 100;//serverside effect

        //causes the player to drop their coins at random intervals overs the length of the effect
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            if (counter <= 0)
            {
                player.DropCoins();
                counter = Main.rand.Next(750, 2000);
            }
            counter--;
            //Main.NewText(counter);
        }
    }

    //TODO: figure out if this breaks in multiplayer
    public class DropTombstoneRandom : PlayerCode
    {
        public override int MaxLengthSeconds => 25;

        public override int MinLengthSeconds => 10;

        public override int NextExtraDelaySeconds => 5;

        private int counter = 10;//serverside
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            if (counter <= 0)
            {
                int mes = Main.rand.Next(0, 16);
                player.DropTombstone(0, Lang.CreateDeathMessage(player.name, -1, -1, -1, mes), Main.rand.Next(-10, 11));
                counter = Main.rand.Next(750, 1000);
            }
            counter--;
            //Main.NewText(counter);
        }
    }

    //TODO: figure out if this breaks in multiplayer (clientsided)
    public class MakeOneItemBait : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds =>  -20;
        bool ran = false;

        [NetSync]
        public int index = Main.rand.Next(Main.LocalPlayer.inventory.Length);
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.Server)//clientside
                return;

            if (!ran)//unsure if the server needs to know if an item is bait
            {
                player.inventory[index].bait = Main.rand.Next(1, 1000);
                ran = true;
                //Main.NewText(player.inventory[index].Name + " " + player.inventory[index].bait);
            }
        }
    }

    //TODO: make this not singleplayer exclusive
    public class MakeOneItemHuge : PlayerCode
    {
        public override int MaxLengthSeconds => 120;
        public override int MinLengthSeconds => 30;

        public override int NextExtraDelaySeconds => -3;
        bool ran = false;

        Item iteminstance;//use a dict?

        [NetSync]
        public int index = Main.rand.Next(Main.LocalPlayer.inventory.Length);
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                if (ChaosEdition.HugeItemActive)
                {
                    if (EffectBools[GetType()])
                        Remove();

                    ran = true;
                    return;
                }

                iteminstance = player.inventory[index];

                if(iteminstance is null || iteminstance.IsAir)
                    return;

                iteminstance.scale *= 10;
                ran = true;
                //Main.NewText(player.inventory[index].Name + " " + player.inventory[index].bait);
            }
        }

        public override void OnRemove()
        {
            if (iteminstance == null || iteminstance.IsAir)
                return;
            else
                iteminstance.scale /= 10;//this can be bypassed by dropping items
        }
    }

    //TODO: make this not singleplayer exclusive
    public class RandomItemFiresale : PlayerCode
    {
        public override int MaxLengthSeconds => 10;
        public override int MinLengthSeconds => 10;

        public override int NextExtraDelaySeconds => -7;
        bool ran = false;
        Item selectedItem;

        [NetSync]
        public int seed = Main.rand.Next(1000000);

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                if (ChaosEdition.RandomItemFiresaleActive)
                {
                    if (EffectBools[GetType()])
                        Remove();

                    ran = true;
                    return;
                }

                Random random = new Random(seed);

                int index = Main.rand.Next(player.inventory.Length);
                for (int i = 0; i < 10; i++)
                {
                    if(player.inventory[index] == null || player.inventory[index].IsAir || player.inventory[index].IsACoin)
                    {
                        index = random.Next(player.inventory.Length);
                    }
                    else
                        break;
                }
                if(player.inventory[index] == null || player.inventory[index].IsAir || player.inventory[index].IsACoin)
                {
                    this.Remove();
                    return;
                }

                selectedItem = player.inventory[index];

                int originalPrice = selectedItem.GetStoreValue();

                random = new Random(seed);

                selectedItem.shopCustomPrice =
                    originalPrice > Item.buyPrice(0, 1, 0, 0) ? originalPrice * random.Next(3, 10) :
                        originalPrice > Item.buyPrice(0, 0, 1, 0) ? originalPrice * random.Next(30, 50) :
                            originalPrice == 0 ? Item.buyPrice(0, 0, 2, 0) : Item.buyPrice(0, 0, 5, 0);

                Main.NewText("Firesale! Your '" + selectedItem.Name + "' is worth much more for a limited time!   Ends in 10 seconds.", new Color(255, 100, 25));
                //Main.NewText("Ends in 10 seconds!", Color.OrangeRed);
                ran = true;
            }
        }

        public override void OnRemove()
        {

            if (selectedItem == null || selectedItem.IsAir)
                return;
            else
                selectedItem.shopCustomPrice = null;
        }
    }

    public class HeldItemOneDamageChange : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => -18;
        bool ran = false;

        [NetSync]
        public bool DamageChangeDirection = Main.rand.NextBool();

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                player.HeldItem.damage += DamageChangeDirection ? 3 : -3;
                ran = true;
            }
        }
    }

    //TODO: make this not singleplayer exclusive
    public class OPDirtRod : PlayerCode
    {
        public override int MaxLengthSeconds => 45;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => 40;
        bool ran = false;
        Player playerInstance;//unsure of multiplater compat

        [NetSync]
        public int damage = Main.hardMode ? Main.rand.Next(400, 1600) : Main.rand.Next(180, 350);

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                if (!ChaosEdition.DirtRodEffectActive)
                {
                    if (EffectBools[GetType()])//disable in multiplayer
                        Remove();

                    ran = true;
                    return;
                }

                playerInstance = player;
                int index = Item.NewItem(player.GetSource_GiftOrReward(), player.position, ItemID.DirtRod);
                Main.item[index].damage = damage;

                ran = true;
            }
        }

        public override void OnRemove()
        {
            if (playerInstance != null && playerInstance.active)
                foreach (Item item in playerInstance.inventory)
                {
                    if (item.type == ItemID.DirtRod)
                        item.TurnToAir();
                }
        }
    }

    //check MP
    public class ChristmasGift : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => 20;
        bool ran = false;

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                Main.NewText("Ho Ho Ho!", new Color(50, 255, 80));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bool evilsanta = Main.rand.NextBool(8);
                    bool halloween = Main.rand.NextBool(16);
                    for (int i = -10; i < 10; i++)
                    {
                        if (evilsanta)
                            Projectile.NewProjectile(player.GetSource_GiftOrReward(), player.position + new Vector2(i * 50, -600), new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), 0), ProjectileID.Grenade, 100, 1, player.whoAmI);
                        else if (halloween)
                            Item.NewItem(player.GetSource_GiftOrReward(), player.position + new Vector2(i * 50, -600), ItemID.GoodieBag);
                        else
                            Item.NewItem(player.GetSource_GiftOrReward(), player.position + new Vector2(i * 50, -600), Main.rand.Next(50) == 0 ? ItemID.Coal : ItemID.Present);
                    }
                }

                ran = true;
            }
        }
    }
    #endregion

    public class ExplosiveDiarrhea : PlayerCode
    {
        public override int MaxLengthSeconds => 5;
        public override int MinLengthSeconds => 4;

        [NetSync]
        public int projectileType = PickRandomGrenade();

        public ExplosiveDiarrhea() : base()
        {
            Main.NewText("Explosive Diarrhea!", new Color(100, 150, 50));
        }

        public static int PickRandomGrenade()
        {
            switch (Main.rand.Next(4))
            {
                default:
                    return ProjectileID.Grenade;
                case 1:
                    return ProjectileID.BouncyGrenade;
                case 2:
                    return ProjectileID.PartyGirlGrenade;
                case 3:
                    return ProjectileID.StickyGrenade;
            }
        }

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (Main.GameUpdateCount % 40 == 0)
            {
                Projectile.NewProjectile(player.GetSource_Loot(), player.Center, new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), 0), projectileType, 100, 1, player.whoAmI);
            }
            //Main.NewText(counter);
        }
    }

    //may need tile syncs, seems to work fine
    public class GrowTallPLants : PlayerCode
    {
        public override int MaxLengthSeconds => 10;
        public override int NextExtraDelaySeconds => -12;

        const int radius = 10;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    if (Main.rand.NextBool())
                        WorldGen.GrowTree((int)(player.Bottom.X / 16) - i, (int)(player.Bottom.Y / 16) - j);
                    else
                        WorldGen.GrowEpicTree((int)(player.Bottom.X / 16) - i, (int)(player.Bottom.Y / 16) - j);

                    if (Main.rand.NextBool(25))
                        WorldGen.GrowPalmTree((int)(player.Bottom.X / 16) - i, (int)(player.Bottom.Y / 16) - j);
                    else
                        WorldGen.GrowCactus((int)(player.Bottom.X / 16) - i, (int)(player.Bottom.Y / 16) - j);

                    WorldGen.GrowShroom((int)(player.Bottom.X / 16) - i, (int)(player.Bottom.Y / 16) - j);
                }
            }
        }
    }

    public class ReframeTiles : PlayerCode
    {
        public override int MaxLengthSeconds => 20;

        public override int NextExtraDelaySeconds => -20;
        const int radius = 15;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.Server)//clientside only
                return;

            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    if(Main.rand.NextBool(8))
                        WorldGen.SquareTileFrame((int)player.Bottom.X / 16 + i, (int)player.Bottom.Y / 16 + j, true);
                }
            }
        }
    }

    //todo: this may need to use a vanilla npc sync
    public class TeleportEnemiesAroundPlayer : PlayerCode
    {
        public override int MaxLengthSeconds => 5;

        public override int NextExtraDelaySeconds => -25;
        int count = 0;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (Main.GameUpdateCount % 10 == 0)
            {
                int index = Main.rand.Next(Main.maxNPCs);
                NPC npc = Main.npc[index];
                if (npc.active && !npc.immortal && !npc.dontCountMe && !npc.dontTakeDamage && (!npc.townNPC || Main.rand.Next(50) == 0))
                {
                    npc.position = player.Center + (Vector2.UnitY.RotatedByRandom(Math.PI * 2) * Main.rand.Next(150, 1200));
                    //Main.NewText("Moved npc: " + npc.TypeName);
                    count++;
                }
            }
        }
    }

    public class RunFast : PlayerCode
    {
        public override int MaxLengthSeconds => 180;
        public override int MinLengthSeconds => 60;

        public override int NextExtraDelaySeconds => 5;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            player.powerrun = true;
        }
    }

    public class PlayerFloat : PlayerCode
    {
        public override int MaxLengthSeconds => 30;
        public override int MinLengthSeconds => 20;

        public override int NextExtraDelaySeconds => 10;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            player.pulley = true;
        }
    }

    public class GravityStrength : PlayerCode
    {
        public override int MaxLengthSeconds => 30;
        public override int MinLengthSeconds => 20;
        public override int NextExtraDelaySeconds => 10;

        [NetSync]
        public float gravStr = Main.rand.NextBool() ? Main.rand.NextFloat(0.05f, 0.2f) : Main.rand.NextFloat(0.3f, 2f);

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            player.gravity = gravStr;
        }
    }

    public class RandomTeleport : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => 40;
        bool ran = false;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            //if (Main.netMode == NetmodeID.Server)
            //    return;

            if (!ran)
            {
                bool bossAlive = false;

                foreach (NPC npc in Main.npc)
                    if (npc.active && npc.boss)
                        bossAlive = true;

                if (!bossAlive)
                {
                    player.TeleportationPotion();

                    if (Main.rand.NextBool(30))
                    {
                        int yval = (int)(player.position.Y / 16);
                        if (yval > Main.rockLayer && yval < Main.maxTilesY - 200)
                        {
                            Main.NewText("Welcome to the underground");
                        }
                    }
                }

                ran = true;
            }
        }
    }

    public class SmileGhost : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => 40;

        bool ran = false;//no sync
        //int npcIndex = -1;//no sync
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            if (!ran)
            {
                Vector2 pos = player.position + (Vector2.UnitY * (Main.screenWidth * 0.75f)).RotatedByRandom(Math.Tau);
                NPC.NewNPC(player.GetSource_GiftOrReward(), (int)pos.X, (int)pos.Y, ModContent.NPCType<Npcs.SmileGhost>());
                ran = true;
            }
        }

        //public override void OnRemove()//ghost despawns on its own
        //{
        //    if (Main.npc[npcIndex].active && Main.npc[npcIndex].type == ModContent.NPCType<Npcs.SmileGhost>())
        //    {
        //        Main.npc[npcIndex].active = false;
        //    }
        //}
    }

    //TODO: text does not show in MP
    //public class BoulderDrop : PlayerCode
    //{
    //    public override int MaxLengthSeconds => 80;
    //    public override int MinLengthSeconds => 20;

    //    public override int NextExtraDelaySeconds => 5;
    //    public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
    //    {
    //        if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
    //            return;

    //        if (Main.rand.NextBool(1000))
    //        {
    //            Main.NewText("Look out!", Color.LightYellow);
    //            //positioning is broken in MP
    //            Projectile.NewProjectile(player.GetSource_GiftOrReward(), player.Center + new Vector2(Main.rand.Next(-1, 2), -Main.screenHeight / 1.9f), new Vector2(0, 2f), ProjectileID.Boulder, 200, 1);
    //        }
    //    }
    //}

    public class OresToLead : PlayerCode
    {
        //left out obsidian (not an ore), meteorite (high rarity), and Lead (target ore)
        public static HashSet<int> OreSet = new HashSet<int> {
        TileID.Copper,
        TileID.Tin,
        TileID.Iron,
        TileID.Lead,
        TileID.Silver,
        TileID.Tungsten,
        TileID.Gold,
        TileID.Platinum,
        TileID.Cobalt,
        TileID.Palladium,
        TileID.Orichalcum,
        TileID.Mythril,
        TileID.Adamantite,
        TileID.Titanium,
        TileID.Chlorophyte,
        TileID.Demonite,
        TileID.Crimtane,
        TileID.Hellstone,
        TileID.LunarOre
        };

        public override int MaxLengthSeconds => 240;
        public override int MinLengthSeconds => 60;

        public override int NextExtraDelaySeconds => -10;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            const int radius = 30;

            //for (int f = 0; f < 3; f++)
            //{

            int i = (int)(player.Center.X / 16) + Main.rand.Next(-radius, radius + 1);
            int j = (int)(player.Center.Y / 16) + Main.rand.Next(-radius, radius + 1);

            if (OreSet.Contains(Main.tile[i, j].TileType))
            {
                Main.tile[i, j].Get<TileTypeData>().Type = TileID.Lead;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1, TileChangeType.None);
            }

            if (Main.tile[i, j].TileType == TileID.MetalBars)
            {
                Main.tile[i, j].Get<TileWallWireStateData>().TileFrameX = 18 * 3;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1, TileChangeType.None);
            }
        }
    }

    public class LeadToGold : PlayerCode
    {
        public override int MaxLengthSeconds => 200;
        public override int MinLengthSeconds => 45;

        public override int NextExtraDelaySeconds => -12;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)//serverside
                return;

            const int radius = 35;

            //for (int f = 0; f < 4; f++)
            //{

            int i = (int)(player.Center.X / 16) + Main.rand.Next(-radius, radius + 1);
            int j = (int)(player.Center.Y / 16) + Main.rand.Next(-radius, radius + 1);

            if (Main.tile[i, j].TileType == TileID.Lead || Main.tile[i, j].TileType == TileID.Iron)
            {
                Main.tile[i, j].Get<TileTypeData>().Type = TileID.Gold;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1, TileChangeType.None);
            }

            if (Main.tile[i, j].TileType == TileID.MetalBars && (Main.tile[i, j].TileFrameX == 18 * 2 || Main.tile[i, j].TileFrameX == 18 * 3))
            {
                Main.tile[i, j].Get<TileWallWireStateData>().TileFrameX = 18 * 6;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1, TileChangeType.None);
            }
        }
    }

    public class CloudScroll : PlayerCode
    {
        public override int MaxLengthSeconds => 10;
        public override int MinLengthSeconds => 3;

        public override int NextExtraDelaySeconds => -5;

        bool ran = false;
        Vector2 pos;
        public override void PreDrawPlayer(SpriteBatch sb, Player player, ModPlayer modPlayer = null)
        {
            if (player.whoAmI != Main.LocalPlayer.whoAmI)
                return;

            if (!ran)
            {
                pos = Main.screenPosition;
                ran = true;
            }
            Main.screenLastPosition = pos;
        }
    }

    #region player drawing
    public class RandomPlayerLayerColors : PlayerCode
    {
        public override int MaxLengthSeconds => 360;
        public override int MinLengthSeconds => 60;

        public override int NextExtraDelaySeconds => -16;

        bool ran = false;

        public bool Transparency = Main.rand.NextBool();//not syncing this because visual only and im not sure if it even works

        //not syncing this...
        Color colorHair;
        Color colorEyeWhites;
        Color colorEyes;
        Color colorHead;
        Color colorBodySkin;
        Color colorLegs;
        Color colorShirt;
        Color colorUnderShirt;
        Color colorPants;
        Color colorShoes;
        Color colorArmorHead;
        Color colorArmorBody;
        Color colorMount;
        Color colorArmorLegs;
        Color colorElectricity;
        Color headGlowColor;
        Color bodyGlowColor;
        Color armGlowColor;
        Color legsGlowColor;
        Color ArkhalisColor;
        Color selectionGlowColor;
        Color itemColor;
        Color floatingTubeColor;

        Color RandomColor => 
            new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256), Transparency ? Main.rand.Next(256) : 255);

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                colorHair = RandomColor;
                colorEyeWhites = RandomColor;
                colorEyes = RandomColor;
                colorHead = RandomColor;
                colorBodySkin = RandomColor;
                colorLegs = RandomColor;
                colorShirt = RandomColor;
                colorUnderShirt = RandomColor;
                colorPants = RandomColor;
                colorShoes = RandomColor;
                colorArmorHead = RandomColor;
                colorArmorBody = RandomColor;
                colorMount = RandomColor;
                colorArmorLegs = RandomColor;
                colorElectricity = RandomColor;
                headGlowColor = RandomColor;
                bodyGlowColor = RandomColor;
                armGlowColor = RandomColor;
                legsGlowColor = RandomColor;
                ArkhalisColor = RandomColor;
                selectionGlowColor = RandomColor;
                itemColor = RandomColor;
                floatingTubeColor = RandomColor;

                ran = true;
            }
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            drawInfo.colorHair = colorHair;
            drawInfo.colorEyeWhites = colorEyeWhites;
            drawInfo.colorEyes = colorEyes;
            drawInfo.colorHead = colorHead;
            drawInfo.colorBodySkin = colorBodySkin;
            drawInfo.colorLegs = colorLegs;
            drawInfo.colorShirt = colorShirt;
            drawInfo.colorUnderShirt = colorUnderShirt;
            drawInfo.colorPants = colorPants;
            drawInfo.colorShoes = colorShoes;
            drawInfo.colorArmorHead = colorArmorHead;
            drawInfo.colorArmorBody = colorArmorBody;
            drawInfo.colorMount = colorMount;
            drawInfo.colorArmorLegs = colorArmorLegs;
            drawInfo.colorElectricity = colorElectricity;
            drawInfo.headGlowColor = headGlowColor;
            drawInfo.bodyGlowColor = bodyGlowColor;
            drawInfo.armGlowColor = armGlowColor;
            drawInfo.legsGlowColor = legsGlowColor;
            drawInfo.ArkhalisColor = ArkhalisColor;
            drawInfo.selectionGlowColor = selectionGlowColor;
            drawInfo.itemColor = itemColor;
            drawInfo.floatingTubeColor = floatingTubeColor;
        }
    }

    public class PlayerFloatyRotate : PlayerCode
    {
        public override int MaxLengthSeconds => 160;
        public override int MinLengthSeconds => 30;

        public override int NextExtraDelaySeconds => -16;

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            drawInfo.rotation = (Main.GameUpdateCount * 0.1f) + (drawInfo.drawPlayer.whoAmI * 1.33333f);
            drawInfo.rotationOrigin = (drawInfo.drawPlayer.Size / 2) + new Vector2((float)Math.Sin((Main.GameUpdateCount * 0.033f)) * 10 + drawInfo.drawPlayer.whoAmI, (float)Math.Sin(Main.GameUpdateCount * 0.0225f) * 10 + (drawInfo.drawPlayer.whoAmI * 2.33333f));
        }
    }
    #endregion

    public class ExtendedWingTime : PlayerCode
    {
        public override int MaxLengthSeconds => 180;
        public override int MinLengthSeconds => 60;

        public override int NextExtraDelaySeconds => 0;

        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            player.wingTimeMax = 2000;
            player.wingTime = 2000;
        }
    }
}
