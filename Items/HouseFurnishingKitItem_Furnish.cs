using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;


namespace HouseFurnishingKit.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		public static void MakeHouse(
					Player player,
					IList<(ushort TileX, ushort TileY)> innerHouseSpace,
					IList<(ushort TileX, ushort TileY)> fullHouseSpace,
					int floorX,
					int floorY ) {
			(int x, int y) topLeft, topRight;
			int floorLeft, floorRight;
			(int x, int y) farTopLeft, farTopRight;

			HouseFurnishingKitItem.FindHousePoints(
				innerHouseSpace,
				floorX,
				floorY,
				out topLeft,
				out topRight,
				out floorLeft,
				out floorRight,
				out farTopLeft,
				out farTopRight
			);

			HouseFurnishingKitItem.MakeHouseWalls( fullHouseSpace );
			HouseFurnishingKitItem.MakeHouseTile4x2( TileID.Beds, floorLeft, floorY );
			//HouseFurnishingKitItem.MakeHouseTile3x2( TileID.Tables, floorRight, floorY );
			HouseFurnishingKitItem.MakeHouseTile2x1( TileID.WorkBenches, floorRight - 1, floorY );
			HouseFurnishingKitItem.MakeHouseTile1x2( TileID.Chairs, floorRight - 2, floorY );

			(ushort TileType, int Width, int Height) custFurniture = HouseFurnishingKitMod.Instance.CustomFurniture;
			if( custFurniture.TileType != 0 ) {
				WorldGen.PlaceTile( floorLeft+5, floorY, custFurniture.TileType );
			}

			(ushort TileType, int Width, int Height) custWallMount1 = HouseFurnishingKitMod.Instance.Custom3x3WallMount1;
			if( custWallMount1.TileType != 0 ) {
				if( custWallMount1.Width == 3 && custWallMount1.Height == 3 ) {
					HouseFurnishingKitItem.MakeHouseWallTile3x3( custWallMount1.TileType, floorLeft, floorY - 5 );
				}
			}
			
			(ushort TileType, int Width, int Height) custWallMount2 = HouseFurnishingKitMod.Instance.Custom3x3WallMount2;
			if( custWallMount2.TileType != 0 ) {
				if( custWallMount2.Width == 3 && custWallMount2.Height == 3 ) {
					HouseFurnishingKitItem.MakeHouseWallTile3x3( custWallMount1.TileType, floorRight - 2, floorY - 5 );
				}
			}

			Main.tile[topLeft.x - 1, topLeft.y].type = TileID.Torches;
			Main.tile[topLeft.x - 1, topLeft.y].active( true );
			Main.tile[topRight.x, topRight.y].type = TileID.Torches;
			Main.tile[topRight.x, topRight.y].active( true );
		}


		////////////////

		private static void FindHousePoints(
					IList<(ushort TileX, ushort TileY)> innerHouseSpace,
					int floorX,
					int floorY,
					out (int x, int y) topLeft,
					out (int x, int y) topRight,
					out int floorLeft,
					out int floorRight,
					out (int x, int y) farTopLeft,
					out (int x, int y) farTopRight ) {
			topLeft = (0, 0);
			topRight = (0, 0);
			floorLeft = 0;
			floorRight = 0;
			farTopLeft = (floorX - 512, floorY - 512);
			farTopRight = (floorX + 512, floorY - 512);

			foreach( (ushort tileX, ushort tileY) in innerHouseSpace ) {
				Tile tile = Main.tile[tileX, tileY];

				if( topLeft.x == 0 ) {
					topLeft.x = topRight.x = tileX;
					topLeft.y = topRight.y = tileY;
				}

				if( tileY == (floorY-2) ) {
					if( floorLeft == 0 ) {
						floorLeft = floorRight = tileX;
					} else {
						if( floorLeft > tileX ) {
							floorLeft = tileX;
						} else if( floorRight < tileX ) {
							floorRight = tileX;
						}
					}
				}

				(int x, int y) oldTopLeft = (topLeft.x - farTopLeft.x, topLeft.y - farTopLeft.y);
				(int x, int y) newTopLeft = (tileX - farTopLeft.x, tileY - farTopLeft.y);

				int oldTopLeftDistSqr = ( oldTopLeft.x * oldTopLeft.x ) + ( oldTopLeft.y * oldTopLeft.y );
				int newTopLeftDistSqr = ( newTopLeft.x * newTopLeft.x ) + ( newTopLeft.y * newTopLeft.y );
				if( newTopLeftDistSqr < oldTopLeftDistSqr ) {
					topLeft.x = tileX;
					topLeft.y = tileY;
				}

				(int x, int y) oldTopRight = (topRight.x - farTopRight.x, topRight.y - farTopRight.y);
				(int x, int y) newTopRight = (tileX - farTopRight.x, tileY - farTopRight.y);

				int oldTopRightDistSqr = ( oldTopRight.x * oldTopRight.x ) + ( oldTopRight.y * oldTopRight.y );
				int newTopRightDistSqr = ( newTopRight.x * newTopRight.x ) + ( newTopRight.y * newTopRight.y );
				if( newTopRightDistSqr < oldTopRightDistSqr ) {
					topRight.x = tileX;
					topRight.y = tileY;
				}
			}
		}


		////////////////

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
			for( int i = tileX - 3; i <= tileX; i++ ) {
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
			WorldGen.Place3x3Wall( tileX - 1, tileY, tileType, 0 );
		}
	}
}
