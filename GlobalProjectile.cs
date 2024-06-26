﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class ChaosEditionGlobalProj : GlobalProjectile
    {
        public override void SetDefaults(Projectile projectile)
        {
            foreach (ProjectileCode code in ChaosEdition.ActiveProjectileCodes)
                code.SetDefaults(projectile);
        }
        public override bool PreAI(Projectile projectile)
        {
            bool RunAI = base.PreAI(projectile);
            foreach (ProjectileCode code in ChaosEdition.ActiveProjectileCodes)
                RunAI = RunAI && code.PreAI(projectile);
            return RunAI;
        }
        public override void AI(Projectile projectile)
        {
            foreach (ProjectileCode code in ChaosEdition.ActiveProjectileCodes)
                code.AI(projectile);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            bool DrawProj = base.PreDraw(projectile, ref lightColor);
            foreach (ProjectileCode code in ChaosEdition.ActiveProjectileCodes)
                DrawProj = DrawProj && code.PreDraw(projectile, ref lightColor);
            return DrawProj;
        }
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            foreach (ProjectileCode code in ChaosEdition.ActiveProjectileCodes)
                code.PostDraw(projectile, lightColor);
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            foreach (ProjectileCode code in ChaosEdition.ActiveProjectileCodes)
                code.OnKill(projectile, timeLeft);
        }
    }
}
