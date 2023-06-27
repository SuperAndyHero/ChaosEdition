using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace ChaosEdition
{
    public class MenuScaleType : MenuCode
    {
        public override int MaxLengthSeconds => 3;

        public override int NextExtraDelaySeconds => -20;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            foreach (GameInterfaceLayer layer in layers)
            {
                if (Main.rand.NextBool(300)) {
                    if (Main.rand.NextBool(3))
                        layer.ScaleType = InterfaceScaleType.None;
                    else
                        layer.ScaleType = Main.rand.NextBool() ? InterfaceScaleType.Game : InterfaceScaleType.UI;
                }
            }
        }
    }
}
