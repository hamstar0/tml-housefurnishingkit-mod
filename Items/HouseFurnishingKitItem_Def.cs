using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace PrefabKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		public static int Width = 36;
		public static int Height = 28;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "House Furnishing Kit" );
			this.Tooltip.SetDefault(
				"Attempts to transform a given space into a livable area"
				+"\nRequires a closed, minimally-sized, unobstructed area"
				+"\nWarning: This will remove ALL objects within the current area"
			);
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
			this.item.value = PrefabKitsConfig.Instance.HouseFurnishingKitPrice;
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
				foreach( Func<int, int, Item, bool> func in PrefabKitsMod.Instance.OnPreHouseCreate ) {
					if( !func(tileX, tileY, this.item) ) {
						return false;
					}
				}
				
				HouseFurnishingKitItem.MakeHouse( player, innerHouseSpace, fullHouseSpace, floorX, floorY, () => {
					foreach( Action<int, int, Item> action in PrefabKitsMod.Instance.OnPostHouseCreate ) {
						action( tileX, tileY, this.item );
					}
				} );

				return true;
			} else {
				Color color;
				String msg = HouseFurnishingKitItem.GetViabilityStateMessage( state, fullHouseSpace.Count, innerHouseSpace.Count, out color );

				Main.NewText( msg, color );
			}

			return false;
		}
	}
}
