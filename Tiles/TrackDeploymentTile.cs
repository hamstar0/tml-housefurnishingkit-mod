using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using PrefabKits.Items;
using PrefabKits.Protocols;


namespace PrefabKits.Tiles {
	class TrackDeploymentTile : ModTile {
		public override void SetDefaults() {
			//var flags = AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile | AnchorType.SolidWithTop;

			Main.tileFrameImportant[this.Type] = true;

			TileObjectData.newTile.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			//
			TileObjectData.newAlternate.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorWall = false;
			TileObjectData.newAlternate.UsesCustomCanPlace = true;
			TileObjectData.addAlternate( 1 );
			/*TileObjectData.newTile.AnchorBottom = new AnchorData( flags, TileObjectData.newTile.Width, 0 );
			TileObjectData.newTile.AnchorAlternateTiles = new[] { (int)TileID.MinecartTrack };
			//
			TileObjectData.newAlternate.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorLeft = new AnchorData( flags, TileObjectData.newTile.Height, 0 );
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { (int)TileID.MinecartTrack };
			TileObjectData.addAlternate( 1 );
			//
			TileObjectData.newAlternate.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorRight = new AnchorData( flags, TileObjectData.newTile.Height, 0 );
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { (int)TileID.MinecartTrack };
			TileObjectData.addAlternate( 2 );
			//
			TileObjectData.newAlternate.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorTop = new AnchorData( flags, TileObjectData.newTile.Height, 0 );
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { (int)TileID.MinecartTrack };
			TileObjectData.addAlternate( 3 );
			//
			TileObjectData.newAlternate.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { (int)TileID.MinecartTrack };
			TileObjectData.addAlternate( 0 );*/
			//
			TileObjectData.addTile( this.Type );
		}


		public override bool CanPlace( int i, int j ) {
			if( i > 0 ) {
				if( j > 0 ) {
					if( Main.tile[i - 1, j - 1]?.active() == true ) {
						return true;
					}
				}
				if( Main.tile[i - 1, j]?.active() == true ) {
					return true;
				}
				if( j < Main.maxTilesY - 1 ) {
					if( Main.tile[i - 1, j + 1]?.active() == true ) {
						return true;
					}
				}
			}

			if( j > 0 ) {
				if( Main.tile[i, j - 1]?.active() == true ) {
					return true;
				}
			}
			if( j < Main.maxTilesY - 1 ) {
				if( Main.tile[i, j + 1]?.active() == true ) {
					return true;
				}
			}

			if( i < Main.maxTilesX - 1 ) {
				if( j > 0 ) {
					if( Main.tile[i + 1, j - 1]?.active() == true ) {
						return true;
					}
				}
				if( Main.tile[i + 1, j]?.active() == true ) {
					return true;
				}
				if( j < Main.maxTilesY - 1 ) {
					if( Main.tile[i + 1, j + 1]?.active() == true ) {
						return true;
					}
				}
			}

			return false;
		}


		public override void PlaceInWorld( int i, int j, Item item ) {
			bool isFacingRight = Main.LocalPlayer.direction == 1;

			Main.tile[i, j].ClearTile();

			TrackDeploymentKitItem.Deploy( isFacingRight, i, j );

			if( Main.netMode == 1 ) {
				TrackDeploymentProtocol.BroadcastFromClient( isFacingRight, i, j );
			}
		}
	}
}
