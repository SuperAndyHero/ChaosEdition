using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ChaosEdition.Configs
{
	public class DisableEffectsConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        public bool EnableDirtRodEffect { get { return ChaosEdition.DirtRodEffectActive; } set { ChaosEdition.DirtRodEffectActive = value; } }

        [DefaultValue(true)]
        public bool EnableFireSale { get { return ChaosEdition.RandomItemFiresaleActive; } set { ChaosEdition.RandomItemFiresaleActive = value; } }

        [DefaultValue(true)]
        public bool EnableHugeItem { get { return ChaosEdition.HugeItemActive; } set { ChaosEdition.HugeItemActive = value; } }
    }
}