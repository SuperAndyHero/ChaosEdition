using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ChaosEdition.Configs
{
	public class DisableEffectsConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool CheatyCodes
        {
            get { return ChaosEdition.ConfigCheatyCodes; }
            set
            {
                ChaosEdition.ConfigCheatyCodes = value;
                ChaosEdition.RebuildWeightedRandom();
            }
        }

        [DefaultValue(false)]
        public bool DestructiveCodes 
        { 
            get { return ChaosEdition.ConfigDestructiveCodes; } 
            set 
            { 
                ChaosEdition.ConfigDestructiveCodes = value;
                ChaosEdition.RebuildWeightedRandom();
            } 
        }
    }
}