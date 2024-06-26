using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ChaosEdition.Configs
{
	public class InterfaceConfig : ModConfig
	{
        #pragma warning disable CS0618 // Type or member is obsolete
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        [BackgroundColor(100, 230, 100)]
        public bool DisableScreenInvert { get { return ChaosEdition.ConfigScreenInvertShow; } set { ChaosEdition.ConfigScreenInvertShow = value; } }

        [DefaultValue(true)]
        public bool ShowCountdownTimer { get { return ChaosEdition.ConfigDrawCountdownTimer; } set { ChaosEdition.ConfigDrawCountdownTimer = value; } }

        [DefaultValue(true)]
        public bool ShowActiveEffectCount { get { return ChaosEdition.ConfigDrawActiveCodeCount; } set { ChaosEdition.ConfigDrawActiveCodeCount = value; } }

        [DefaultValue(false)]
        public bool ShowActiveEffectList { get { return ChaosEdition.ConfigDrawActiveCodes; } set { ChaosEdition.ConfigDrawActiveCodes = value; } }

        [DefaultValue(false)]
        public bool ShowEffectTimeLeft { get { return ChaosEdition.ConfigDrawActiveTimes; } set { ChaosEdition.ConfigDrawActiveTimes = value; } }

        [DefaultValue(false)]
        [Tooltip("Shows a list of all effects.\nuse PageUp & PageDown.")]
        public bool DebugShowAllEffects { get { return ChaosEdition.ConfigDrawFullCodeList; } set { ChaosEdition.ConfigDrawFullCodeList = value; } }
        #pragma warning restore CS0618 // Type or member is obsolete
    }
}