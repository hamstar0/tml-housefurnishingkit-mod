using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;


namespace AdventureMode.Items {
	partial class HouseFurnishingKitItem : ModItem {
		public static int Width = 24;
		public static int Height = 22;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "House Furnishing Kit" );
			this.Tooltip.SetDefault( "Attempts to transform a given space into a livable area" );
		}

		public override void SetDefaults() {
			this.item.width = HouseFurnishingKitItem.Width;
			this.item.height = HouseFurnishingKitItem.Height;
			this.item.consumable = true;
			this.item.useStyle = 4;
			this.item.useTime = 30;
			this.item.useAnimation = 30;
			//this.item.UseSound = SoundID.Item108;
			this.item.maxStack = 1;
			this.item.value = Item.buyPrice( 0, 10, 0, 0 );
			this.item.rare = 2;
		}


		////////////////

		public override bool UseItem( Player player ) {
			if( player.itemAnimation > 0 && player.itemTime == 0 ) {
				player.itemTime = item.useTime;
				return true;
			}
			return base.UseItem( player );
		}

		public override bool ConsumeItem( Player player ) {
			int tileX = (int)player.Center.X >> 4;
			int tileY = (int)player.Center.Y >> 4;
			IList<(ushort TileX, ushort TileY)> innerHouseSpace, fullHouseSpace;
			int floorX, floorY;
			
			HouseViabilityState state = HouseFurnishingKitItem.IsValidHouse(
				tileX,
				tileY,
				out innerHouseSpace,
				out fullHouseSpace,
				out floorX,
				out floorY
			);

			if( state == HouseViabilityState.Good ) {
				HouseFurnishingKitItem.MakeHouse( player, innerHouseSpace, fullHouseSpace, floorX, floorY );
				return true;
			} else {
				Color color;
				String msg = HouseFurnishingKitItem.GetViabilityStateMessage( state, out color );

				Main.NewText( msg, color );
			}

			return false;
		}
	}
}
