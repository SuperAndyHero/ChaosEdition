using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace ChaosEdition
{
    public class ChaosEditionPlayer : ModPlayer
    {
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            foreach (PlayerCode code in ChaosEdition.ActivePlayerCodes)
                code.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            foreach (PlayerCode code in ChaosEdition.ActivePlayerCodes)
                code.ModifyDrawInfo(ref drawInfo);
        }
        public override void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions)
        {
            foreach (PlayerCode code in ChaosEdition.ActivePlayerCodes)
                code.ModifyDrawLayerOrdering(positions);
        }
        public override void PreUpdate()
        {
            foreach (PlayerCode code in ChaosEdition.ActivePlayerCodes)
                code.PreUpdatePlayer(Player, this);
        }
        public override void PostUpdate()
        {
            foreach (PlayerCode code in ChaosEdition.ActivePlayerCodes)
                code.PostUpdatePlayer(Player, this);
        }

        public override void PreUpdateBuffs()
        {
            if (ChaosEdition.CodeInfo[typeof(GravityFlip)].Running)
            {
                Player.gravControl = true;
                if (Player.position.Y > 1500)
                    Player.gravDir = -1f;
            }
        }
    }

    public class ChaosEditionSceneEffect : ModSceneEffect
    {
        public override void SpecialVisuals(Player player, bool isActive)
        {
            //if (isActive)//this is never true for some reason
            //{
                player.ManageSpecialBiomeVisuals("testInvert", !ChaosEdition.ConfigScreenInvertShow && ChaosEdition.CodeInfo[typeof(InvertScreen)].Running, player.Center);
                player.ManageSpecialBiomeVisuals("Test2", ChaosEdition.CodeInfo[typeof(ScreenGameboy)].Running, player.Center);
                player.ManageSpecialBiomeVisuals("Sandstorm", ChaosEdition.CodeInfo[typeof(ScreenRed)].Running, player.Center);
                player.ManageSpecialBiomeVisuals("BloodMoon", ChaosEdition.CodeInfo[typeof(ScreenRed)].Running, player.Center);
                player.ManageSpecialBiomeVisuals("ChaosEdition:Moonlord", ChaosEdition.CodeInfo[typeof(ScreenMoonlord)].Running, player.Center);
            //}
        }
    }
}