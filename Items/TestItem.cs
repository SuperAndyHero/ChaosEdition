using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition.Items
{
	public class TestItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("TestItem"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			//Tooltip.SetDefault("This is a basic modded sword.");
		}

		public override void SetDefaults() 
		{
			Item.damage = 50;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

        public override bool? UseItem(Player player)
        {
			//new GroundItemRotation();
			if (player.controlUseItem)
			{
				if (player.altFunctionUse == 2)
				{
					ChaosEdition.ClearAllCodes();
					//ChaosEdition.ClearSwappedMethods();
				}
				else
				{
					//new MethodSwapCode();
					//new RandomItemFiresale();
					//new MakeOneItemHuge();
					//new ChristmasGift();
					new ExplosionOnNpcDeath();

                    //ChaosEdition.CurrentExtraDelay = new TimeSpan(0, 0, 0);
                    //ChaosEdition.TimeLastCodeSelected = DateTime.Now.Subtract(new TimeSpan(0, 0, ChaosEdition.DefaultDelayBetweenCodes));
                }
			}
			return true;
        }

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	}
}