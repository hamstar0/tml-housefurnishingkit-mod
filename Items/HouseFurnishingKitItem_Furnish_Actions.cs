using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;


namespace HouseFurnishingKit.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		private static void CleanHouse( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[tileX, tileY];
				if( !tile.active() ) { continue; }

				if( HouseFurnishingKitItem.IsCleanableTile(tile) ) {
					WorldGen.KillTile( tileX, tileY, false, false, true );
				}
			}
		}

		private static void MakeHouseWalls( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[tileX, tileY];

				if( !Main.wallHouse[tile.wall] ) {
					WorldGen.PlaceWall( tileX, tileY, WallID.Wood, true );
				}
			}
		}

		////

		private static void MakeHouseTile4x2( ushort tileType, int tileX, int tileY ) {
			for( int i = tileX; i < tileX + 4; i++ ) {
				for( int j = tileY - 1; j <= tileY; j++ ) {
					Main.tile[i, j].active( false );
				}
			}
			WorldGen.Place4x2( tileX, tileY, tileType, 1, 0 );
		}

		private static void MakeHouseTile3x2( ushort tileType, int tileX, int tileY ) {
			for( int i = tileX - 2; i <= tileX; i++ ) {
				for( int j = tileY - 1; j <= tileY; j++ ) {
					Main.tile[i, j].active( false );
				}
			}
			WorldGen.Place3x2( tileX - 1, tileY, tileType, 0 );
		}

		private static void MakeHouseTile2x1( ushort tileType, int tileX, int tileY ) {
			Main.tile[tileX, tileY].active( false );
			Main.tile[tileX+1, tileY].active( false );
			WorldGen.PlaceTile( tileX, tileY, tileType );
		}

		private static void MakeHouseTile1x2( int tileType, int tileX, int tileY ) {
			for( int j = tileY - 1; j <= tileY; j++ ) {
				Main.tile[tileX - 1, j].active( false );
			}

			WorldGen.Place1x2( tileX, tileY, TileID.Chairs, 0 );
			Main.tile[tileX, tileY - 1].frameX += 18;
			Main.tile[tileX, tileY].frameX += 18;
		}

		////

		private static void MakeHouseWallTile3x3( ushort tileType, int tileX, int tileY ) {
			for( int i = tileX; i < tileX+3; i++ ) {
				for( int j = tileY; j < tileY+3; j++ ) {
					Main.tile[i, j].active( false );
				}
			}
			WorldGen.Place3x3Wall( tileX, tileY, tileType, 0 );
		}

		////

		private static void MakeHouseCustomFurnishings( int leftTileX, int rightTileX, int tileY ) {
			(ushort TileType, int Width, int Height) custFurniture = HouseFurnishingKitMod.Instance.CustomFurniture;
			if( custFurniture.TileType != 0 ) {
//Main.NewText( "placing CustomFurniture " + custFurniture.TileType );
//Dust.NewDustPerfect( new Vector2(leftTileX<<4, tileY<<4), 1 );
				switch( custFurniture.TileType ) {
				case TileID.Bottles:
				case TileID.PiggyBank:
					WorldGen.PlaceTile( leftTileX + 5, tileY, TileID.Platforms );
					WorldGen.PlaceTile( leftTileX + 6, tileY, TileID.Platforms );
					WorldGen.PlaceTile( leftTileX + 5, tileY - 1, custFurniture.TileType );
					break;

				default:
					if( custFurniture.Width == 3 && custFurniture.Height == 2 ) {
						HouseFurnishingKitItem.MakeHouseTile3x2( custFurniture.TileType, leftTileX + 5, tileY );
					} else {
						WorldGen.PlaceTile( leftTileX + 5, tileY, custFurniture.TileType );
					}
					break;
				}
			}

			(ushort TileType, int Width, int Height) custWallMount1 = HouseFurnishingKitMod.Instance.Custom3x3WallMount1;
			if( custWallMount1.TileType != 0 ) {
//Main.NewText( "placing Custom3x3WallMount1 " + custWallMount1.TileType );
//Dust.NewDustPerfect( new Vector2(leftTileX<<4, tileY<<4), 1 );
				if( custWallMount1.Width == 3 && custWallMount1.Height == 3 ) {
					HouseFurnishingKitItem.MakeHouseWallTile3x3( custWallMount1.TileType, leftTileX, tileY - 3 );
				}
			}

			(ushort TileType, int Width, int Height) custWallMount2 = HouseFurnishingKitMod.Instance.Custom3x3WallMount2;
			if( custWallMount2.TileType != 0 ) {
//Main.NewText( "placing Custom3x3WallMount2 " + custWallMount2.TileType );
//Dust.NewDustPerfect( new Vector2(leftTileX<<4, tileY<<4), 1 );
				if( custWallMount2.Width == 3 && custWallMount2.Height == 3 ) {
					HouseFurnishingKitItem.MakeHouseWallTile3x3( custWallMount2.TileType, rightTileX - 1, tileY - 3 );
				}
			}
		}
	}
}
