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
	public class AngryGhost : SmileGhost
	{
        public override LocalizedText DisplayName => ModContent.GetInstance<SmileGhost>().DisplayName;

        public override string musicstring => "ChaosEdition/Npcs/angryghost";
        public override float maxspeed => 4f;
        public override float velrotinc => 1.1f;
        public override float speedinc => 0.25f;
        public override int AdditionalTimeMinimum => 25;

        public override void PostSetDefaults()
        {
            NPC.lifeMax = 750;
            NPC.value = 120f;
        }

        public override void OnKill()
        {
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Explosion>(), 0, 0);
            Item.NewItem(NPC.GetSource_Loot(), NPC.Center, ItemID.MeowmereMinecart);
        }
    }
}