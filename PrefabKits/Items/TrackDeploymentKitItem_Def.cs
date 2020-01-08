using System;
using Terraria;
using Terraria.ModLoader;
using PrefabKits.Tiles;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		public const int ItemWidth = 32;
		public const int ItemHeight = 32;



		////////////////

		public (int TileX, int TileY, int dir) ResumeDeploymentAt { get; private set; }



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Track Deployment Kit" );
			this.Tooltip.SetDefault( "Unfurls a spool of train tracks in the direction you're facing"
				+ "\nPlaces " + PrefabKitsConfig.Instance.TrackDeploymentKitTracks + " tracks in a row"
				+ "\nAlways places the row as close to the ground as it can while also clearing oncoming obstacles"
				+ "\nKits in your hands will auto-deploy when you ride to the end of the unfurled row" );
		}

		public override void SetDefaults() {
			this.item.width = TrackDeploymentKitItem.ItemWidth;
			this.item.height = TrackDeploymentKitItem.ItemHeight;
			this.item.maxStack = 99;
			this.item.useTurn = true;
			//this.item.autoReuse = true;
			this.item.useAnimation = 15;
			this.item.useTime = 10;
			this.item.useStyle = 1;
			this.item.consumable = true;
			this.item.createTile = ModContent.TileType<TrackDeploymentTile>();
			this.item.placeStyle = 0;
			this.item.value = PrefabKitsConfig.Instance.TrackDeploymentKitTracks * 100;
			//this.item.UseSound = SoundID.Item108;
			this.item.rare = 2;
		}


		////////////////

		/*public override bool UseItem( Player player ) {
			if( player.itemAnimation > 0 && player.itemTime == 0 ) {
				player.itemTime = item.useTime;
				return true;
			}
			return base.UseItem( player );
		}

		public override bool ConsumeItem( Player player ) {
			this.item.createTile = -1;

			if( player.whoAmI != Main.myPlayer ) {
				return base.ConsumeItem( player );
			}
			
			bool isFacingRight = player.direction == 1;
			int tileX = Main.mouseX + (int)Main.screenPosition.X;
			int tileY = Main.mouseY + (int)Main.screenPosition.Y;
			tileX = tileX >> 4;
			tileY = tileY >> 4;
			
			if( Main.tile[tileX, tileY]?.active() == true ) {
				if( Main.tile[tileX, tileY].type != TileID.MinecartTrack ) {
					return false;
				}
			}
			if( Main.tile[tileX, tileY-1]?.active() != true
				&& Main.tile[tileX, tileY+1]?.active() != true
				&& Main.tile[tileX-1, tileY]?.active() != true
				&& Main.tile[tileX+1, tileY]?.active() != true ) {
				return false;
			}

			if( Main.netMode != 2 && Main.myPlayer == player.whoAmI ) {
				TrackDeploymentKitItem.Deploy( isFacingRight, tileX, tileY );

				if( Main.netMode == 1 ) {
					TrackDeploymentProtocol.BroadcastFromClient( isFacingRight, tileX, tileY );
				}
			}
			
			return true;
		}

		public override void OnConsumeItem( Player player ) {
			this.item.createTile = 1;
		}*/
	}
}
