using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using HamstarHelpers.Helpers.Tiles;


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

			HouseFurnishingKitItem.CleanHouse( fullHouseSpace );
			HouseFurnishingKitItem.MakeHouseWalls( fullHouseSpace );
			HouseFurnishingKitItem.MakeHouseTile4x2( TileID.Beds, floorLeft + 1, floorY );
			//HouseFurnishingKitItem.MakeHouseTile3x2( TileID.Tables, floorRight - 1, floorY );
			HouseFurnishingKitItem.MakeHouseTile2x1( TileID.WorkBenches, floorRight - 2, floorY );
			HouseFurnishingKitItem.MakeHouseTile1x2( TileID.Chairs, floorRight - 3, floorY );
			HouseFurnishingKitItem.MakeHouseCustomFurnishings( floorLeft + 1, floorRight - 1, floorY );

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
	}
}
