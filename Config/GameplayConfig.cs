using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ChaosEdition.Configs
{
	public class GameplayConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

        //could use better name, like enable selecting effects
        [DefaultValue(true)]
        public bool EnableChaosEdition { get { return ChaosEdition.ConfigAutoSelectingCodes; } set { ChaosEdition.ConfigAutoSelectingCodes = value; } }

        [Increment(1)]
        [Range(1, 30)]
        [DefaultValue(10)]
        //[Slider] // The Slider attribute makes this field be presented with a slider rather than a text input. The default ticks is 1.
        public int MaxActiveEffects { get { return ChaosEdition.ConfigMaxActiveCodes; } set { ChaosEdition.ConfigMaxActiveCodes = value; } }

        [Increment(1)]
        [Range(2, 600)]
        [DefaultValue(ChaosEdition.DefaultDelayBetweenCodes)]
        public int DelayBetweenEffects { get { return ChaosEdition.ConfigDelayBetweenCodes; } set { ChaosEdition.ConfigDelayBetweenCodes = value; } }
    }
}