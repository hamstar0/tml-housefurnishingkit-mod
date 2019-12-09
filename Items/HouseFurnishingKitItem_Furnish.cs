using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using HamstarHelpers.Services.Timers;
using HamstarHelpers.Helpers.Debug;


namespace HouseKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		public static void MakeHouse(
					Player player,
					IList<(ushort TileX, ushort TileY)> innerHouseSpace,
					IList<(ushort TileX, ushort TileY)> fullHouseSpace,
					int floorX,
					int floorY,
					Action onFinish ) {
			(int x, int y) topLeft, topRight;
			int floorLeft, floorRight;
			(int x, int y) farTopLeft, farTopRight;
			IDictionary<int, ISet<int>> occupiedTiles = new Dictionary<int, ISet<int>>();

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

			Timers.SetTimer( "HouseKitsFurnishingDelay", 1, false, () => {
				HouseFurnishingKitItem.MakeHouseWalls( fullHouseSpace );
				HouseFurnishingKitItem.MakeHouseTile( floorLeft,		floorY,	TileID.Beds, 0, 1, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTile( floorRight - 2,	floorY,	TileID.WorkBenches, 0, -1, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTile( floorRight - 3,	floorY,	TileID.Chairs, 0, -1, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseCustomFurnishings( floorLeft, floorRight, floorY, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( TileID.Torches,	topLeft.x,	topLeft.y,	innerHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( TileID.Torches,	topRight.x,	topRight.y,	innerHouseSpace, occupiedTiles );
				onFinish();
				return false;
			} );
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

				//if( tileY == (floorY-2) ) {
				if( tileY == (floorY-1) ) {
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
