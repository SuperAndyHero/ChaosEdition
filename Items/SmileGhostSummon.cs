using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosEdition.Items
{
	public class SmileGhostSummon : ModItem
	{
		public override void SetStaticDefaults() 
		{
			//DisplayName.SetDefault("TestItem"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			//Tooltip.SetDefault("This is a basic modded sword.");
		}

		public override void SetDefaults() 
		{
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 1;
			Item.maxStack = 9999;
			Item.autoReuse = false;
			Item.consumable = true;
		}

        public override bool? UseItem(Player player)
        {
			if (player.controlUseItem)
			{
                new SmileGhost();
            }
			return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.ManaCrystal, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}