using HouseKits.Items;
using HamstarHelpers.Helpers.Debug;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Classes.PlayerData;
using HouseKits.Protocols;


namespace HouseKits {
	class HouseKitsCustomPlayer : CustomPlayerData {
		protected override void OnEnter( object data ) {
			if( Main.netMode == 1 ) {
				CustomFurnitureProtocol.QuickRequest();
			}
		}
	}




	class HouseFurnishingKitPlayer : ModPlayer {
		private int CurrentZoomedX;
		private int CurrentZoomedY;
		private bool HasPreviousHouseViableAlert = false;


		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void PreUpdate() {
			if( Main.netMode != 2 ) {
				this.PreUpdateLocal();
			}
		}
		
		private void PreUpdateLocal() {
			if( Main.myPlayer != this.player.whoAmI ) {
				return;
			}
			
			int zoomedX = (int)this.player.Center.X / 64;
			int zoomedY = (int)this.player.Center.Y / 64;

			if( this.CurrentZoomedX == 0 ) {
				this.CurrentZoomedX = zoomedX;
				this.CurrentZoomedY = zoomedY;
			} else {
				if( this.CurrentZoomedX != zoomedX || this.CurrentZoomedY != zoomedY ) {
					this.CurrentZoomedX = zoomedX;
					this.CurrentZoomedY = zoomedY;

					this.UpdateViableHouseCheck();
				}
			}
		}

		////

		private void UpdateViableHouseCheck() {
			bool? isViable = this.CheckIfViableHouseIfNearby();

			if( isViable.HasValue && isViable.Value ) {
				if( !this.HasPreviousHouseViableAlert ) {
					this.HasPreviousHouseViableAlert = true;

					Color color;
					string msg = HouseFurnishingKitItem.GetViabilityStateMessage( HouseViabilityState.Good, out color );

					Main.NewText( msg, color );
				}
			} else {
				this.HasPreviousHouseViableAlert = false;
			}
		}


		////////////////

		public bool? CheckIfViableHouseIfNearby() {
			int tileX = (int)this.player.Center.X / 16;
			int tileY = (int)this.player.Center.Y / 16;
			int houseKitItemType = ModContent.ItemType<HouseFurnishingKitItem>();

			for( int i = 0; i < this.player.inventory.Length; i++ ) {
				Item item = this.player.inventory[i];
				if( item == null || item.IsAir || item.type != houseKitItemType ) {
					continue;
				}

				HouseViabilityState state = HouseFurnishingKitItem.IsValidHouse( tileX, tileY );
				return state == HouseViabilityState.Good;
			}

			return null;
		}
	}
}
