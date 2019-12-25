using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using PrefabKits.Items;


namespace PrefabKits {
	/*class PrefabKitsCustomPlayer : CustomPlayerData {
		protected override void OnEnter( object data ) {
			if( Main.netMode == 1 ) {
				CustomFurnitureProtocol.QuickRequest();
			}
		}
	}*/




	class PrefabKitsPlayer : ModPlayer {
		private int CurrentZoomedX;
		private int CurrentZoomedY;

		private bool HasPreviousHouseViableAlert = false;
		private ISet<(int, int)> ChartedHouseSpaces = new HashSet<(int, int)>();


		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void PreUpdate() {
			if( Main.netMode != 2 ) {
				if( Main.myPlayer == this.player.whoAmI ) {
					this.PreUpdateLocal();
				}
			}

			if( PrefabKitsConfig.Instance.DebugModeInfo ) {
				if( Main.mouseRight && Main.mouseRightRelease ) {
					Tile tile = Main.tile[
						(int)(Main.screenPosition.X + Main.mouseX) / 16,
						(int)(Main.screenPosition.Y + Main.mouseY) / 16
					];
					Main.NewText( tile.ToString() );
				}
			}
		}
		
		private void PreUpdateLocal() {
			int frameKitItemType = ModContent.ItemType<HouseFramingKitItem>();
			int furnishKitItemType = ModContent.ItemType<HouseFurnishingKitItem>();

			if( this.UpdateZoomedPosition() ) {
				if( this.player.inventory.Any( i => ((i?.active == true) && (i.type == furnishKitItemType)) ) ) {
					this.CheckFurnishableHouse();
				}
			}
			
			if( this.player.HeldItem?.active == true && this.player.HeldItem.type == frameKitItemType ) {
				this.CheckFrameableHouse();
			}
		}

		////

		private bool UpdateZoomedPosition() {
			int zoomedX = (int)this.player.Center.X / 64;
			int zoomedY = (int)this.player.Center.Y / 64;

			if( this.CurrentZoomedX == 0 ) {
				this.CurrentZoomedX = zoomedX;
				this.CurrentZoomedY = zoomedY;
				return true;
			} else {
				if( this.CurrentZoomedX != zoomedX || this.CurrentZoomedY != zoomedY ) {
					this.CurrentZoomedX = zoomedX;
					this.CurrentZoomedY = zoomedY;
					return true;
				}
			}
			return false;
		}


		////////////////

		private void CheckFurnishableHouse() {
			(int x, int y) pos = (this.CurrentZoomedX, this.CurrentZoomedY);
			if( !this.ChartedHouseSpaces.Contains(pos) ) {
				this.ChartedHouseSpaces.Add( pos );
			} else {
				return;
			}

			HouseViabilityState state = HouseFurnishingKitItem.IsValidHouse(
				(int)this.player.Center.X / 16,
				(int)this.player.Center.Y / 16
			);

			if( state == HouseViabilityState.Good ) {
				if( !this.HasPreviousHouseViableAlert ) {
					this.HasPreviousHouseViableAlert = true;

					Color color;
					string msg = HouseFurnishingKitItem.GetViabilityStateMessage( HouseViabilityState.Good, 0, 0, out color );

					Main.NewText( msg, color );
				}
			} else {
				this.HasPreviousHouseViableAlert = false;
			}
		}

		private void CheckFrameableHouse() {
			int tileX = (int)this.player.Center.X >> 4;
			int tileY = (int)this.player.position.Y >> 4;

			ISet<(int, int)> tiles;
			bool canErect = HouseFramingKitItem.Validate( ref tileX, ref tileY, out tiles );

			Color color;
			Vector2 pos;
			foreach( (int x, int y) in tiles ) {
				pos = new Vector2( (x<<4) + 8, (y<<4) + 8 );
				color = canErect ? Color.Lime : Color.Red;

				Dust dust = Dust.NewDustPerfect(
					Position: pos,
					Velocity: default(Vector2),
					Type: 59,
					Alpha: 0,
					newColor: color,
					Scale: 1.25f
				);
				dust.noGravity = true;
				dust.noLight = true;
			}
		}
	}
}
