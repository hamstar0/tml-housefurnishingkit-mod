using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace PrefabKits.Items {
	public partial class HouseFramingKitItem : ModItem {
		public readonly static int ItemWidth = 24;
		public readonly static int ItemHeight = 22;

		public readonly static int FrameWidth = 16;
		public readonly static int FrameHeight = 8;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "House Framing Kit" );
			this.Tooltip.SetDefault( "Attempts to erect a skeletal house frame" );
		}

		public override void SetDefaults() {
			this.item.width = HouseFramingKitItem.ItemWidth;
			this.item.height = HouseFramingKitItem.ItemHeight;
			this.item.consumable = true;
			this.item.useStyle = 4;
			this.item.useTime = 30;
			this.item.useAnimation = 30;
			//this.item.UseSound = SoundID.Item108;
			this.item.maxStack = 1;
			this.item.value = PrefabKitsConfig.Instance.HouseFramingKitPrice;
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
			int tileY = (int)player.position.Y >> 4;

			ISet<(int, int)> _;
			bool canErect = HouseFramingKitItem.Validate( ref tileX, ref tileY, out _ );

			if( canErect ) {
				HouseFramingKitItem.MakeHouseFrame( tileX, tileY );
			} else {
				Main.NewText( "Not enough open space.", Color.Yellow );
			}

			return canErect;
		}
	}
}
