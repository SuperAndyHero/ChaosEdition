using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace ChaosEdition
{
    public class ChaosEditionPlayer : ModPlayer
    {
        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            foreach (PlayerCode code in ChaosEdition.PlayerCodes)
                code.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            foreach (PlayerCode code in ChaosEdition.PlayerCodes)
                code.ModifyDrawHeadLayers(layers);
        }
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            foreach (PlayerCode code in ChaosEdition.PlayerCodes)
                code.ModifyDrawInfo(ref drawInfo);
        }
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            foreach (PlayerCode code in ChaosEdition.PlayerCodes)
                code.ModifyDrawLayers(layers);
        }
        public override void PreUpdate()
        {
            foreach (PlayerCode code in ChaosEdition.PlayerCodes)
                code.PreUpdatePlayer(player, this);
        }
        public override void PostUpdate()
        {
            foreach (PlayerCode code in ChaosEdition.PlayerCodes)
                code.PostUpdatePlayer(player, this);
        }
        public override void UpdateBiomeVisuals()
        {
            player.ManageSpecialBiomeVisuals("testInvert", ChaosEdition.ActiveEffects[typeof(InvertScreen)], player.Center);
            player.ManageSpecialBiomeVisuals("Test2", ChaosEdition.ActiveEffects[typeof(ScreenGameboy)], player.Center);
            player.ManageSpecialBiomeVisuals("Sandstorm", ChaosEdition.ActiveEffects[typeof(ScreenRed)], player.Center);
            player.ManageSpecialBiomeVisuals("BloodMoon", ChaosEdition.ActiveEffects[typeof(ScreenRed)], player.Center);
            player.ManageSpecialBiomeVisuals("ChaosEdition:Moonlord", ChaosEdition.ActiveEffects[typeof(ScreenMoonlord)], player.Center);
        }

        public override void PreUpdateBuffs()
        {
            if (ChaosEdition.ActiveEffects[typeof(GravityFlip)])
            {
                player.gravControl = true;
                if (player.position.Y > 1500)
                    player.gravDir = -1f;
            }
        }
    }
}