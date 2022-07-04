using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition
{
    public class DropHeldItemRandom : PlayerCode
    {
        public override int MaxLengthSeconds => 120;
        public override int MinLengthSeconds => 55;

        public override int NextExtraDelaySeconds => 10;

        private int counter = 100;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if(counter <= 0)
            {
                if (player.itemTime == 0)
                {
                    player.inventory[player.selectedItem].noGrabDelay = Main.rand.Next(100, 400);
                    player.QuickSpawnClonedItem(player.inventory[player.selectedItem], player.inventory[player.selectedItem].stack);
                    player.inventory[player.selectedItem].TurnToAir();
                }
                counter = Main.rand.Next(550, 2000);
            }
            counter--;
            //Main.NewText(counter);
        }

    }

    public class AlwaysDropOneSlot : PlayerCode
    {
        public override int MaxLengthSeconds => 120;
        public override int MinLengthSeconds => 30;

        public override int NextExtraDelaySeconds => 5;
        private readonly int slot = Main.rand.Next(0, 10);
        private int counter = 100;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (counter <= 0)
            {
                if (player.itemTime == 0)
                {
                    player.inventory[slot].noGrabDelay = Main.rand.Next(100, 400);
                    player.QuickSpawnClonedItem(player.inventory[slot], player.inventory[slot].stack);
                    player.inventory[slot].TurnToAir();
                }
                counter = Main.rand.Next(550, 2000);
            }
            counter--;
            //Main.NewText(counter);
        }

    }

    public class DropCoinsRandom : PlayerCode
    {
        public override int MaxLengthSeconds => 180;
        public override int MinLengthSeconds => 75;

        public override int NextExtraDelaySeconds => 5;

        private int counter = 100;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (counter <= 0)
            {
                player.DropCoins();
                counter = Main.rand.Next(750, 2000);
            }
            counter--;
            //Main.NewText(counter);
        }
    }

    public class DropTombstoneRandom : PlayerCode
    {
        public override int MaxLengthSeconds => 25;

        public override int MinLengthSeconds => 10;

        public override int NextExtraDelaySeconds => 5;

        private int counter = 10;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
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

    public class MakeOneItemBait : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds =>  -20;
        bool ran = false;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                int index = Main.rand.Next(player.inventory.Length);
                player.inventory[index].bait = Main.rand.Next(1, 666);
                ran = true;
                //Main.NewText(player.inventory[index].Name + " " + player.inventory[index].bait);
            }
        }
    }

    public class HeldItemOneDamageChange : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => -18;
        bool ran = false;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                player.HeldItem.damage += Main.rand.NextBool() ? 1 : -1;
                ran = true;
            }
        }
    }

    public class ExplosiveDiarrhea : PlayerCode
    {
        public override int MaxLengthSeconds => 5;
        public override int MinLengthSeconds => 4;

        public int projectileType = ProjectileID.Grenade;
        public ExplosiveDiarrhea() : base()
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    projectileType = ProjectileID.Grenade;
                    break;
                case 1:
                    projectileType = ProjectileID.BouncyGrenade;
                    break;
                case 2:
                    projectileType = ProjectileID.PartyGirlGrenade;
                    break;
                case 3:
                    projectileType = ProjectileID.StickyGrenade;
                    break;
            }
            Main.NewText("Explosive Diarrhea!", new Color(100, 150, 50));
        }

        private int counter = 1;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (counter <= 0)
            {
                Projectile.NewProjectile(player.Center, new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), 0), projectileType, 100, 1, player.whoAmI);
                counter = 40;
            }
            counter--;
            //Main.NewText(counter);
        }
    }

    public class GrowTallPLants : PlayerCode
    {
        public override int MaxLengthSeconds => 10;

        public override int NextExtraDelaySeconds => -15;
        const int radius = 10;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    if (Main.rand.NextBool())
                        WorldGen.GrowTree((int)(player.Bottom.X / 16) - i, (int)(player.Bottom.Y / 16) - j);
                    else
                        WorldGen.GrowEpicTree((int)(player.Bottom.X / 16) - i, (int)(player.Bottom.Y / 16) - j);

                    if (Main.rand.Next(25) == 0)
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
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    if(Main.rand.Next(8) == 0)
                        WorldGen.SquareTileFrame((int)player.Bottom.X / 16 + i, (int)player.Bottom.Y / 16 + j, true);
                }
            }
        }
    }

    public class TeleportEnemiesAroundPlayer : PlayerCode
    {
        public override int MaxLengthSeconds => 5;

        public override int NextExtraDelaySeconds => -25;
        int count = 0;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (count < 10)
            {
                int index = Main.rand.Next(Main.maxNPCs);
                NPC npc = Main.npc[index];
                if (npc.active && !npc.immortal && !npc.dontCountMe && !npc.dontTakeDamage && (!npc.townNPC || Main.rand.Next(50) == 0))
                {
                    npc.position = player.Center + (Vector2.UnitY.RotatedByRandom(Math.PI * 2) * Main.rand.Next(150, 1200));
                    Main.NewText("Moved npc: " + npc.TypeName);
                    count++;
                }
            }
        }
    }

    public class GravityFlip : PlayerCode
    {
        public override int MaxLengthSeconds => 60;
        public override int MinLengthSeconds => 10;

        public override int NextExtraDelaySeconds => 20;
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
        public readonly float gravStr = Main.rand.NextBool() ? Main.rand.NextFloat(0.05f, 0.2f) : Main.rand.NextFloat(0.3f, 2f);
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            player.gravity = gravStr;
        }
    }

    public class InvertScreen : PlayerCode
    {
        public override int MaxLengthSeconds => 20;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -10;
    }

    public class ScreenGameboy : PlayerCode
    {
        public override int MaxLengthSeconds => 40;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -12;
    }

    public class ScreenRed : PlayerCode
    {
        public override int MaxLengthSeconds => 60;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -15;
    }

    public class ScreenMoonlord : PlayerCode
    {
        public override int MaxLengthSeconds => 30;
        public override int MinLengthSeconds => 15;

        public override int NextExtraDelaySeconds => -5;
    }

    public class RandomTeleport : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => 40;
        bool ran = false;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                bool bossAlive = false;

                foreach (NPC npc in Main.npc)
                    if (npc.active && npc.boss)
                        bossAlive = true;

                if (!bossAlive)
                    player.TeleportationPotion();

                ran = true;
            }
        }
    }

    public class OPDirtRod : PlayerCode
    {
        public override int MaxLengthSeconds => 1;
        public override int MinLengthSeconds => 1;

        public override int NextExtraDelaySeconds => 40;
        bool ran = false;
        public override void PreUpdatePlayer(Player player, ModPlayer modPlayer = null)
        {
            if (!ran)
            {
                int index = Item.NewItem(player.position, ItemID.DirtRod);
                Main.item[index].damage = Main.hardMode ? Main.rand.Next(400, 1600) : Main.rand.Next(180, 350);

                ran = true;
            }
        }
    }

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
                for(int i = -10; i < 10; i++)
                    Item.NewItem(player.position + new Vector2(i * 50, -600), Main.rand.Next(50) == 0 ? ItemID.Coal : ItemID.Present);
                ran = true;
            }
        }
    }
}
