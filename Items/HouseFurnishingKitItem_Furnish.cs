using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using HamstarHelpers.Services.Timers;
using HamstarHelpers.Helpers.Debug;


namespace PrefabKits.Items {
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

			//

			bool checkPlacement( int x, int y, int width ) {
				for( int x2 = x; x2 < x+width; x2++ ) {
					for( int y2 = y; y2 >= y-3; y2-- ) {
						if( Main.tile[x2, y2].active() && Main.tileSolid[Main.tile[x2, y2].type] ) {
							return false;
						}
					}
				}
				return true;
			}
			
			bool placeBed( int x, int y ) {
				if( !checkPlacement( x, y, 4 ) ) {
					return false;
				}
				return HouseFurnishingKitItem.MakeHouseTile( x, y, TileID.Beds, 0, 1, fullHouseSpace, occupiedTiles );
			}
			bool placeWorkbench( int x, int y ) {
				if( !checkPlacement(x, y, 2) ) {
					return false;
				}
				return HouseFurnishingKitItem.MakeHouseTile( x, y, TileID.WorkBenches, 0, 1, fullHouseSpace, occupiedTiles );
			}
			bool placeChair( int x, int y ) {
				if( !checkPlacement(x, y, 1) ) {
					return false;
				}
				return HouseFurnishingKitItem.MakeHouseTile( x, y, TileID.Chairs, 0, 1, fullHouseSpace, occupiedTiles );
			}
			bool placeTorch( int x, int y ) {
				return WorldGen.PlaceTile( x, y, TileID.Torches );
			}

			//

			Timers.SetTimer( "HouseKitsFurnishingDelay", 1, false, () => {
				HouseFurnishingKitItem.MakeHouseWalls( fullHouseSpace );
				HouseFurnishingKitItem.MakeHouseTileNear( placeBed,			floorLeft,		floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeWorkbench,	floorRight - 2,	floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeChair,		floorRight - 3,	floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseCustomFurnishings(			floorLeft,		floorRight, floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeTorch,		topLeft.x,		topLeft.y, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeTorch,		topRight.x,		topRight.y, fullHouseSpace, occupiedTiles );

				if( PrefabKitsConfig.Instance.CustomFloorTile > 0 ) {
					HouseFurnishingKitItem.ChangeFlooring(
						PrefabKitsConfig.Instance.CustomFloorTile,
						floorLeft,
						floorRight,
						floorY
					);
				}
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
