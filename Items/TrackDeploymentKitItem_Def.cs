using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PrefabKits.Protocols;
using Terraria;
using Terraria.ModLoader;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		public const int ItemWidth = 24;
		public const int ItemHeight = 24;

		public readonly static int FrameWidth = 16;
		public readonly static int FrameHeight = 8;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Track Deployment Kit" );
			this.Tooltip.SetDefault( "Deploys a train rail spool in the direction you're facing"
				+"\nPlaces rails as close to the ground as it can contiguously"
				+"\nContains 100 tracks" );
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
			this.item.value = PrefabKitsConfig.Instance.TrackDeploymentKitTracks * 100;
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
			int tileX = Main.mouseX;
			int tileY = Main.mouseY;

			if( Main.netMode != 2 && Main.myPlayer == player.whoAmI ) {
				TrackDeploymentKitItem.Deploy( Main.LocalPlayer.direction == 1, tileX, tileY );

				if( Main.netMode == 1 ) {
					TrackDeploymentProtocol.BroadcastFromClient( Main.LocalPlayer.direction == 1, tileX, tileY );
				}
			}

			return true;
		}
	}
}
