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
        public bool EnableCodeSelection { get { return ChaosEdition.ConfigAutoSelectingCodes; } set { ChaosEdition.ConfigAutoSelectingCodes = value; } }

        [Increment(1)]
        [Range(1, 30)]
        [DefaultValue(10)]
        //[Slider] // The Slider attribute makes this field be presented with a slider rather than a text input. The default ticks is 1.
        public int MaxActiveEffects { get { return ChaosEdition.ConfigMaxActiveCodes; } set { ChaosEdition.ConfigMaxActiveCodes = value; } }

        [Increment(1)]
        [Range(2, 600)]
        [Tooltip("Length of effects is scaled by this")]
        public int DelayBetweenEffects { get { return ChaosEdition.ConfigDelayBetweenCodes; } set { ChaosEdition.ConfigDelayBetweenCodes = value; } }

        //[Range(0.1f, 20)]
        //public float EffectLengthMultiplier { get { return ChaosEdition.ConfigEffectLengthMult; } set { ChaosEdition.ConfigEffectLengthMult = value; } }


        //int a = 1;//fixes a weird issue with the above incement attribute counting to the below option

        //[Increment(1)]
        [Range(0.1f, 20f)]//, Increment(0.25f)]
        [DefaultValue(1)]
        //[Slider]
        //[DrawTicks]
        public float ConfigLengthMultiplier { get { return ChaosEdition.ConfigCodeLengthMult; } set { ChaosEdition.ConfigCodeLengthMult = value; } }
    }
}