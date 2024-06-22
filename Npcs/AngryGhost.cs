using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition.Npcs
{
	public class AngryGhost : SmileGhost
	{
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
            Item.NewItem(NPC.GetSource_Loot(), NPC.Center, ItemID.MeowmereMinecart);
        }
    }
}