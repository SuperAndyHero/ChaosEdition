using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ChaosEdition.Configs
{
	public class InterfaceConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool ShowCountdownTimer { get { return ChaosEdition.ConfigDrawCountdownTimer; } set { ChaosEdition.ConfigDrawCountdownTimer = value; } }

        [DefaultValue(true)]
        public bool ShowActiveEffectCount { get { return ChaosEdition.ConfigDrawActiveCodeCount; } set { ChaosEdition.ConfigDrawActiveCodeCount = value; } }

        [DefaultValue(false)]
        public bool ShowActiveEffectList { get { return ChaosEdition.ConfigDrawActiveCodes; } set { ChaosEdition.ConfigDrawActiveCodes = value; } }

        [DefaultValue(false)]
        public bool ShowActiveEffectTime { get { return ChaosEdition.ConfigDrawActiveTimes; } set { ChaosEdition.ConfigDrawActiveTimes = value; } }

        [DefaultValue(false)]
        public bool DebugShowAllEffects { get { return ChaosEdition.ConfigDrawFullCodeList; } set { ChaosEdition.ConfigDrawFullCodeList = value; } }
    }
}