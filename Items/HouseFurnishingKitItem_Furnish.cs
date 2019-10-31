using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;


namespace AdventureMode.Items {
	partial class HouseFurnishingKitItem : ModItem {
		public static void MakeHouse(
					Player player,
					IList<(ushort TileX, ushort TileY)> innerHouseSpace,
					IList<(ushort TileX, ushort TileY)> fullHouseSpace,
					int floorX,
					int floorY ) {
			(int x, int y) topLeft=(0,0), topRight=(0,0);
			int floorLeft=0, floorRight=0;
			(int x, int y) farTopLeft = (floorX - 512, floorY - 512);
			(int x, int y) farTopRight = (floorX + 512, floorY - 512);

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

			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[tileX, tileY];

				if( !Main.wallHouse[ tile.wall ] ) {
					WorldGen.PlaceWall( tileX, tileY, WallID.Wood, true );
				}
			}

			for( int i=floorLeft; i<floorLeft+4; i++ ) {
				for( int j=floorY-1; j<=floorY; j++ ) {
					Main.tile[i, j].active( false );
				}
			}
			WorldGen.Place4x2( floorLeft, floorY, TileID.Beds, 1, 0 );
			
			for( int i=floorRight-3; i<=floorRight; i++ ) {
				for( int j=floorY-1; j<=floorY; j++ ) {
					Main.tile[i, j].active( false );
				}
			}
			WorldGen.Place3x2( floorRight-1, floorY, TileID.Tables, 0 );

			for( int j = floorY-1; j <= floorY; j++ ) {
				Main.tile[floorRight-4, j].active( false );
			}
			WorldGen.Place1x2( floorRight-3, floorY, TileID.Chairs, 0 );
			Main.tile[ floorRight-3, floorY-1 ].frameX += 18;
			Main.tile[ floorRight-3, floorY ].frameX += 18;

			Main.tile[ topLeft.x-1, topLeft.y ].type = TileID.Torches;
			Main.tile[ topLeft.x-1, topLeft.y ].active( true );
			Main.tile[ topRight.x, topRight.y ].type = TileID.Torches;
			Main.tile[ topRight.x, topRight.y ].active( true );
		}
	}
}
